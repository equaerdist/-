using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class addUsersNotesInRaffles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultUserRaffle");

            migrationBuilder.DropTable(
                name: "DefaultUserRaffle1");

            migrationBuilder.CreateTable(
                name: "ParticipantNote",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParticipantId = table.Column<long>(type: "bigint", nullable: false),
                    RaffleId = table.Column<long>(type: "bigint", nullable: false),
                    HaveAbused = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantNote_AllUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AllUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantNote_Raffles_RaffleId",
                        column: x => x.RaffleId,
                        principalTable: "Raffles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WinnerNote",
                columns: table => new
                {
                    WinnerId = table.Column<long>(type: "bigint", nullable: false),
                    RaffleId = table.Column<long>(type: "bigint", nullable: false),
                    AmountOfWins = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WinnerNote", x => new { x.WinnerId, x.RaffleId });
                    table.ForeignKey(
                        name: "FK_WinnerNote_AllUsers_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "AllUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WinnerNote_Raffles_RaffleId",
                        column: x => x.RaffleId,
                        principalTable: "Raffles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantNote_ParticipantId",
                table: "ParticipantNote",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantNote_RaffleId",
                table: "ParticipantNote",
                column: "RaffleId");

            migrationBuilder.CreateIndex(
                name: "IX_WinnerNote_RaffleId",
                table: "WinnerNote",
                column: "RaffleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantNote");

            migrationBuilder.DropTable(
                name: "WinnerNote");

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
                        name: "FK_DefaultUserRaffle_AllUsers_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "AllUsers",
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
                        name: "FK_DefaultUserRaffle1_AllUsers_WinnersId",
                        column: x => x.WinnersId,
                        principalTable: "AllUsers",
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
                name: "IX_DefaultUserRaffle_ParticipantsId",
                table: "DefaultUserRaffle",
                column: "ParticipantsId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultUserRaffle1_WinnersId",
                table: "DefaultUserRaffle1",
                column: "WinnersId");
        }
    }
}
