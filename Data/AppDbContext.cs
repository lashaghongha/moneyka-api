using Microsoft.EntityFrameworkCore;
using MoneyKa.Api.Models;

namespace MoneyKa.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<UserPlan> UserPlans => Set<UserPlan>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<PushSub> PushSubs => Set<PushSub>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed transactions
        modelBuilder.Entity<Transaction>().HasData(
            new Transaction { Id = 1, Category = "food",          Desc = "სუპერმარკეტი",        Amount = -85,   Date = "2024-05-25", Time = "14:30", Type = "expense", Recurring = false },
            new Transaction { Id = 2, Category = "transport",     Desc = "ბენზინი",              Amount = -40,   Date = "2024-05-25", Time = "12:10", Type = "expense", Recurring = false },
            new Transaction { Id = 3, Category = "entertainment", Desc = "რესტორანი",            Amount = -60,   Date = "2024-05-24", Time = "20:45", Type = "expense", Recurring = false },
            new Transaction { Id = 4, Category = "utilities",     Desc = "Netflix",              Amount = -15,   Date = "2024-05-24", Time = "09:15", Type = "expense", Recurring = true, RecFreq = "monthly" },
            new Transaction { Id = 5, Category = "other",         Desc = "ხელფასი",              Amount = 2800,  Date = "2024-05-01", Time = "10:00", Type = "income",  Recurring = true, RecFreq = "monthly" },
            new Transaction { Id = 6, Category = "food",          Desc = "ბაზრობა",             Amount = -120,  Date = "2024-05-22", Time = "11:00", Type = "expense", Recurring = false },
            new Transaction { Id = 7, Category = "health",        Desc = "აფთიაქი",             Amount = -35,   Date = "2024-05-21", Time = "16:20", Type = "expense", Recurring = false },
            new Transaction { Id = 8, Category = "utilities",     Desc = "მობილური",            Amount = -25,   Date = "2024-05-20", Time = "09:00", Type = "expense", Recurring = true, RecFreq = "monthly" },
            new Transaction { Id = 9, Category = "education",     Desc = "ონლაინ კურსი",        Amount = -45,   Date = "2024-05-18", Time = "14:00", Type = "expense", Recurring = false },
            new Transaction { Id = 10, Category = "transport",    Desc = "მეტრო ბარათი",        Amount = -20,   Date = "2024-05-17", Time = "08:30", Type = "expense", Recurring = false },
            new Transaction { Id = 11, Category = "utilities",    Desc = "Spotify",              Amount = -8,    Date = "2024-05-15", Time = "09:00", Type = "expense", Recurring = true, RecFreq = "monthly" },
            new Transaction { Id = 12, Category = "other",        Desc = "ფრილანს შემოსავალი",  Amount = 450,   Date = "2024-05-10", Time = "15:30", Type = "income",  Recurring = false }
        );

        // Seed goals
        modelBuilder.Entity<Goal>().HasData(
            new Goal { Id = 1, Title = "შვებულება",       Icon = "🌴", Target = 5000, Saved = 2500, Color = "#4CAF82" },
            new Goal { Id = 2, Title = "ახალი ტელეფონი", Icon = "📱", Target = 2000, Saved = 1200, Color = "#4A90D9" },
            new Goal { Id = 3, Title = "საგანგებო ფონდი", Icon = "🛡️", Target = 5000, Saved = 3000, Color = "#E07B54" }
        );

        // Seed subscriptions
        modelBuilder.Entity<Subscription>().HasData(
            new Subscription { Id = 1, Name = "Spotify",          Icon = "🎵", Color = "#1DB954", Price = 8.99m,  Billing = "monthly", Category = "მუსიკა",        NextDate = "2024-06-15", Active = true },
            new Subscription { Id = 2, Name = "YouTube Premium",  Icon = "▶️", Color = "#FF0000", Price = 13.99m, Billing = "monthly", Category = "ვიდეო",         NextDate = "2024-06-10", Active = true },
            new Subscription { Id = 3, Name = "Netflix",          Icon = "🎬", Color = "#E50914", Price = 15.99m, Billing = "monthly", Category = "ვიდეო",         NextDate = "2024-06-24", Active = true },
            new Subscription { Id = 4, Name = "iCloud+",          Icon = "☁️", Color = "#4A90D9", Price = 2.99m,  Billing = "monthly", Category = "ღრუბელი",      NextDate = "2024-06-05", Active = true },
            new Subscription { Id = 5, Name = "Adobe Creative",   Icon = "🎨", Color = "#FF0000", Price = 29.99m, Billing = "monthly", Category = "შემოქმედება",   NextDate = "2024-06-18", Active = false }
        );

        // Seed default user plan
        modelBuilder.Entity<UserPlan>().HasData(
            new UserPlan { Id = 1, Plan = "free", UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
