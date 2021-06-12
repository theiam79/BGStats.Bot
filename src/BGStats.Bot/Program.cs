using BGStats.Bot.Context;
using BGStats.Bot.Modules;
using BGStats.Bot.Services;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
              o.Dsn = context.Configuration["sentry"];
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
            .AddHttpClient()
            .AddTransient<INotificationService, NotificationService>()
            .AddDbContext<SubscriberContext>(options =>
            {
              options.UseSqlite(context.Configuration.GetConnectionString("SubscriberDB"));  
            }, ServiceLifetime.Transient);
        });

      var host = builder.Build();
      using (host)
      {
        await host.MigrateDatabase<SubscriberContext>().RunAsync();
      }
    }
  }

  public static class Extensions
  {
    public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
    {
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          var db = services.GetRequiredService<T>();
          db.Database.Migrate();
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred while migrating the database.");
        }
      }
      return host;
    }
  }
}
