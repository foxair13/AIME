using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.SocialNetwork.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModerationReason",
                table: "posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerationStatus",
                table: "posts",
                type: "text",
                nullable: false,
                defaultValue: "pending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModerationReason",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                table: "posts");
        }
    }
}
