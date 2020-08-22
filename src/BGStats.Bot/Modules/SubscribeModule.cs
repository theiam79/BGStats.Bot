using BGStats.Bot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BGStats.Bot.Modules
{
  public class SubscribeModule : ModuleBase<SocketCommandContext>
  {
    private readonly INotificationService _notificationService;

    public SubscribeModule(INotificationService notificationService)
    {
      _notificationService = notificationService;
    }

    [Command("subscribe")]
    public async Task Subscribe([Remainder] string playerName)
    {
      await _notificationService.Subscribe(Context.User.Id, playerName);
      await ReplyAsync($"Successfully subscribed for plays that include {playerName}");
    }
  }
}
