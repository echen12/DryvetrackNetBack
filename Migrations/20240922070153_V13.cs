using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DryvetrackTest.Migrations
{
    /// <inheritdoc />
    public partial class V13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarId",
                table: "Insurance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarId",
                table: "Insurance",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
