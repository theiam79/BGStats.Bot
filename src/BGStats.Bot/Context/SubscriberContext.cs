using BGStats.Bot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace BGStats.Bot.Context
{
  public class SubscriberContext : DbContext
  {
    public SubscriberContext(DbContextOptions<SubscriberContext> options) : base(options) { }

    public DbSet<Subscriber> Subscribers { get; set; }


  }

  public class SubscriberContextFactory : IDesignTimeDbContextFactory<SubscriberContext>
  {
    public SubscriberContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<SubscriberContext>().UseSqlite("Data Source = TestSubscribers.db");
      return new SubscriberContext(optionsBuilder.Options);
    }
  }
}
