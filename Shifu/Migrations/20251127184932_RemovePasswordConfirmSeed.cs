using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shifu.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordConfirmSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        INSERT INTO Users (Id, FirstName, LastName, Email, Password, PhoneNumber, IsAdmin)
        VALUES (1, 'Admin', 'User', 'admin@gmail.com', 'AdminPass123', '000-000-0000', 1);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Users WHERE Id = 1;");
        }

    }
}
