using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BGStats.Bot.Models
{
  public class Player
  {
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("isAnonymous")]
    public bool IsAnonymous { get; set; }

    [JsonPropertyName("modificationDate")]
    public string ModificationDate { get; set; }

    [JsonPropertyName("bggUsername")]
    public string BggUsername { get; set; }
  }
}
