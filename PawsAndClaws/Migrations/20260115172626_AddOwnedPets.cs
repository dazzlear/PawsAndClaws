using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawsAndClaws.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnedPets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "OwnedPets",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OwnedPets",
                newName: "CreatedAtUtc");
        }
    }
}
