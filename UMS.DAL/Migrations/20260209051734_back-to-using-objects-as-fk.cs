using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class backtousingobjectsasfk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LessonIds",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "SectionIds",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "ExamIds",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LessonIds",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "StudentIds",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "TeacherIds",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "SectionIds",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "SectionIds",
                table: "Students",
                newName: "SectionId");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Exams",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LessonSection",
                columns: table => new
                {
                    LessonsId = table.Column<int>(type: "integer", nullable: false),
                    SectionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonSection", x => new { x.LessonsId, x.SectionsId });
                    table.ForeignKey(
                        name: "FK_LessonSection_Lessons_LessonsId",
                        column: x => x.LessonsId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonSection_Sections_SectionsId",
                        column: x => x.SectionsId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionTeacher",
                columns: table => new
                {
                    SectionsId = table.Column<int>(type: "integer", nullable: false),
                    TeachersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionTeacher", x => new { x.SectionsId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_SectionTeacher_Sections_SectionsId",
                        column: x => x.SectionsId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionTeacher_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_SectionId",
                table: "Students",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_TeacherId",
                table: "Lessons",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_StudentId",
                table: "Exams",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonSection_SectionsId",
                table: "LessonSection",
                column: "SectionsId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTeacher_TeachersId",
                table: "SectionTeacher",
                column: "TeachersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Students_StudentId",
                table: "Exams",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Teachers_TeacherId",
                table: "Lessons",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Sections_SectionId",
                table: "Students",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Students_StudentId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Teachers_TeacherId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Sections_SectionId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "LessonSection");

            migrationBuilder.DropTable(
                name: "SectionTeacher");

            migrationBuilder.DropIndex(
                name: "IX_Students_SectionId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_TeacherId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Exams_StudentId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Exams");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Students",
                newName: "SectionIds");

            migrationBuilder.AddColumn<List<int>>(
                name: "LessonIds",
                table: "Teachers",
                type: "integer[]",
                nullable: false);

            migrationBuilder.AddColumn<List<int>>(
                name: "SectionIds",
                table: "Teachers",
                type: "integer[]",
                nullable: false);

            migrationBuilder.AddColumn<List<int>>(
                name: "ExamIds",
                table: "Students",
                type: "integer[]",
                nullable: false);

            migrationBuilder.AddColumn<List<int>>(
                name: "LessonIds",
                table: "Sections",
                type: "integer[]",
                nullable: false);

            migrationBuilder.AddColumn<List<int>>(
                name: "StudentIds",
                table: "Sections",
                type: "integer[]",
                nullable: false);

            migrationBuilder.AddColumn<List<int>>(
                name: "TeacherIds",
                table: "Sections",
                type: "integer[]",
                nullable: false);

            migrationBuilder.AddColumn<List<int>>(
                name: "SectionIds",
                table: "Lessons",
                type: "integer[]",
                nullable: false);
        }
    }
}
