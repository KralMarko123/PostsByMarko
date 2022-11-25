using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspnetserver.Data.Migrations
{
    public partial class addedSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8e7db51d-d0b1-461b-b486-2065d85af760", "8e6e2e08-5104-4c65-a940-3e19a2381aaf", "User", "USER" },
                    { "a8b62d80-f16c-4804-ab1d-05b459a10e22", "071426d2-9f68-44b4-97fa-149b194cfc2e", "Admin", "ADMIN" },
                    { "cf47ff15-5ef3-4d2a-95f5-73dc8b53dbf3", "8d952018-b07a-40d6-9576-1dd025a2ad9f", "Editor", "EDITOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "64df9a45-b607-4fe7-9f17-f8d78328d7e2", 0, "be0ed6f6-2e65-4bb3-abb5-7401788fad36", "test@test.com", false, "Test", "Testerson", false, null, "TEST@TEST.COM", "TEST@TEST.COM", "AQAAAAEAACcQAAAAEJEc3fgRgdz4yY6qobgHzNIUWz2Tmsuz5BtnEHXwe3+DDtEE4XLA80GCpSVKLOLM/Q==", null, false, "f336e02f-56de-4a9d-b3e8-832cc24accfa", false, "test@test.com" },
                    { "d68e1efa-5c81-47c2-888e-4e813362319a", 0, "daba4dae-711e-4614-b007-9acc8f3288a4", "kralmarko123@gmail.com", false, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAEAACcQAAAAEIyVn24jbg3C2hg3z5HSKezfmDyZEX5fXgSixXqKgddEYPpkbPljS9RUM/wJP1UzSA==", null, false, "7890f4b1-f1b4-4766-b933-1ec23929bcb7", false, "kralmarko123@gmail.com" }
                });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 1,
                columns: new[] { "Content", "Title" },
                values: new object[] { "This is Post No.1's content.", "Post No.1" });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 2,
                columns: new[] { "Content", "Title" },
                values: new object[] { "This is Post No.2's content.", "Post No.2" });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 3,
                columns: new[] { "Content", "Title" },
                values: new object[] { "This is Post No.3's content.", "Post No.3" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "8e7db51d-d0b1-461b-b486-2065d85af760", "64df9a45-b607-4fe7-9f17-f8d78328d7e2" },
                    { "a8b62d80-f16c-4804-ab1d-05b459a10e22", "64df9a45-b607-4fe7-9f17-f8d78328d7e2" },
                    { "cf47ff15-5ef3-4d2a-95f5-73dc8b53dbf3", "64df9a45-b607-4fe7-9f17-f8d78328d7e2" },
                    { "8e7db51d-d0b1-461b-b486-2065d85af760", "d68e1efa-5c81-47c2-888e-4e813362319a" },
                    { "a8b62d80-f16c-4804-ab1d-05b459a10e22", "d68e1efa-5c81-47c2-888e-4e813362319a" },
                    { "cf47ff15-5ef3-4d2a-95f5-73dc8b53dbf3", "d68e1efa-5c81-47c2-888e-4e813362319a" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8e7db51d-d0b1-461b-b486-2065d85af760", "64df9a45-b607-4fe7-9f17-f8d78328d7e2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a8b62d80-f16c-4804-ab1d-05b459a10e22", "64df9a45-b607-4fe7-9f17-f8d78328d7e2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "cf47ff15-5ef3-4d2a-95f5-73dc8b53dbf3", "64df9a45-b607-4fe7-9f17-f8d78328d7e2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8e7db51d-d0b1-461b-b486-2065d85af760", "d68e1efa-5c81-47c2-888e-4e813362319a" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a8b62d80-f16c-4804-ab1d-05b459a10e22", "d68e1efa-5c81-47c2-888e-4e813362319a" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "cf47ff15-5ef3-4d2a-95f5-73dc8b53dbf3", "d68e1efa-5c81-47c2-888e-4e813362319a" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e7db51d-d0b1-461b-b486-2065d85af760");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a8b62d80-f16c-4804-ab1d-05b459a10e22");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cf47ff15-5ef3-4d2a-95f5-73dc8b53dbf3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "64df9a45-b607-4fe7-9f17-f8d78328d7e2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d68e1efa-5c81-47c2-888e-4e813362319a");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 1,
                columns: new[] { "Content", "Title" },
                values: new object[] { "Post No.1's content.", "Post 1" });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 2,
                columns: new[] { "Content", "Title" },
                values: new object[] { "Post No.2's content.", "Post 2" });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 3,
                columns: new[] { "Content", "Title" },
                values: new object[] { "Post No.3's content.", "Post 3" });
        }
    }
}
