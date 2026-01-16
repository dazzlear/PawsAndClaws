using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawsAndClaws.Migrations
{
    /// <inheritdoc />
    public partial class FilterUniqueAdoptionApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdoptionApplications_UserId_PetId",
                table: "AdoptionApplications");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionApplications_UserId_PetId",
                table: "AdoptionApplications",
                columns: new[] { "UserId", "PetId" },
                unique: true,
                filter: "[Status] <> 'CANCELLED'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdoptionApplications_UserId_PetId",
                table: "AdoptionApplications");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionApplications_UserId_PetId",
                table: "AdoptionApplications",
                columns: new[] { "UserId", "PetId" },
                unique: true);
        }
    }
}
