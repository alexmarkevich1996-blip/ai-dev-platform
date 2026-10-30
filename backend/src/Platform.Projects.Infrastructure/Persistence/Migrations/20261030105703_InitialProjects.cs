using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Projects.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "projects");

            migrationBuilder.CreateTable(
                name: "projects",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "instructions",
                schema: "projects",
                columns: table => new
                {
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    markdown = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instructions", x => x.project_id);
                    table.ForeignKey(
                        name: "fk_instructions_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_memberships",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_memberships", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_memberships_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "repository_records",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    purpose = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    added_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_repository_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_repository_records_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_project_memberships_employee_id_project_id",
                schema: "projects",
                table: "project_memberships",
                columns: new[] { "employee_id", "project_id" });

            migrationBuilder.CreateIndex(
                name: "ix_project_memberships_project_id",
                schema: "projects",
                table: "project_memberships",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_repository_records_project_id",
                schema: "projects",
                table: "repository_records",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "instructions",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "project_memberships",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "repository_records",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "projects",
                schema: "projects");
        }
    }
}
