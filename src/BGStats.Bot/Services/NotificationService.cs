using BGStats.Bot.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BGStats.Bot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Discord.WebSocket;
using System.IO;

namespace BGStats.Bot.Services
{
  public class NotificationService : INotificationService
  {
    private readonly SubscriberContext _subscriberContext;
    private readonly DiscordSocketClient _client;

    public NotificationService(SubscriberContext subscriberContext, DiscordSocketClient client)
    {
      _subscriberContext = subscriberContext;
      _client = client;
    }

    public async Task Subscribe(ulong discordId, string playerName)
    {
      var subscriber = new Subscriber { DiscordId = discordId, PlayerName = playerName };
      await _subscriberContext.Subscribers.AddAsync(subscriber);
      await _subscriberContext.SaveChangesAsync();
    }

    public async Task Notify(string fileName, Stream stream, PlayFile playFile)
    {
      var test = _subscriberContext.Subscribers.ToList();
      var targets = await _subscriberContext.Subscribers.AsNoTracking().ToAsyncEnumerable().Where(x => playFile.Players.Any(p => p.Name == x.PlayerName)).Select(s => s.DiscordId).Distinct().ToListAsync();
      
      foreach (var target in targets)
      {
        stream.Seek(0, SeekOrigin.Begin);
        var channel = await _client.GetUser(target)?.GetOrCreateDMChannelAsync();

        if (channel != null)
        {
          await channel.SendFileAsync(stream, fileName, "A play file that included you was shared!");
        }
      }
    }
  }
}
