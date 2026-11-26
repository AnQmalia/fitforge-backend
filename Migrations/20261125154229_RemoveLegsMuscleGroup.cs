using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegsMuscleGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE exercises SET musclegroupid = 9 WHERE musclegroupid = 2;");

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "musclegroups",
                columns: new[] { "id", "name" },
                values: new object[] { 2, "Nogi" });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 2,
                column: "musclegroupid",
                value: 2);

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 9,
                column: "musclegroupid",
                value: 2);
        }
    }
}
