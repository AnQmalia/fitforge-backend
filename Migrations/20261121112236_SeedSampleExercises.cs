using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "musclegroups",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Klatka piersiowa" },
                    { 2, "Nogi" },
                    { 3, "Plecy" },
                    { 4, "Barki" },
                    { 5, "Biceps" },
                    { 6, "Triceps" },
                    { 7, "Brzuch" }
                });

            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "id", "description", "equipmentneeded", "musclegroupid", "name" },
                values: new object[,]
                {
                    { 1, "Połóż się na ławce, ściągnij łopatki i wypchnij sztangę nad klatkę, kontrolując ruch w dół.", "Sztanga, ławka", 1, "Wyciskanie sztangi na ławce poziomej" },
                    { 2, "Sztanga oparta na górnej części pleców. Zejdź do kąta prostego w kolanach i wstań, trzymając pięty na podłodze.", "Sztanga, stojaki", 2, "Przysiady ze sztangą na plecach" },
                    { 3, "Chwyt na szerokość barków, plecy proste. Unieś sztangę, prostując biodra i kolana jednocześnie.", "Sztanga", 3, "Martwy ciąg klasyczny" },
                    { 4, "Zwis na drążku, przyciągnij klatkę do drążka, ściągając łopatki. Opuszczaj się do pełnego zwisu.", "Drążek", 3, "Podciąganie na drążku nachwytem" },
                    { 5, "Stojąc, wypchnij sztangę znad obojczyków nad głowę i opuść ją kontrolowanie do brody.", "Sztanga", 4, "Wyciskanie żołnierskie" },
                    { 6, "Łokcie przy tułowiu, ugnij przedramiona i powoli opuść hantle. Nie bujaj ciałem.", "Hantle", 5, "Uginanie przedramion z hantlami" },
                    { 7, "Leżąc na ławce, opuść sztangę łamaną do czoła, zginając tylko łokcie, a następnie wyprostuj ramiona.", "Sztanga łamana, ławka", 6, "Wyciskanie francuskie" },
                    { 8, "Tułów prawie równolegle do podłogi, przyciągnij sztangę do brzucha i opuść ją bez zaokrąglania pleców.", "Sztanga", 3, "Wiosłowanie sztangą w opadzie" },
                    { 9, "Tylna stopa na ławce, zejdź w dół na nodze wykrocznej i wróć do pozycji startowej.", "Hantle, ławka", 2, "Przysiad bułgarski" },
                    { 10, "Oprzyj się na przedramionach, ciało w jednej linii. Napnij brzuch i utrzymuj pozycję bez opadania bioder.", "Mata", 7, "Deska" }
                });

            migrationBuilder.Sql("""
                SELECT setval(pg_get_serial_sequence('musclegroups', 'id'), (SELECT MAX(id) FROM musclegroups));
                SELECT setval(pg_get_serial_sequence('exercises', 'id'), (SELECT MAX(id) FROM exercises));
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "musclegroups",
                keyColumn: "id",
                keyValue: 7);
        }
    }
}
