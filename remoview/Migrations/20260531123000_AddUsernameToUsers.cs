using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace remoview.Migrations
{
    /// <inheritdoc />
    [Migration("20260531123000_AddUsernameToUsers")]
    public partial class AddUsernameToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Username" text;

                UPDATE "Users"
                SET "Username" = lower(split_part("Email", '@', 1))
                WHERE "Username" IS NULL OR trim("Username") = '';

                ALTER TABLE "Users" ALTER COLUMN "Username" SET NOT NULL;

                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Username" ON "Users" ("Username");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS "IX_Users_Username";
                ALTER TABLE "Users" DROP COLUMN IF EXISTS "Username";
                """);
        }
    }
}
