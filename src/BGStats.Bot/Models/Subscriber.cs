using System;
using System.Collections.Generic;
using System.Text;

namespace BGStats.Bot.Models
{
  public class Subscriber
  {
    
    public int SubscriberId { get; set; }
    public ulong DiscordId { get; set; }
    public string PlayerName { get; set; }
  }
}
