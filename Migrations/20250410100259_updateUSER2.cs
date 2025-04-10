using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BKAC.Migrations
{
    /// <inheritdoc />
    public partial class updateUSER2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Avatar",
                table: "Users",
                newName: "FaceImg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FaceImg",
                table: "Users",
                newName: "Avatar");
        }
    }
}
