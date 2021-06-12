using BGStats.Bot.Models;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGStats.Bot.Services
{
  public class PlayFormatService
  {
    public Discord.Embed FormatPlay(PlayFile playFile, string imageUrl = null)
    {
      var game = playFile.Games.FirstOrDefault();
      var location = playFile.Locations.FirstOrDefault();
      var play = playFile.Plays.FirstOrDefault();
      var playerScores = play?.PlayerScores;
      var players = playFile.Players;

      var builder = new Discord.EmbedBuilder()
        .WithTitle(game?.Name ?? "None")
        .WithDescription(BuildDescription(location, play))
        .WithFooter(BuildFooter(game))
        .WithThumbnailUrl(game.UrlThumb)
        .WithColor(Discord.Color.Green);

      if (imageUrl != null) { builder.WithImageUrl(imageUrl); }

      if (DateTimeOffset.TryParse(play.EntryDate, out DateTimeOffset timestamp))
      {
        builder.WithTimestamp(timestamp.ToLocalTime());
      }

      if (play.UsesTeams)
      {
        var teams = playerScores.GroupBy(x => x.Team).OrderBy(g => g.Key);
        var i = 1;
        foreach (var team in teams)
        {
          var teamWon = team.Any(x => x.Winner);
          var teamName = $"\r\nTeam {i++}{(teamWon ? " :trophy:" : "")}";

          var sb = new StringBuilder();

          foreach (var playerScore in team)
          {
            var playerData = players.FirstOrDefault(x => x.Id == playerScore.PlayerRefId);
            if (playerData == null) continue;

            sb.AppendLine($"{playerData.Name}{(string.IsNullOrEmpty(playerScore.Score) ? "" : $" - {playerScore.Score}")}");
            sb.AppendLine($"```{(string.IsNullOrEmpty(playerScore.TeamRole) ? "" : $"Role: {playerScore.TeamRole}\r\n")}BGG: {(string.IsNullOrEmpty(playerData.BggUsername) ? "Not set" : playerData.BggUsername)}```");
          }

          builder.AddField(x =>
          {
            x.Name = teamName;
            x.Value = sb.ToString();
          });
        }
      }
      else
      {
        var sb = new StringBuilder();
        foreach (var playerScore in playerScores)
        {
          var playerData = players.FirstOrDefault(x => x.Id == playerScore.PlayerRefId);
          if (playerData == null) continue;

          sb.AppendLine($"{playerData.Name}{(string.IsNullOrEmpty(playerScore.Score) ? "" : $" - {new Expression(playerScore.Score, EvaluateOptions.None).Evaluate()}")}{(playerScore.Winner ? " :trophy:" : "")}");
          sb.AppendLine($"```{(string.IsNullOrEmpty(playerScore.Role) ? "" : $"Role: {playerScore.Role}\r\n")}BGG: {(string.IsNullOrEmpty(playerData.BggUsername) ? "Not set" : playerData.BggUsername)}```");
        }

        builder.AddField(x =>
        {
          x.Name = "Players";
          x.Value = sb.ToString();
        });
      }

      if (!string.IsNullOrEmpty(play.Board))
      {
        builder.AddField(x =>
        {
          x.Name = "Board, Variant or Expansions";
          x.Value = play.Board;
        });
      }

      return builder.Build();
    }

    string BuildDescription(Location location, Play play)
    {
      var descriptionItems = new List<string>();

      descriptionItems.Add(location.Name);
      if (play.Rounds != 0) { descriptionItems.Add($"{play.Rounds} Rounds"); }
      
      if (play.DurationMin == 0)
      {
        descriptionItems.Add("Untimed");
      }
      else
      {
        var duration = TimeSpan.FromMinutes(play.DurationMin);
        var timeDescription = duration.Hours > 0 ? $"{duration.Hours} hours" : "";
        timeDescription += duration.Minutes % 60 != 0 ? $"{duration.Minutes} minutes" : "";
        descriptionItems.Add(timeDescription);
      }

      if (play.Ignored) { descriptionItems.Add(Discord.Format.Bold("Ignored for stats")); }

      return new StringBuilder().AppendJoin(" - ", descriptionItems).ToString();
    }

    string BuildFooter(Game game)
    {
      if (game == null) return "";

      var sb = new StringBuilder()
        .Append($"{game.BggYear} {game.Designers}");

      if (game.Cooperative)
      {
        sb.Append(" | Co-op | ");
      }
      else
      {
        sb.Append(" | PvP ");
        if (game.UsesTeams)
        {
          sb.Append("(Teams) ");
        }
        sb.Append("| ");
      }

      sb.Append($"{game.MinPlayerCount}-{game.MaxPlayerCount} Players");

      return sb.ToString();
    }
  }
}
