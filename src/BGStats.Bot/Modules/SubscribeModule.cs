using BGStats.Bot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGStats.Bot.Modules
{
  [Group("subscription")]
  public class SubscribeModule : ModuleBase<SocketCommandContext>
  {
    private readonly INotificationService _notificationService;

    public SubscribeModule(INotificationService notificationService)
    {
      _notificationService = notificationService;
    }

    [Command("add")]
    [Summary("Subscribe to recieve any play files sent by others that include the given player name")]
    public async Task Subscribe([Remainder] string playerName)
    {
      await _notificationService.Subscribe(Context.User.Id, playerName);
      await ReplyAsync($"Successfully subscribed to plays that include {playerName}");
    }

    [Command("remove")]
    [Summary("Remove one of your existing subscriptions")]
    public async Task Unsubscribe([Remainder] string playerName)
    {
      await _notificationService.Unsubscribe(Context.User.Id, playerName);
      await ReplyAsync($"Successfully unsubscribed from plays that include {playerName}");
    }

    [Command("removeAll")]
    [Summary("Remove all of your existing subscriptions")]
    public async Task UnsubscribeAll()
    {
      await _notificationService.Unsubscribe(Context.User.Id);
      await ReplyAsync($"Successfully unsubscribed from all plays");
    }

    [Command("list")]
    [Summary("List all of your current subscriptions")]
    public async Task ListSubscriptions()
    {
      var subs = await _notificationService.GetSubscriptions(Context.User.Id);
      
      var message = new StringBuilder().AppendLine($"Found {subs.Count} subscriptions for {Context.User.Username}").AppendJoin('\n', subs.Where(s => s.DiscordId == Context.User.Id).Select(s => s.PlayerName)).ToString();
      var channel = await Context.User.GetOrCreateDMChannelAsync();
      await channel.SendMessageAsync(message);
    }

    [Command("listAll")]
    [RequireOwner]
    public async Task ListAllSubscriptions()
    {
      var subs = await _notificationService.GetAllSubscriptions();
      var message = new StringBuilder().AppendLine($"Found {subs.Count} total subscriptions").AppendJoin('\n', subs.Select(s => $"{Context.Client.GetUser(s.DiscordId)?.Username ?? $"Something went wrong {s.DiscordId}"} - {s.PlayerName}")).ToString();
      var channel = await Context.User.GetOrCreateDMChannelAsync();
      await channel.SendMessageAsync(message);
    }
  }
}
