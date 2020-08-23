using BGStats.Bot.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BGStats.Bot.Models;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;
using System.IO;
using Discord.Addons.Interactive;
using System.Linq;

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

    public async Task<List<Subscriber>> GetSubscriptions(ulong discordId)
    {
      return await _subscriberContext.Subscribers.AsNoTracking().Where(x => x.DiscordId == discordId).ToListAsync();
    }

    public async Task<List<Subscriber>> GetAllSubscriptions()
    {
      return await _subscriberContext.Subscribers.AsNoTracking().ToListAsync();
    }

    public async Task Subscribe(ulong discordId, string playerName)
    {
      var subscriber = new Subscriber { DiscordId = discordId, PlayerName = playerName };
      await _subscriberContext.Subscribers.AddAsync(subscriber);
      await _subscriberContext.SaveChangesAsync();
    }

    public async Task Unsubscribe(ulong discordId)
    {
      var subscriptions = await _subscriberContext.Subscribers.AsQueryable().Where(x => x.DiscordId == discordId).ToListAsync();
      _subscriberContext.Subscribers.RemoveRange(subscriptions);
      await _subscriberContext.SaveChangesAsync();
    }

    public async Task Unsubscribe(ulong discordId, string playerName)
    {

      var subscription = await _subscriberContext.Subscribers.AsQueryable().FirstOrDefaultAsync(x => x.DiscordId == discordId && x.PlayerName == playerName);
      _subscriberContext.Subscribers.Remove(subscription);
      await _subscriberContext.SaveChangesAsync();
    }

    public async Task Notify(string fileName, Stream stream, PlayFile playFile, ulong sender)
    {
      var targets = await _subscriberContext.Subscribers.AsNoTracking().ToAsyncEnumerable().Where(x => playFile.Players.Any(p => p.Name == x.PlayerName)).Select(s => s.DiscordId).Distinct().ToListAsync();
      
      foreach (var target in targets)
      {
        if (target == sender) continue;

        var channel = await _client.GetUser(target)?.GetOrCreateDMChannelAsync();

        if (channel != null)
        {
          stream.Seek(0, SeekOrigin.Begin);
          await channel.SendFileAsync(stream, fileName, "A play file that included you was shared!");
        }
      }
    }
  }
}
