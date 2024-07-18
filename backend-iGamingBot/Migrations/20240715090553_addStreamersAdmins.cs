using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class addStreamersAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultUserStreamer",
                columns: table => new
                {
                    AdminsId = table.Column<long>(type: "bigint", nullable: false),
                    NegotiableId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultUserStreamer", x => new { x.AdminsId, x.NegotiableId });
                    table.ForeignKey(
                        name: "FK_DefaultUserStreamer_DefaultUser_AdminsId",
                        column: x => x.AdminsId,
                        principalTable: "DefaultUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultUserStreamer_DefaultUser_NegotiableId",
                        column: x => x.NegotiableId,
                        principalTable: "DefaultUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultUserStreamer_NegotiableId",
                table: "DefaultUserStreamer",
                column: "NegotiableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultUserStreamer");
        }
    }
}
