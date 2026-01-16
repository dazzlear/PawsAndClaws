using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawsAndClaws.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnedPetsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sex",
                table: "OwnedPets");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OwnedPets",
                newName: "CreatedAtUtc");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "OwnedPets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "OwnedPets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "OwnedPets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OwnedPets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Species",
                table: "OwnedPets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedPets_UserId",
                table: "OwnedPets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnedPets_AspNetUsers_UserId",
                table: "OwnedPets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnedPets_AspNetUsers_UserId",
                table: "OwnedPets");

            migrationBuilder.DropIndex(
                name: "IX_OwnedPets_UserId",
                table: "OwnedPets");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "OwnedPets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OwnedPets");

            migrationBuilder.DropColumn(
                name: "Species",
                table: "OwnedPets");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "OwnedPets",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "OwnedPets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Age",
                table: "OwnedPets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Sex",
                table: "OwnedPets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
