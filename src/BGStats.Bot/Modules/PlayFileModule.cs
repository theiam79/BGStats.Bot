using BGStats.Bot.Models;
using BGStats.Bot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BGStats.Bot.Modules
{
  public class PlayFileModule : ModuleBase<SocketCommandContext>
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PlayFormatService _playFormatService;
    private readonly PostingService _postingService;

    public PlayFileModule(IHttpClientFactory httpClientFactory, PlayFormatService playFormatService, PostingService postingService)
    {
      _httpClientFactory = httpClientFactory;
      _playFormatService = playFormatService;
      _postingService = postingService;
    }

    [Command("sharePlayFile")]
    [Alias("You", "You can import this file using Board Game Stats.")]
    [RequirePlayfile]
    public async Task SharePlayFile()
    {
      var playFile = Context.Message.Attachments.FirstOrDefault(x => x.Filename.EndsWith(".bgsplay"));
      var playFileContents = await _httpClientFactory.CreateClient().GetStreamAsync(playFile.Url);
      var play = await JsonSerializer.DeserializeAsync<PlayFile>(playFileContents);

      await _postingService.PostAsync(_playFormatService.FormatPlay(play));
    }
  }

  public class RequirePlayfileAttribute : PreconditionAttribute
  {
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
      var playfile = context.Message.Attachments.FirstOrDefault(x => x.Filename.EndsWith(".bgsplay"));

      if (playfile == null)
      {
        return Task.FromResult(PreconditionResult.FromError("You must attach a file with the .bgsplay extension to use this"));
      }

      return Task.FromResult(PreconditionResult.FromSuccess());
    }
  }
}
