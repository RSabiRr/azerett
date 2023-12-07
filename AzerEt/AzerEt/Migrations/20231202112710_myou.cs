using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzerEt.Migrations
{
    /// <inheritdoc />
    public partial class myou : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOnline",
                table: "AspNetUsers",
                newName: "IsOnlinee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOnlinee",
                table: "AspNetUsers",
                newName: "IsOnline");
        }
    }
}
