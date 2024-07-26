using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    /// <inheritdoc />
    public partial class addUsernameProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "AllUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "AllUsers");
        }
    }
}
