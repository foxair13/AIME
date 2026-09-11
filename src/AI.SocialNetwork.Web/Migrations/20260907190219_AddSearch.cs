using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AI.SocialNetwork.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hashtags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hashtags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "post_hashtags",
                columns: table => new
                {
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    HashtagId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_hashtags", x => new { x.PostId, x.HashtagId });
                    table.ForeignKey(
                        name: "FK_post_hashtags_hashtags_HashtagId",
                        column: x => x.HashtagId,
                        principalTable: "hashtags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_hashtags_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hashtags_Tag",
                table: "hashtags",
                column: "Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_post_hashtags_HashtagId",
                table: "post_hashtags",
                column: "HashtagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_hashtags");

            migrationBuilder.DropTable(
                name: "hashtags");
        }
    }
}
