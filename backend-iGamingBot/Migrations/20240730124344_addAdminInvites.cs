using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class addAdminInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Invites",
                table: "Invites");

            migrationBuilder.RenameTable(
                name: "Invites",
                newName: "StreamerInvites");

            migrationBuilder.RenameIndex(
                name: "IX_Invites_Name",
                table: "StreamerInvites",
                newName: "IX_StreamerInvites_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamerInvites",
                table: "StreamerInvites",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AdminInvites",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminInvites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminInvites_Code_Name",
                table: "AdminInvites",
                columns: new[] { "Code", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminInvites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamerInvites",
                table: "StreamerInvites");

            migrationBuilder.RenameTable(
                name: "StreamerInvites",
                newName: "Invites");

            migrationBuilder.RenameIndex(
                name: "IX_StreamerInvites_Name",
                table: "Invites",
                newName: "IX_Invites_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invites",
                table: "Invites",
                column: "Id");
        }
    }
}
