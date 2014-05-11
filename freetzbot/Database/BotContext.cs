﻿using System.Data.Entity;

namespace FritzBot.Database
{
    public class BotContext : DbContext
    {
        public DbSet<AliasEntry> AliasEntries { get; set; }
        public DbSet<Box> Boxes { get; set; }
        public DbSet<BoxEntry> BoxEntries { get; set; }
        public DbSet<Nickname> Nicknames { get; set; }
        public DbSet<NotificationHistory> NotificationHistories { get; set; }
        public DbSet<ReminderEntry> ReminderEntries { get; set; }
        public DbSet<SeenEntry> SeenEntries { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserKeyValueEntry> UserKeyValueEntries { get; set; }
        public DbSet<WitzEntry> WitzEntries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(x => x.Names)
                .WithRequired(x => x.User);
        }
    }
}