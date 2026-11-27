using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSecondaryMuscleGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercisesecondarymusclegroups",
                columns: table => new
                {
                    exerciseid = table.Column<int>(type: "integer", nullable: false),
                    musclegroupid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercisesecondarymusclegroups", x => new { x.exerciseid, x.musclegroupid });
                    table.ForeignKey(
                        name: "fk_exercisesecondarymusclegroups_exercises_exerciseid",
                        column: x => x.exerciseid,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_exercisesecondarymusclegroups_musclegroups_musclegroupid",
                        column: x => x.musclegroupid,
                        principalTable: "musclegroups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "exercisesecondarymusclegroups",
                columns: new[] { "exerciseid", "musclegroupid" },
                values: new object[,]
                {
                    { 1, 4 },
                    { 1, 6 },
                    { 2, 7 },
                    { 2, 10 },
                    { 2, 11 },
                    { 3, 10 },
                    { 3, 11 },
                    { 3, 13 },
                    { 4, 5 },
                    { 4, 13 },
                    { 5, 6 },
                    { 5, 7 },
                    { 6, 13 },
                    { 8, 5 },
                    { 8, 13 },
                    { 9, 10 },
                    { 9, 11 },
                    { 10, 4 },
                    { 10, 11 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_exercisesecondarymusclegroups_musclegroupid",
                table: "exercisesecondarymusclegroups",
                column: "musclegroupid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercisesecondarymusclegroups");
        }
    }
}
