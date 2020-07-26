using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BGStats.Bot.Models
{
  public class Play
  {
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }

    [JsonPropertyName("modificationDate")]
    public string ModificationDate { get; set; }

    [JsonPropertyName("entryDate")]
    public string EntryDate { get; set; }

    [JsonPropertyName("playDate")]
    public string PlayDate { get; set; }

    [JsonPropertyName("usesTeams")]
    public bool UsesTeams { get; set; }

    [JsonPropertyName("durationMin")]
    public int DurationMin { get; set; }

    [JsonPropertyName("ignored")]
    public bool Ignored { get; set; }

    [JsonPropertyName("manualWinner")]
    public bool ManualWinner { get; set; }

    [JsonPropertyName("rounds")]
    public int Rounds { get; set; }

    [JsonPropertyName("locationRefId")]
    public int LocationRefId { get; set; }

    [JsonPropertyName("gameRefId")]
    public int GameRefId { get; set; }

    [JsonPropertyName("board")]
    public string Board { get; set; }

    [JsonPropertyName("scoringSetting")]
    public int ScoringSetting { get; set; }

    [JsonPropertyName("playerScores")]
    public List<PlayerScore> PlayerScores { get; set; }
  }
}
