using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyKa.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceSecret",
                table: "UserSyncs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceSecret",
                table: "UserSyncs");
        }
    }
}
