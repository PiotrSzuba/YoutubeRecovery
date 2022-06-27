using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeVideoRecovery.Migrations
{
    public partial class FixedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Users");
        }
    }
}
