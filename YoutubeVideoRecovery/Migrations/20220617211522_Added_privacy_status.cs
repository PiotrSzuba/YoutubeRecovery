using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeVideoRecovery.Migrations
{
    public partial class Added_privacy_status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrivacyStatus",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivacyStatus",
                table: "Videos");
        }
    }
}
