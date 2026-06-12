using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Istapio.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SomeChangesOnAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpCountToday",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "OtpCountToday",
                table: "Users",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
