#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Falc.Identity.Admin.Infrastructure.EntityFramework.Migrations.IdentityServerConfiguration;

public partial class UpdateToIS61 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "CoordinateLifetimeWithUserSession",
            table: "Clients",
            type: "boolean",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CoordinateLifetimeWithUserSession",
            table: "Clients");
    }
}