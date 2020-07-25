using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace BGStats.Bot.Services
{
  public class PostingService
  {
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _config;

    public PostingService(DiscordSocketClient client, IConfiguration config)
    {
      _client = client;
      _config = config;
    }

    public async Task PostAsync(Embed embed)
    {
      if (!ulong.TryParse(_config["guildId"], out ulong guildId))
      {
        throw new ConfigurationErrorsException("Unable to parse guild Id");
      }
      if (!ulong.TryParse(_config["channelId"], out ulong channelId))
      {
        throw new ConfigurationErrorsException("Unable to parse channel Id");
      }
      
      await _client.GetGuild(guildId).GetTextChannel(channelId).SendMessageAsync(embed: embed);
    }
  }
}
