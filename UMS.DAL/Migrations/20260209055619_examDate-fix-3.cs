using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class examDatefix3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ExamDate",
                table: "Exams",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamDate",
                table: "Exams");
        }
    }
}
