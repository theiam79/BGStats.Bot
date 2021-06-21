using BGStats.Bot.Converters;
using BGStats.Bot.Models;
using BGStats.Bot.Services;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BGStats.Bot.Modules
{
  public class PlayFileModule : InteractiveBase<SocketCommandContext>
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PlayFormatService _playFormatService;
    private readonly PostingService _postingService;
    private readonly INotificationService _notificationService;

    public PlayFileModule(IHttpClientFactory httpClientFactory, PlayFormatService playFormatService, PostingService postingService, INotificationService notificationService)
    {
      _httpClientFactory = httpClientFactory;
      _playFormatService = playFormatService;
      _postingService = postingService;
      _notificationService = notificationService;
    }

    [Command("sharePlayFile")]
    [Alias("You", "You can import this file using Board Game Stats.")]
    [RequirePlayfile]
    public async Task SharePlayFile()
    {

      await Context.Channel.SendMessageAsync("Play file recieved. Respond with a picture if you would like to include one.");

      var imageMessageTask = NextMessageAsync(true, true, TimeSpan.FromSeconds(20));



      var playFile = Context.Message.Attachments.FirstOrDefault(x => x.Filename.EndsWith(".bgsplay"));
      var playFileContents = await _httpClientFactory.CreateClient().GetStreamAsync(playFile.Url);

      var contentStream = new MemoryStream();
      await playFileContents.CopyToAsync(contentStream);
      contentStream.Seek(0, SeekOrigin.Begin);

      var serializerOptions = new JsonSerializerOptions();
      serializerOptions.Converters.Add(new AutoIntToBoolConverter());

      var play = await JsonSerializer.DeserializeAsync<PlayFile>(contentStream, serializerOptions);


      var imageAttachment = (await imageMessageTask)?.Attachments.FirstOrDefault(x => PhotoExtension(x.Filename));

      await _postingService.PostAsync(_playFormatService.FormatPlay(play, imageAttachment?.Url));
      await _notificationService.Notify(playFile.Filename, contentStream, play, Context.User.Id);
    }

    private readonly List<string> validImageExtensions = new List<string>
    {
      ".jpg",
      ".jpeg",
      ".png",
      //".gif", // Do I trust people with these?
      //".gifv"
    };

    private bool PhotoExtension(string fileName)
    {
      return validImageExtensions.Any(vie => fileName.EndsWith(vie, StringComparison.InvariantCultureIgnoreCase));
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
