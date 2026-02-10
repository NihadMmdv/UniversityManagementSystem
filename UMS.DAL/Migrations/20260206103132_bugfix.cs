using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class bugfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "photoUrl",
                table: "Teachers",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "SectionsId",
                table: "Teachers",
                newName: "SectionIds");

            migrationBuilder.RenameColumn(
                name: "photoUrl",
                table: "Students",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Students",
                newName: "SectionIds");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Lessons",
                newName: "SectionIds");

            migrationBuilder.RenameColumn(
                name: "photoUrl",
                table: "Exams",
                newName: "PhotoUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Teachers",
                newName: "photoUrl");

            migrationBuilder.RenameColumn(
                name: "SectionIds",
                table: "Teachers",
                newName: "SectionsId");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Students",
                newName: "photoUrl");

            migrationBuilder.RenameColumn(
                name: "SectionIds",
                table: "Students",
                newName: "SectionId");

            migrationBuilder.RenameColumn(
                name: "SectionIds",
                table: "Lessons",
                newName: "SectionId");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Exams",
                newName: "photoUrl");
        }
    }
}
