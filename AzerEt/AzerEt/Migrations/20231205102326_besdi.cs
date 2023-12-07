using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzerEt.Migrations
{
    /// <inheritdoc />
    public partial class besdi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnlinee",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnlinee",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }
    }
}
