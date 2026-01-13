using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawsAndClaws.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentSteptoAdoptionApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStep",
                table: "AdoptionApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStep",
                table: "AdoptionApplications");
        }
    }
}
