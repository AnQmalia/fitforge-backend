using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraMuscleGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "musclegroups",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 8, "Cardio" },
                    { 9, "Czworogłowe uda" },
                    { 10, "Dwugłowe uda" },
                    { 11, "Biodra" },
                    { 12, "Łydki" },
                    { 13, "Przedramiona" }
                });

            migrationBuilder.Sql(
                "SELECT setval('musclegroups_id_seq', (SELECT MAX(id) FROM musclegroups));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 13);
        }
    }
}
