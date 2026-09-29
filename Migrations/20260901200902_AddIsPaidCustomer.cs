using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyKa.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPaidCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaidCustomer",
                table: "AppUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaidCustomer",
                table: "AppUsers");
        }
    }
}
