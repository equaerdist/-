using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class relationsSubscriber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TgId",
                table: "Streamers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Raffles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AmountOfWinners = table.Column<int>(type: "integer", nullable: false),
                    ShowWinners = table.Column<bool>(type: "boolean", nullable: false),
                    RaffleConditions = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShouldNotifyUsers = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raffles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Raffles_Streamers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TgId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    PayMethods = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

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
                name: "Subscribers",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StreamerId = table.Column<long>(type: "bigint", nullable: false),
                    SubscribeTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => new { x.UserId, x.StreamerId });
                    table.ForeignKey(
                        name: "FK_Subscribers_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscribers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Raffles_CreatorId",
                table: "Raffles",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleUser_ParticipantsId",
                table: "RaffleUser",
                column: "ParticipantsId");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleUser1_WinnersId",
                table: "RaffleUser1",
                column: "WinnersId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_StreamerId",
                table: "Subscribers",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TgId",
                table: "Users",
                column: "TgId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaffleUser");

            migrationBuilder.DropTable(
                name: "RaffleUser1");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "Raffles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Streamers_Name",
                table: "Streamers");

            migrationBuilder.DropIndex(
                name: "IX_Streamers_TgId",
                table: "Streamers");

            migrationBuilder.DropColumn(
                name: "TgId",
                table: "Streamers");
        }
    }
}
