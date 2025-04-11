using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVD_World.Migrations
{
    /// <inheritdoc />
    public partial class updateAdvertisementModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Advertisements",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "BannerUrl",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Advertisements",
                newName: "Position");

            migrationBuilder.AlterColumn<string>(
                name: "BannerUrl",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
