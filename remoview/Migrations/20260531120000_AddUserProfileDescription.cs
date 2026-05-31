using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace remoview.Migrations
{
    /// <inheritdoc />
    [Migration("20260531120000_AddUserProfileDescription")]
    public partial class AddUserProfileDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Users\" ADD COLUMN IF NOT EXISTS \"ProfileDescription\" text;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileDescription",
                table: "Users");
        }
    }
}
