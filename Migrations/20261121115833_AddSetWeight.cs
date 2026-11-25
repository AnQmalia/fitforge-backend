using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSetWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "setnumber",
                table: "workoutplanitems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "weight",
                table: "workoutplanitems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "setnumber",
                table: "workoutplanitems");

            migrationBuilder.DropColumn(
                name: "weight",
                table: "workoutplanitems");
        }
    }
}
