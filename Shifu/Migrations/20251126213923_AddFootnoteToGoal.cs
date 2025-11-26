using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shifu.Migrations
{
    /// <inheritdoc />
    public partial class AddFootnoteToGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Footnote",
                table: "JournalEntries");

            migrationBuilder.AddColumn<string>(
                name: "Footnote",
                table: "Goals",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Footnote",
                table: "Goals");

            migrationBuilder.AddColumn<string>(
                name: "Footnote",
                table: "JournalEntries",
                type: "TEXT",
                nullable: true);
        }
    }
}
