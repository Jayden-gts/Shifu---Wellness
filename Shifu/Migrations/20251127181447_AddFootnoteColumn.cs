using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shifu.Migrations
{
    /// <inheritdoc />
    public partial class AddFootnoteColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "PasswordConfirm",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordConfirm",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Bio", "CurrentGoal", "Email", "FirstName", "GoalsCompleted", "IsAdmin", "IsAvailable", "IsMentor", "IsMentorApplicant", "JournalStreak", "LastName", "MentorStatus", "Password", "PhoneNumber", "Qualifications", "Specialities" },
                values: new object[] { 1, null, null, "admin@gmail.com", "Admin", null, true, false, false, false, null, "User", null, "AdminPass123", "000-000-0000", null, null });
        }
    }
}
