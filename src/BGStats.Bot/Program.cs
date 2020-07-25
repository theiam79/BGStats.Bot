using BGStats.Bot.Modules;
using BGStats.Bot.Services;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BGStats.Bot
{
  class Program
  {
    static async Task Main()
    {
      var builder = new HostBuilder()
        .ConfigureAppConfiguration(x =>
        {
          var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile("appsettings.development.json", true, true)
            .Build();

          x.AddConfiguration(configuration);
        })
        .UseSerilog((context, config) =>
        {
          config
            .WriteTo.Console()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error);

          if (!string.IsNullOrEmpty(context.Configuration["sentry"]))
          {
            config.WriteTo.Sentry(o =>
            {
              o.Dsn = new Dsn(context.Configuration["sentry"]);
              o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
              o.MinimumEventLevel = LogEventLevel.Error;
            });
          }
        })
        .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
        {
          config.SocketConfig = new DiscordSocketConfig
          {
            LogLevel = LogSeverity.Info,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 200
          };

          config.Token = context.Configuration["token"];
        })
        .UseCommandService((context, config) =>
        {
          config.LogLevel = LogSeverity.Verbose;
          config.DefaultRunMode = RunMode.Async;
        })
        .ConfigureServices((context, services) =>
        {
          services
            .AddHostedService<CommandHandler>()
            .AddSingleton<InteractiveService>()
            .AddTransient<PlayFormatService>()
            .AddTransient<PostingService>()
            .AddSingleton<Helper>()
            .AddHttpClient();
        });

      var host = builder.Build();
      using (host)
      {
        await host.RunAsync();
      }
    }
  }
}
