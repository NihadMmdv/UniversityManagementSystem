using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Changed_nameclass_to_section : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.RenameColumn(
                name: "ClasseIds",
                table: "Teachers",
                newName: "SectionsId");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Students",
                newName: "SectionId");

            migrationBuilder.RenameColumn(
                name: "ClassIds",
                table: "Lessons",
                newName: "SectionId");

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StudentIds = table.Column<List<int>>(type: "integer[]", nullable: false),
                    TeacherIds = table.Column<List<int>>(type: "integer[]", nullable: false),
                    LessonIds = table.Column<List<int>>(type: "integer[]", nullable: false),
                    LastModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.RenameColumn(
                name: "SectionsId",
                table: "Teachers",
                newName: "ClasseIds");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Students",
                newName: "ClassId");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Lessons",
                newName: "ClassIds");

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeletedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LessonIds = table.Column<List<int>>(type: "integer[]", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StudentIds = table.Column<List<int>>(type: "integer[]", nullable: false),
                    TeacherIds = table.Column<List<int>>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                });
        }
    }
}
