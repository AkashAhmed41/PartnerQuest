using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendWebApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class MessageGroupsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessagesGroups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesGroups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    SignalRGroupName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_Connections_MessagesGroups_SignalRGroupName",
                        column: x => x.SignalRGroupName,
                        principalTable: "MessagesGroups",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_SignalRGroupName",
                table: "Connections",
                column: "SignalRGroupName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "MessagesGroups");
        }
    }
}
