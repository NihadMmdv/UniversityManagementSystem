using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class schedulesupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleTeacher",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "integer", nullable: false),
                    TeachersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTeacher", x => new { x.ScheduleId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_ScheduleTeacher_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleTeacher_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTeacher_TeachersId",
                table: "ScheduleTeacher",
                column: "TeachersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleTeacher");
        }
    }
}
