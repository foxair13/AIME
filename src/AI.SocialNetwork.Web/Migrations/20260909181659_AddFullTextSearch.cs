using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI.SocialNetwork.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE users
                    ADD COLUMN search_vector tsvector
                    GENERATED ALWAYS AS (to_tsvector('russian', coalesce("Name", '') || ' ' || coalesce("Email", '') || ' ' || coalesce("City", ''))) STORED;
                CREATE INDEX idx_users_search_vector ON users USING gin (search_vector);

                ALTER TABLE skills
                    ADD COLUMN search_vector tsvector
                    GENERATED ALWAYS AS (to_tsvector('russian', coalesce("Name", '') || ' ' || coalesce("Description", ''))) STORED;
                CREATE INDEX idx_skills_search_vector ON skills USING gin (search_vector);

                ALTER TABLE posts
                    ADD COLUMN search_vector tsvector
                    GENERATED ALWAYS AS (to_tsvector('russian', coalesce("Body", ''))) STORED;
                CREATE INDEX idx_posts_search_vector ON posts USING gin (search_vector);

                ALTER TABLE groups
                    ADD COLUMN search_vector tsvector
                    GENERATED ALWAYS AS (to_tsvector('russian', coalesce("Name", ''))) STORED;
                CREATE INDEX idx_groups_search_vector ON groups USING gin (search_vector);

                ALTER TABLE hashtags
                    ADD COLUMN search_vector tsvector
                    GENERATED ALWAYS AS (to_tsvector('simple', coalesce("Tag", ''))) STORED;
                CREATE INDEX idx_hashtags_search_vector ON hashtags USING gin (search_vector);
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS idx_users_search_vector;
                ALTER TABLE users DROP COLUMN IF EXISTS search_vector;

                DROP INDEX IF EXISTS idx_skills_search_vector;
                ALTER TABLE skills DROP COLUMN IF EXISTS search_vector;

                DROP INDEX IF EXISTS idx_posts_search_vector;
                ALTER TABLE posts DROP COLUMN IF EXISTS search_vector;

                DROP INDEX IF EXISTS idx_groups_search_vector;
                ALTER TABLE groups DROP COLUMN IF EXISTS search_vector;

                DROP INDEX IF EXISTS idx_hashtags_search_vector;
                ALTER TABLE hashtags DROP COLUMN IF EXISTS search_vector;
                """
            );
        }
    }
}
