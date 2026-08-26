using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyKa.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSyncs",
                columns: table => new
                {
                    DeviceId = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionsJson = table.Column<string>(type: "TEXT", nullable: false),
                    GoalsJson = table.Column<string>(type: "TEXT", nullable: false),
                    SubsJson = table.Column<string>(type: "TEXT", nullable: false),
                    BudgetsJson = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSyncs", x => x.DeviceId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSyncs");
        }
    }
}
