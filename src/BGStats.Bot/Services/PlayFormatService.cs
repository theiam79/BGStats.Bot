﻿using BGStats.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGStats.Bot.Services
{
  public class PlayFormatService
  {
    public Discord.Embed FormatPlay(PlayFile playFile)
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

      if (DateTimeOffset.TryParse(play.EntryDate, out DateTimeOffset timestamp))
      {
        builder.WithTimestamp(timestamp);
      }

      if (play.UsesTeams)
      {
        var teams = playerScores.GroupBy(x => x.Team).OrderBy(g => g.Key);
        var i = 1;
        foreach (var team in teams)
        {
          var teamWon = team.Any(x => x.Winner);
          builder.AddField(x =>
          {
            x.Name = $"\r\nTeam {i++}{(teamWon ? " :trophy:" : "")}";
            x.Value = "----------------------------------------------------";
          });

          foreach (var playerScore in team)
          {
            var playerData = players.FirstOrDefault(x => x.Id == playerScore.PlayerRefId);
            if (playerData == null) continue;

            builder.AddField(x =>
            {
              x.Name = $"{playerData.Name}{(string.IsNullOrEmpty(playerScore.Score) ? "" : $" - {playerScore.Score}")}";
              x.Value = $"```{(string.IsNullOrEmpty(playerScore.TeamRole) ? "" : $"Role: {playerScore.TeamRole}\r\n")}BGG: {(string.IsNullOrEmpty(playerData.BggUsername) ? "Not set" : playerData.BggUsername)}```";
              //x.IsInline = true;
            });
          }
        }
      }
      else
      {
        foreach (var playerScore in playerScores)//.OrderBy(p => p.Winner ? 0 : 1))
        {
          var playerData = players.FirstOrDefault(x => x.Id == playerScore.PlayerRefId);
          if (playerData == null) continue;

          builder.AddField(x =>
          {
            x.Name = $"{playerData.Name}{(string.IsNullOrEmpty(playerScore.Score) ? "" : $" - {playerScore.Score}")}{(playerScore.Winner ? " :trophy:" : "")}";
            x.Value = $"```{(string.IsNullOrEmpty(playerScore.Role) ? "" : $"Role: {playerScore.Role}\r\n")}BGG: {(string.IsNullOrEmpty(playerData.BggUsername) ? "Not set" : playerData.BggUsername)}```";
          });
        }
      }

      return builder.Build();
    }

    string BuildDescription(Location location, Play play)
    {
      var sb = new StringBuilder()
        .Append(location.Name)
        .Append(" - ");

      if (play.Rounds != 0)
      {
        sb.Append($"{play.Rounds} Rounds - ");
      }

      if (play.DurationMin == 0)
      {
        sb.Append("Untimed");
      }
      else
      {
        var duration = TimeSpan.FromMinutes(play.DurationMin);
        sb.Append($"{duration.Hours} hours");

        if (play.DurationMin % 60 != 0)
        {
          sb.Append($" {duration.Minutes} minutes");
        }
      }

      if (play.Ignored)
      {
        sb.Append(" - *Ignored for stats*");
      }

      return sb.ToString();
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
