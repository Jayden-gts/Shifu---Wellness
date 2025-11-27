using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shifu.Migrations
{
    /// <inheritdoc />
    public partial class AddMentorStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MentorStatus",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MentorStatus",
                table: "Users");
        }
    }
}
