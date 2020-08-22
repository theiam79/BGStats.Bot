using BGStats.Bot.Models;
using System.IO;
using System.Threading.Tasks;

namespace BGStats.Bot.Services
{
  public interface INotificationService
  {
    Task Notify(string fileName, Stream fileStream, PlayFile playFile);
    Task Subscribe(ulong discordId, string playerName);
  }
}