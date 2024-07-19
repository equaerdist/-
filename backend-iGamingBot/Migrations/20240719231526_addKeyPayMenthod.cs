using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class addKeyPayMenthod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WinnerNote_AllUsers_WinnerId",
                table: "WinnerNote");

            migrationBuilder.DropForeignKey(
                name: "FK_WinnerNote_Raffles_RaffleId",
                table: "WinnerNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WinnerNote",
                table: "WinnerNote");

            migrationBuilder.DropColumn(
                name: "PayMethods",
                table: "AllUsers");

            migrationBuilder.RenameTable(
                name: "WinnerNote",
                newName: "WinnerNotes");

            migrationBuilder.RenameIndex(
                name: "IX_WinnerNote_RaffleId",
                table: "WinnerNotes",
                newName: "IX_WinnerNotes_RaffleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WinnerNotes",
                table: "WinnerNotes",
                columns: new[] { "WinnerId", "RaffleId" });

            migrationBuilder.CreateTable(
                name: "UserPayMethod",
                columns: table => new
                {
                    Platform = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPayMethod", x => new { x.UserId, x.Platform });
                    table.ForeignKey(
                        name: "FK_UserPayMethod_AllUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AllUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_WinnerNotes_AllUsers_WinnerId",
                table: "WinnerNotes",
                column: "WinnerId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WinnerNotes_Raffles_RaffleId",
                table: "WinnerNotes",
                column: "RaffleId",
                principalTable: "Raffles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WinnerNotes_AllUsers_WinnerId",
                table: "WinnerNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_WinnerNotes_Raffles_RaffleId",
                table: "WinnerNotes");

            migrationBuilder.DropTable(
                name: "UserPayMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WinnerNotes",
                table: "WinnerNotes");

            migrationBuilder.RenameTable(
                name: "WinnerNotes",
                newName: "WinnerNote");

            migrationBuilder.RenameIndex(
                name: "IX_WinnerNotes_RaffleId",
                table: "WinnerNote",
                newName: "IX_WinnerNote_RaffleId");

            migrationBuilder.AddColumn<string>(
                name: "PayMethods",
                table: "AllUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WinnerNote",
                table: "WinnerNote",
                columns: new[] { "WinnerId", "RaffleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_WinnerNote_AllUsers_WinnerId",
                table: "WinnerNote",
                column: "WinnerId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WinnerNote_Raffles_RaffleId",
                table: "WinnerNote",
                column: "RaffleId",
                principalTable: "Raffles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
