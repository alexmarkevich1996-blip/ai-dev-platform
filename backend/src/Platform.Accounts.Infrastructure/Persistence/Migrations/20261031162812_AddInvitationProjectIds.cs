using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Accounts.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationProjectIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid[]>(
                name: "project_ids",
                schema: "accounts",
                table: "invitations",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "project_ids",
                schema: "accounts",
                table: "invitations");
        }
    }
}
