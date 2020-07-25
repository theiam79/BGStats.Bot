using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BGStats.Bot.Models
{
  public class PlayerScore
  {
    [JsonPropertyName("score")]
    public string Score { get; set; }

    [JsonPropertyName("winner")]
    public bool Winner { get; set; }

    [JsonPropertyName("newPlayer")]
    public bool NewPlayer { get; set; }

    [JsonPropertyName("startPlayer")]
    public bool StartPlayer { get; set; }

    [JsonPropertyName("playerRefId")]
    public int PlayerRefId { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("teamRole")]
    public string TeamRole { get; set; }

    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("seatOrder")]
    public int SeatOrder { get; set; }

    [JsonPropertyName("team")]
    public string Team { get; set; }
  }
}
