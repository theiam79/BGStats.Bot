using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BGStats.Bot.Models
{
  public class Game
  {
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("modificationDate")]
    public string ModificationDate { get; set; }

    [JsonPropertyName("cooperative")]
    public bool Cooperative { get; set; }

    [JsonPropertyName("highestWins")]
    public bool HighestWins { get; set; }

    [JsonPropertyName("noPoints")]
    public bool NoPoints { get; set; }

    [JsonPropertyName("usesTeams")]
    public bool UsesTeams { get; set; }

    [JsonPropertyName("urlThumb")]
    public string UrlThumb { get; set; }

    [JsonPropertyName("urlImage")]
    public string UrlImage { get; set; }

    [JsonPropertyName("bggName")]
    public string BggName { get; set; }

    [JsonPropertyName("bggYear")]
    public int BggYear { get; set; }

    [JsonPropertyName("bggId")]
    public int BggId { get; set; }

    [JsonPropertyName("designers")]
    public string Designers { get; set; }

    [JsonPropertyName("isBaseGame")]
    public bool IsBaseGame { get; set; }

    [JsonPropertyName("isExpansion")]
    public bool IsExpansion { get; set; }

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("minPlayerCount")]
    public int MinPlayerCount { get; set; }

    [JsonPropertyName("maxPlayerCount")]
    public int MaxPlayerCount { get; set; }

    [JsonPropertyName("minPlayTime")]
    public int MinPlayTime { get; set; }

    [JsonPropertyName("maxPlayTime")]
    public int MaxPlayTime { get; set; }

    [JsonPropertyName("minAge")]
    public int MinAge { get; set; }
  }
}
