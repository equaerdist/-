using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class addPropParticipantToCtx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantNote_AllUsers_ParticipantId",
                table: "ParticipantNote");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantNote_Raffles_RaffleId",
                table: "ParticipantNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantNote",
                table: "ParticipantNote");

            migrationBuilder.RenameTable(
                name: "ParticipantNote",
                newName: "Participants");

            migrationBuilder.RenameIndex(
                name: "IX_ParticipantNote_RaffleId",
                table: "Participants",
                newName: "IX_Participants_RaffleId");

            migrationBuilder.RenameIndex(
                name: "IX_ParticipantNote_ParticipantId",
                table: "Participants",
                newName: "IX_Participants_ParticipantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participants",
                table: "Participants",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_AllUsers_ParticipantId",
                table: "Participants",
                column: "ParticipantId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Raffles_RaffleId",
                table: "Participants",
                column: "RaffleId",
                principalTable: "Raffles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_AllUsers_ParticipantId",
                table: "Participants");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Raffles_RaffleId",
                table: "Participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participants",
                table: "Participants");

            migrationBuilder.RenameTable(
                name: "Participants",
                newName: "ParticipantNote");

            migrationBuilder.RenameIndex(
                name: "IX_Participants_RaffleId",
                table: "ParticipantNote",
                newName: "IX_ParticipantNote_RaffleId");

            migrationBuilder.RenameIndex(
                name: "IX_Participants_ParticipantId",
                table: "ParticipantNote",
                newName: "IX_ParticipantNote_ParticipantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantNote",
                table: "ParticipantNote",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantNote_AllUsers_ParticipantId",
                table: "ParticipantNote",
                column: "ParticipantId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantNote_Raffles_RaffleId",
                table: "ParticipantNote",
                column: "RaffleId",
                principalTable: "Raffles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
