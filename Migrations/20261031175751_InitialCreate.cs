using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitForge.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "musclegroups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_musclegroups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    fitnessgoal = table.Column<string>(type: "text", nullable: false),
                    experiencelevel = table.Column<string>(type: "text", nullable: false),
                    equipmentavailable = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    musclegroupid = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    equipmentneeded = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercises", x => x.id);
                    table.ForeignKey(
                        name: "fk_exercises_musclegroups_musclegroupid",
                        column: x => x.musclegroupid,
                        principalTable: "musclegroups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workoutplans",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workoutplans", x => x.id);
                    table.ForeignKey(
                        name: "fk_workoutplans_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workoutplanitems",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workoutplanid = table.Column<int>(type: "integer", nullable: false),
                    exerciseid = table.Column<int>(type: "integer", nullable: false),
                    dayofweek = table.Column<string>(type: "text", nullable: false),
                    sets = table.Column<int>(type: "integer", nullable: false),
                    targetreps = table.Column<int>(type: "integer", nullable: false),
                    exerciseorder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workoutplanitems", x => x.id);
                    table.ForeignKey(
                        name: "fk_workoutplanitems_exercises_exerciseid",
                        column: x => x.exerciseid,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workoutplanitems_workoutplans_workoutplanid",
                        column: x => x.workoutplanid,
                        principalTable: "workoutplans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_exercises_musclegroupid",
                table: "exercises",
                column: "musclegroupid");

            migrationBuilder.CreateIndex(
                name: "ix_workoutplanitems_exerciseid",
                table: "workoutplanitems",
                column: "exerciseid");

            migrationBuilder.CreateIndex(
                name: "ix_workoutplanitems_workoutplanid",
                table: "workoutplanitems",
                column: "workoutplanid");

            migrationBuilder.CreateIndex(
                name: "ix_workoutplans_userid",
                table: "workoutplans",
                column: "userid");

            migrationBuilder.Sql("""
                CREATE OR REPLACE PROCEDURE sp_insert_workout_plan(
                    IN p_user_id integer,
                    IN p_name varchar,
                    INOUT p_plan_id integer DEFAULT NULL)
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    INSERT INTO workoutplans (userid, name, createdat, isactive)
                    VALUES (p_user_id, p_name, now(), true)
                    RETURNING id INTO p_plan_id;
                END;
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_insert_workout_plan(integer, varchar, integer);");

            migrationBuilder.DropTable(
                name: "workoutplanitems");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "workoutplans");

            migrationBuilder.DropTable(
                name: "musclegroups");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
