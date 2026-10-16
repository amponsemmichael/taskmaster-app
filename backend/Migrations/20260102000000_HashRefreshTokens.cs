using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TaskMaster.Data;

#nullable disable

namespace TaskMaster.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260102000000_HashRefreshTokens")]
public partial class HashRefreshTokens : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DELETE FROM "RefreshTokens";""");

        migrationBuilder.DropIndex(
            name: "IX_RefreshTokens_Token",
            table: "RefreshTokens");

        migrationBuilder.DropColumn(
            name: "Token",
            table: "RefreshTokens");

        migrationBuilder.AddColumn<string>(
            name: "TokenHash",
            table: "RefreshTokens",
            type: "character varying(64)",
            maxLength: 64,
            nullable: false);

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_TokenHash",
            table: "RefreshTokens",
            column: "TokenHash",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DELETE FROM "RefreshTokens";""");

        migrationBuilder.DropIndex(
            name: "IX_RefreshTokens_TokenHash",
            table: "RefreshTokens");

        migrationBuilder.DropColumn(
            name: "TokenHash",
            table: "RefreshTokens");

        migrationBuilder.AddColumn<string>(
            name: "Token",
            table: "RefreshTokens",
            type: "character varying(256)",
            maxLength: 256,
            nullable: false);

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_Token",
            table: "RefreshTokens",
            column: "Token",
            unique: true);
    }
}
