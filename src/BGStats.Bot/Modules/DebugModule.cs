using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGStats.Bot.Modules
{
  [Group("debug")]
  [RequireOwner]
  public class DebugModule : ModuleBase<SocketCommandContext>
  {
    [Command("listGuilds")]
    public async Task ListGuilds()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("```");
      foreach (var guild in Context.Client.Guilds)
      {
        builder.AppendLine($"n: {guild.Name}, i: {guild.Id} o: {guild.Owner} ({guild.OwnerId})");
      }

      builder.Append("```");

      await ReplyAsync(builder.ToString());
    }

    [Command("listUsers")]
    public async Task ListUsers(ulong guildId)
    {
      var guild = Context.Client.Guilds.FirstOrDefault(x => x.Id == guildId);
      if (guild == null)
      {
        await ReplyAsync("Unable to find guild");
        return;
      }

      StringBuilder builder = new StringBuilder();
      builder.Append("```");
      int count = 0;
      foreach (var user in guild.Users)
      {
        if (count == 30) break;
        count++;
        builder.AppendLine($"n: {user}, i: {user.Id}, r[0]: {user.Roles.FirstOrDefault(x => !x.IsEveryone)?.Name}");
      }

      builder.Append("```");

      await ReplyAsync("Retrieving first 30 users...");
      await ReplyAsync(builder.ToString());
    }
  }
}
