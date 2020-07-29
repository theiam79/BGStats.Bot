using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BGStats.Bot.Models
{
  public class PlayFile
  {
    [JsonPropertyName("about")]
    public string About { get; set; }

    [JsonPropertyName("players")]
    public List<Player> Players { get; set; }

    [JsonPropertyName("locations")]
    public List<Location> Locations { get; set; }

    [JsonPropertyName("games")]
    public List<Game> Games { get; set; }

    [JsonPropertyName("plays")]
    public List<Play> Plays { get; set; }

    [JsonPropertyName("userInfo")]
    public UserInfo UserInfo { get; set; }
  }
}
