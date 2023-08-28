using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendWebApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class FavouriteUsersModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavouriteUsersDb",
                columns: table => new
                {
                    SourceUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    FavouriteUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteUsersDb", x => new { x.SourceUserId, x.FavouriteUserId });
                    table.ForeignKey(
                        name: "FK_FavouriteUsersDb_Users_FavouriteUserId",
                        column: x => x.FavouriteUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavouriteUsersDb_Users_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteUsersDb_FavouriteUserId",
                table: "FavouriteUsersDb",
                column: "FavouriteUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouriteUsersDb");
        }
    }
}
