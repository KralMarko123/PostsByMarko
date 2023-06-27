using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspnetserver.Data.Migrations
{
    public partial class changed_request_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "20180076-b56d-4f18-b05b-a4c073d63a27", "4fe0d243-ea80-44f5-9412-5ebebbf4654d" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2e5be653-a7df-482a-8631-c1397c08d8fd", "4fe0d243-ea80-44f5-9412-5ebebbf4654d" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "20180076-b56d-4f18-b05b-a4c073d63a27", "8cc12489-77e8-4a0c-9480-6349dc230d67" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2e5be653-a7df-482a-8631-c1397c08d8fd", "8cc12489-77e8-4a0c-9480-6349dc230d67" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "20180076-b56d-4f18-b05b-a4c073d63a27");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2e5be653-a7df-482a-8631-c1397c08d8fd");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4fe0d243-ea80-44f5-9412-5ebebbf4654d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8cc12489-77e8-4a0c-9480-6349dc230d67");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6f84dc7b-5afe-41af-ae39-95f58d8ca5d2", "b9ca4b23-49d2-4c1f-ac37-2b82f89a21db", "User", null },
                    { "96958a9e-7d07-4e31-8333-dbd736c73523", "4c5895fd-6df5-4424-8ace-d56f334ffe61", "Admin", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "66c1344b-abd7-417b-b0fb-70a55e334cd6", 0, "28c03e02-3216-4b36-a9ec-895c01918f37", "test_user@test.com", true, "Test", "User", false, null, "TEST_USER@TEST.COM", "TEST_USER", "AQAAAAEAACcQAAAAENxyLzj2FL5EqNBosjWSpX7m3SuaVwaxaTczDCe+GUbfFHoCl6o+CB6pQOHyMzbS8A==", null, false, "d2f59de2-69f3-4a88-8555-888ab74e9d94", false, "test_user" },
                    { "f00d1d51-126f-4290-82bc-1aba743a2591", 0, "cc6a0054-0339-4a5f-b1f4-212bceb57e22", "kralmarko123@gmail.com", true, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAEAACcQAAAAEJju7JR208vN9kZY6b6HnnRWA3kAqtRJSK6LTP9FNnZ5OPkW1okv+orJV/R2e09ZIg==", null, false, "8b647685-301b-4987-9fa0-10174eafb24f", false, "kralmarko123@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "6f84dc7b-5afe-41af-ae39-95f58d8ca5d2", "66c1344b-abd7-417b-b0fb-70a55e334cd6" },
                    { "96958a9e-7d07-4e31-8333-dbd736c73523", "66c1344b-abd7-417b-b0fb-70a55e334cd6" },
                    { "6f84dc7b-5afe-41af-ae39-95f58d8ca5d2", "f00d1d51-126f-4290-82bc-1aba743a2591" },
                    { "96958a9e-7d07-4e31-8333-dbd736c73523", "f00d1d51-126f-4290-82bc-1aba743a2591" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "6f84dc7b-5afe-41af-ae39-95f58d8ca5d2", "66c1344b-abd7-417b-b0fb-70a55e334cd6" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "96958a9e-7d07-4e31-8333-dbd736c73523", "66c1344b-abd7-417b-b0fb-70a55e334cd6" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "6f84dc7b-5afe-41af-ae39-95f58d8ca5d2", "f00d1d51-126f-4290-82bc-1aba743a2591" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "96958a9e-7d07-4e31-8333-dbd736c73523", "f00d1d51-126f-4290-82bc-1aba743a2591" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6f84dc7b-5afe-41af-ae39-95f58d8ca5d2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96958a9e-7d07-4e31-8333-dbd736c73523");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "66c1344b-abd7-417b-b0fb-70a55e334cd6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f00d1d51-126f-4290-82bc-1aba743a2591");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "20180076-b56d-4f18-b05b-a4c073d63a27", "bb2d9b0c-8194-407d-82e1-ed0e6bba78ba", "Editor", "EDITOR" },
                    { "2e5be653-a7df-482a-8631-c1397c08d8fd", "57f46f2d-840e-41a5-961b-129281788ba7", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "4fe0d243-ea80-44f5-9412-5ebebbf4654d", 0, "76c45055-c9bf-4a16-b74c-d4a56984d63e", "kralmarko123@gmail.com", true, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAEAACcQAAAAEOi/yAiXIOLqAGVAl+SnzqU6ZE01qepyczjHExm9YbobRuCJJQjjuUXOtQxLMp87hw==", null, false, "017a3a6f-add7-4eb0-8b76-617f2ed8d26f", false, "kralmarko123@gmail.com" },
                    { "8cc12489-77e8-4a0c-9480-6349dc230d67", 0, "eb321692-18f6-499a-9564-fe1b2633dacb", "test_user@test.com", true, "Test", "User", false, null, "TEST_USER@TEST.COM", "TEST_USER", "AQAAAAEAACcQAAAAEK2ZEefpKJT0s7CvA/ZWCCHNULSP9b38xTzYyxKf1sSXcwzfd2Hrw7fgOsMmoDTvfg==", null, false, "a98a7ea4-7357-4d2e-a112-ef0386b12444", false, "test_user" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "20180076-b56d-4f18-b05b-a4c073d63a27", "4fe0d243-ea80-44f5-9412-5ebebbf4654d" },
                    { "2e5be653-a7df-482a-8631-c1397c08d8fd", "4fe0d243-ea80-44f5-9412-5ebebbf4654d" },
                    { "20180076-b56d-4f18-b05b-a4c073d63a27", "8cc12489-77e8-4a0c-9480-6349dc230d67" },
                    { "2e5be653-a7df-482a-8631-c1397c08d8fd", "8cc12489-77e8-4a0c-9480-6349dc230d67" }
                });
        }
    }
}
