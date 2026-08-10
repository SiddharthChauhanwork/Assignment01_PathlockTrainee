using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniquePrimaryDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationClients_PrimaryDomain",
                table: "ApplicationClients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApplicationClients_PrimaryDomain",
                table: "ApplicationClients",
                column: "PrimaryDomain",
                unique: true);
        }
    }
}
