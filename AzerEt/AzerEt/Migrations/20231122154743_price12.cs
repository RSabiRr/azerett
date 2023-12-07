using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzerEt.Migrations
{
    /// <inheritdoc />
    public partial class price12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "Checkouts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Success",
                table: "Checkouts");
        }
    }
}
