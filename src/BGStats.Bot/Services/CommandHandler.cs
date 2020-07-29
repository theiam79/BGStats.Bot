using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Sentry;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGStats.Bot.Services
{
  public class CommandHandler : InitializedService
  {
    private readonly IServiceProvider _services;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly IConfiguration _config;
    private readonly Helper _helper;

    public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, IConfiguration config, Helper helper)
    {
      _client = client;
      _commandService = commands;
      _services = services;
      _config = config;
      _helper = helper;

      _client.MessageReceived += HandleCommandAsync;
      _commandService.CommandExecuted += CommandExecutedAsync;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken)
    {
      //_commandService.AddTypeReader<IBan>(new IBanTypeReader());
      await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    private async Task HandleCommandAsync(SocketMessage incomingMessage)
    {
      if (!(incomingMessage is SocketUserMessage message)) return;
      if (message.Source != MessageSource.User) return;

      int argPos = 0;

      if (!(incomingMessage.Channel is SocketDMChannel))
      {
        if (!message.HasStringPrefix(_config["prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;
      }

      using (SentrySdk.PushScope())
      {
        var context = new SocketCommandContext(_client, message);

        SentrySdk.ConfigureScope(s =>
        {
          s.User = new User { Id = message.Author.Id.ToString(), Username = message.Author.ToString() };
          s.SetTag("Guild", context.Guild?.Name ?? "Private Message");
          s.SetTag("Channel", context.Channel?.Name ?? "N/A");
        });

        await _commandService.ExecuteAsync(context, argPos, _services);
      }
    }

    public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
      if (!command.IsSpecified && result.IsSuccess)
        return;

      switch (result.Error)
      {
        case CommandError.UnknownCommand:
          break;
        case CommandError.BadArgCount:
          await context.Channel.SendMessageAsync("Incorrect command usage, showing helper:");
          EmbedBuilder builder = _helper.GetHelpInformationBuilder(command.Value);
          await context.Channel.SendMessageAsync(embed: builder.Build());
          break;
        case CommandError.ParseFailed:
        case CommandError.ObjectNotFound:
        case CommandError.MultipleMatches:
        case CommandError.UnmetPrecondition:
          await context.Channel.SendMessageAsync(result.ErrorReason);
          break;
        case CommandError.Exception:
          await context.Channel.SendMessageAsync($"An error occurred whilst processing this command. This has been reported automatically. (ID: {SentrySdk.LastEventId})");
          break;
        case CommandError.Unsuccessful:
          break;
        case null:
          break;
      }
    }
  }
}
