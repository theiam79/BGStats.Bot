using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BGStats.Bot.Models
{
  public class UserInfo
  {
    [JsonPropertyName("meRefId")]
    public int MeRefId { get; set; }
  }
}
