using BGStats.Bot.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BGStats.Bot.Services
{
  public interface INotificationService
  {
    Task<List<Subscriber>> GetAllSubscriptions();
    Task<List<Subscriber>> GetSubscriptions(ulong discordId);
    Task Notify(string fileName, Stream stream, PlayFile playFile, ulong sender);
    Task Subscribe(ulong discordId, string playerName);
    Task Unsubscribe(ulong discordId);
    Task Unsubscribe(ulong discordId, string playerName);
  }
}