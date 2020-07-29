using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BGStats.Bot.Models
{
  public class Location
  {
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("modificationDate")]
    public string ModificationDate { get; set; }
  }
}
