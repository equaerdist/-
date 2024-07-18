using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class rootUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefaultUserRaffle_DefaultUser_ParticipantsId",
                table: "DefaultUserRaffle");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultUserRaffle1_DefaultUser_WinnersId",
                table: "DefaultUserRaffle1");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultUserStreamer_DefaultUser_AdminsId",
                table: "DefaultUserStreamer");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultUserStreamer_DefaultUser_NegotiableId",
                table: "DefaultUserStreamer");

            migrationBuilder.DropForeignKey(
                name: "FK_Raffles_DefaultUser_CreatorId",
                table: "Raffles");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_DefaultUser_StreamerId",
                table: "Subscribers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_DefaultUser_UserId",
                table: "Subscribers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DefaultUser",
                table: "DefaultUser");

            migrationBuilder.RenameTable(
                name: "DefaultUser",
                newName: "AllUsers");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultUser_TgId",
                table: "AllUsers",
                newName: "IX_AllUsers_TgId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultUser_Name",
                table: "AllUsers",
                newName: "IX_AllUsers_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllUsers",
                table: "AllUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultUserRaffle_AllUsers_ParticipantsId",
                table: "DefaultUserRaffle",
                column: "ParticipantsId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultUserRaffle1_AllUsers_WinnersId",
                table: "DefaultUserRaffle1",
                column: "WinnersId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultUserStreamer_AllUsers_AdminsId",
                table: "DefaultUserStreamer",
                column: "AdminsId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultUserStreamer_AllUsers_NegotiableId",
                table: "DefaultUserStreamer",
                column: "NegotiableId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Raffles_AllUsers_CreatorId",
                table: "Raffles",
                column: "CreatorId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_AllUsers_StreamerId",
                table: "Subscribers",
                column: "StreamerId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_AllUsers_UserId",
                table: "Subscribers",
                column: "UserId",
                principalTable: "AllUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefaultUserRaffle_AllUsers_ParticipantsId",
                table: "DefaultUserRaffle");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultUserRaffle1_AllUsers_WinnersId",
                table: "DefaultUserRaffle1");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultUserStreamer_AllUsers_AdminsId",
                table: "DefaultUserStreamer");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultUserStreamer_AllUsers_NegotiableId",
                table: "DefaultUserStreamer");

            migrationBuilder.DropForeignKey(
                name: "FK_Raffles_AllUsers_CreatorId",
                table: "Raffles");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_AllUsers_StreamerId",
                table: "Subscribers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_AllUsers_UserId",
                table: "Subscribers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllUsers",
                table: "AllUsers");

            migrationBuilder.RenameTable(
                name: "AllUsers",
                newName: "DefaultUser");

            migrationBuilder.RenameIndex(
                name: "IX_AllUsers_TgId",
                table: "DefaultUser",
                newName: "IX_DefaultUser_TgId");

            migrationBuilder.RenameIndex(
                name: "IX_AllUsers_Name",
                table: "DefaultUser",
                newName: "IX_DefaultUser_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DefaultUser",
                table: "DefaultUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultUserRaffle_DefaultUser_ParticipantsId",
                table: "DefaultUserRaffle",
                column: "ParticipantsId",
                principalTable: "DefaultUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultUserRaffle1_DefaultUser_WinnersId",
                table: "DefaultUserRaffle1",
                column: "WinnersId",
                principalTable: "DefaultUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultUserStreamer_DefaultUser_AdminsId",
                table: "DefaultUserStreamer",
                column: "AdminsId",
                principalTable: "DefaultUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultUserStreamer_DefaultUser_NegotiableId",
                table: "DefaultUserStreamer",
                column: "NegotiableId",
                principalTable: "DefaultUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Raffles_DefaultUser_CreatorId",
                table: "Raffles",
                column: "CreatorId",
                principalTable: "DefaultUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_DefaultUser_StreamerId",
                table: "Subscribers",
                column: "StreamerId",
                principalTable: "DefaultUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_DefaultUser_UserId",
                table: "Subscribers",
                column: "UserId",
                principalTable: "DefaultUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
