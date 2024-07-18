using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class addInheritance1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Raffles_Streamers_CreatorId",
                table: "Raffles");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_Streamers_StreamerId",
                table: "Subscribers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_Users_UserId",
                table: "Subscribers");

            migrationBuilder.DropTable(
                name: "RaffleUser");

            migrationBuilder.DropTable(
                name: "RaffleUser1");

            migrationBuilder.DropTable(
                name: "Streamers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "DefaultUser");

            migrationBuilder.RenameIndex(
                name: "IX_Users_TgId",
                table: "DefaultUser",
                newName: "IX_DefaultUser_TgId");

            migrationBuilder.AlterColumn<string>(
                name: "PayMethods",
                table: "DefaultUser",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "DefaultUser",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DefaultUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Socials",
                table: "DefaultUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DefaultUser",
                table: "DefaultUser",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DefaultUserRaffle",
                columns: table => new
                {
                    ParticipantRafflesId = table.Column<long>(type: "bigint", nullable: false),
                    ParticipantsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultUserRaffle", x => new { x.ParticipantRafflesId, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_DefaultUserRaffle_DefaultUser_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "DefaultUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultUserRaffle_Raffles_ParticipantRafflesId",
                        column: x => x.ParticipantRafflesId,
                        principalTable: "Raffles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultUserRaffle1",
                columns: table => new
                {
                    WinnerRafflesId = table.Column<long>(type: "bigint", nullable: false),
                    WinnersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultUserRaffle1", x => new { x.WinnerRafflesId, x.WinnersId });
                    table.ForeignKey(
                        name: "FK_DefaultUserRaffle1_DefaultUser_WinnersId",
                        column: x => x.WinnersId,
                        principalTable: "DefaultUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultUserRaffle1_Raffles_WinnerRafflesId",
                        column: x => x.WinnerRafflesId,
                        principalTable: "Raffles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultUser_Name",
                table: "DefaultUser",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DefaultUserRaffle_ParticipantsId",
                table: "DefaultUserRaffle",
                column: "ParticipantsId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultUserRaffle1_WinnersId",
                table: "DefaultUserRaffle1",
                column: "WinnersId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Raffles_DefaultUser_CreatorId",
                table: "Raffles");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_DefaultUser_StreamerId",
                table: "Subscribers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_DefaultUser_UserId",
                table: "Subscribers");

            migrationBuilder.DropTable(
                name: "DefaultUserRaffle");

            migrationBuilder.DropTable(
                name: "DefaultUserRaffle1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DefaultUser",
                table: "DefaultUser");

            migrationBuilder.DropIndex(
                name: "IX_DefaultUser_Name",
                table: "DefaultUser");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "DefaultUser");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DefaultUser");

            migrationBuilder.DropColumn(
                name: "Socials",
                table: "DefaultUser");

            migrationBuilder.RenameTable(
                name: "DefaultUser",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultUser_TgId",
                table: "Users",
                newName: "IX_Users_TgId");

            migrationBuilder.AlterColumn<string>(
                name: "PayMethods",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RaffleUser",
                columns: table => new
                {
                    ParticipantRafflesId = table.Column<long>(type: "bigint", nullable: false),
                    ParticipantsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaffleUser", x => new { x.ParticipantRafflesId, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_RaffleUser_Raffles_ParticipantRafflesId",
                        column: x => x.ParticipantRafflesId,
                        principalTable: "Raffles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaffleUser_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaffleUser1",
                columns: table => new
                {
                    WinnerRafflesId = table.Column<long>(type: "bigint", nullable: false),
                    WinnersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaffleUser1", x => new { x.WinnerRafflesId, x.WinnersId });
                    table.ForeignKey(
                        name: "FK_RaffleUser1_Raffles_WinnerRafflesId",
                        column: x => x.WinnerRafflesId,
                        principalTable: "Raffles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaffleUser1_Users_WinnersId",
                        column: x => x.WinnersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Streamers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Socials = table.Column<string>(type: "text", nullable: false),
                    TgId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streamers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaffleUser_ParticipantsId",
                table: "RaffleUser",
                column: "ParticipantsId");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleUser1_WinnersId",
                table: "RaffleUser1",
                column: "WinnersId");

            migrationBuilder.CreateIndex(
                name: "IX_Streamers_Name",
                table: "Streamers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streamers_TgId",
                table: "Streamers",
                column: "TgId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Raffles_Streamers_CreatorId",
                table: "Raffles",
                column: "CreatorId",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_Streamers_StreamerId",
                table: "Subscribers",
                column: "StreamerId",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_Users_UserId",
                table: "Subscribers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
