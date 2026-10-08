using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostsByMarko.Host.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaginationIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create replacement indexes first: MariaDB foreign keys depend on their leading columns.
            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId_LastUpdatedAt",
                table: "Posts",
                columns: new[] { "AuthorId", "LastUpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedAt_Id",
                table: "Posts",
                columns: new[] { "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId_CreatedAt_Id",
                table: "Messages",
                columns: new[] { "ChatId", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UpdatedAt_Id",
                table: "Chats",
                columns: new[] { "UpdatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email_Id",
                table: "AspNetUsers",
                columns: new[] { "Email", "Id" });

            migrationBuilder.DropIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId_CreatedAt",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore the foreign-key-supporting indexes before dropping their replacements.
            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId_CreatedAt",
                table: "Messages",
                columns: new[] { "ChatId", "CreatedAt" });

            migrationBuilder.DropIndex(
                name: "IX_Posts_AuthorId_LastUpdatedAt",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedAt_Id",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId_CreatedAt_Id",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UpdatedAt_Id",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email_Id",
                table: "AspNetUsers");

        }
    }
}
