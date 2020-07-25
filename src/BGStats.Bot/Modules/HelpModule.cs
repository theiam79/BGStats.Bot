using BGStats.Bot.Services;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BGStats.Bot.Modules
{
  public class HelpModule : ModuleBase<SocketCommandContext>
  {
    private readonly Helper _help;
    private readonly InteractiveService _interactiveService;
    private readonly IHost _host;

    public HelpModule(Helper help, InteractiveService interactiveService, IHost host)
    {
      _help = help;
      _interactiveService = interactiveService;
      _host = host;
    }

    [Command("help")]
    [Summary("Displays list of available commands")]
    [RequireContext(ContextType.Guild)]
    public async Task Help()
    {
      await _interactiveService.ReplyAndDeleteAsync(Context, "Sending you list of my commands now!", timeout: TimeSpan.FromSeconds(10));
      await _help.HelpAsync(Context);
    }

    [Command("help")]
    [Summary("Shows information about a command")]
    public Task Help([Remainder] string command) => _help.HelpAsync(Context, command);

    [Command("info")]
    [Alias("stats")]
    [Summary("Provides information about the bot")]
    public async Task Info()
    {
      await ReplyAsync(
          $"{Format.Bold("Info")}\n" +
          "- Developed by TheIAm79#0951\n" +
          "- Github: `URL_HERE` \n" +
          $"- Library: Discord.Net ({DiscordConfig.Version})\n" +
          $"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}\n" +
          $"- Uptime: {GetUptime()}\n\n" +

          $"{Format.Bold("Stats")}\n" +
          $"- Heap Size: {GetHeapSize()} MB\n" +
          $"- Guilds: {Context.Client.Guilds.Count}\n" +
          $"- Channels: {Context.Client.Guilds.Sum(g => g.Channels.Count)}\n" +
          $"- Users: {Context.Client.Guilds.Sum(g => g.MemberCount)}"
      );
      static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
      static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString(CultureInfo.CurrentCulture);
    }

    [Command("shutdown")]
    [RequireOwner]
    public async Task Shutdown()
    {
      await ReplyAsync("Goodbye!");
      _ = _host.StopAsync();
    }
  }
}
