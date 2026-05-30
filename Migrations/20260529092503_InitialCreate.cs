using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyKa.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: false),
                    Target = table.Column<decimal>(type: "TEXT", nullable: false),
                    Saved = table.Column<decimal>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Billing = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    NextDate = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Desc = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Recurring = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecFreq = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Plan = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlans", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Goals",
                columns: new[] { "Id", "Color", "Icon", "Saved", "Target", "Title" },
                values: new object[,]
                {
                    { 1, "#4CAF82", "🌴", 2500m, 5000m, "შვებულება" },
                    { 2, "#4A90D9", "📱", 1200m, 2000m, "ახალი ტელეფონი" },
                    { 3, "#E07B54", "🛡️", 3000m, 5000m, "საგანგებო ფონდი" }
                });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "Active", "Billing", "Category", "Color", "Icon", "Name", "NextDate", "Price" },
                values: new object[,]
                {
                    { 1, true, "monthly", "მუსიკა", "#1DB954", "🎵", "Spotify", "2024-06-15", 8.99m },
                    { 2, true, "monthly", "ვიდეო", "#FF0000", "▶️", "YouTube Premium", "2024-06-10", 13.99m },
                    { 3, true, "monthly", "ვიდეო", "#E50914", "🎬", "Netflix", "2024-06-24", 15.99m },
                    { 4, true, "monthly", "ღრუბელი", "#4A90D9", "☁️", "iCloud+", "2024-06-05", 2.99m },
                    { 5, false, "monthly", "შემოქმედება", "#FF0000", "🎨", "Adobe Creative", "2024-06-18", 29.99m }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "Category", "Date", "Desc", "RecFreq", "Recurring", "Time", "Type" },
                values: new object[,]
                {
                    { 1, -85m, "food", "2024-05-25", "სუპერმარკეტი", null, false, "14:30", "expense" },
                    { 2, -40m, "transport", "2024-05-25", "ბენზინი", null, false, "12:10", "expense" },
                    { 3, -60m, "entertainment", "2024-05-24", "რესტორანი", null, false, "20:45", "expense" },
                    { 4, -15m, "utilities", "2024-05-24", "Netflix", "monthly", true, "09:15", "expense" },
                    { 5, 2800m, "other", "2024-05-01", "ხელფასი", "monthly", true, "10:00", "income" },
                    { 6, -120m, "food", "2024-05-22", "ბაზრობა", null, false, "11:00", "expense" },
                    { 7, -35m, "health", "2024-05-21", "აფთიაქი", null, false, "16:20", "expense" },
                    { 8, -25m, "utilities", "2024-05-20", "მობილური", "monthly", true, "09:00", "expense" },
                    { 9, -45m, "education", "2024-05-18", "ონლაინ კურსი", null, false, "14:00", "expense" },
                    { 10, -20m, "transport", "2024-05-17", "მეტრო ბარათი", null, false, "08:30", "expense" },
                    { 11, -8m, "utilities", "2024-05-15", "Spotify", "monthly", true, "09:00", "expense" },
                    { 12, 450m, "other", "2024-05-10", "ფრილანს შემოსავალი", null, false, "15:30", "income" }
                });

            migrationBuilder.InsertData(
                table: "UserPlans",
                columns: new[] { "Id", "Plan", "UpdatedAt" },
                values: new object[] { 1, "free", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserPlans");
        }
    }
}
