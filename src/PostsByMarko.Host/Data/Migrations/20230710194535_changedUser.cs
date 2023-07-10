using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostsByMarko.Host.Data.Migrations
{
    public partial class changedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "030a6ad6-dcab-4955-b04e-afa3cbea05a4", "31f8d175-658e-44f7-b5ae-90ca7b5b15b2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b0aaeedc-c0e4-4091-8eed-651260aa6ceb", "31f8d175-658e-44f7-b5ae-90ca7b5b15b2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "030a6ad6-dcab-4955-b04e-afa3cbea05a4", "e6d5370e-e83c-4dd8-bcc9-64ca3501428e" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b0aaeedc-c0e4-4091-8eed-651260aa6ceb", "e6d5370e-e83c-4dd8-bcc9-64ca3501428e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "030a6ad6-dcab-4955-b04e-afa3cbea05a4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b0aaeedc-c0e4-4091-8eed-651260aa6ceb");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "31f8d175-658e-44f7-b5ae-90ca7b5b15b2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e6d5370e-e83c-4dd8-bcc9-64ca3501428e");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3f10497c-85db-487f-b1f5-e624a911b8ec", "ec063470-5454-4cbf-a7cf-d511b7ac4064", "Admin", null },
                    { "4a25666e-a7cc-4d26-af42-5a0f8d37b8ed", "6859092b-8c35-4cc9-9542-b388746195e5", "User", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PostIds", "ProfilePicture", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "250d08fa-5fa6-439d-992e-067dcbb58d27", 0, "aa184cf3-3c12-4e64-95fd-e820cefe9d7e", "test_user@test.com", true, "Test", "User", false, null, "TEST_USER@TEST.COM", "TEST_USER@TEST.COM", "AQAAAAEAACcQAAAAEFpz6SWCtQJQ9QUnuIvH/7UT1ti7KQZt/zbPzN0E5uDNqhJDQAAFd6lCLH6F90OKOg==", null, false, "[]", null, "6bea5f7b-f983-488a-ae8e-478d6c6a2919", false, "test_user@test.com" },
                    { "8f5cad38-5bf2-41f0-8a0c-70551f4d7003", 0, "2864f13b-bf08-4830-af7e-bbf85fc3e5c8", "kralmarko123@gmail.com", true, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAEAACcQAAAAEHgarPJNqWFgvroZSnCYXkNIAmaKmGNJBUxB3f0tQeikgb5OwEg3dnKbd9tioMrUww==", null, false, "[]", null, "7ff65537-c0ea-4640-ac60-b524ce6ab005", false, "kralmarko123@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "3f10497c-85db-487f-b1f5-e624a911b8ec", "250d08fa-5fa6-439d-992e-067dcbb58d27" },
                    { "4a25666e-a7cc-4d26-af42-5a0f8d37b8ed", "250d08fa-5fa6-439d-992e-067dcbb58d27" },
                    { "3f10497c-85db-487f-b1f5-e624a911b8ec", "8f5cad38-5bf2-41f0-8a0c-70551f4d7003" },
                    { "4a25666e-a7cc-4d26-af42-5a0f8d37b8ed", "8f5cad38-5bf2-41f0-8a0c-70551f4d7003" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3f10497c-85db-487f-b1f5-e624a911b8ec", "250d08fa-5fa6-439d-992e-067dcbb58d27" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4a25666e-a7cc-4d26-af42-5a0f8d37b8ed", "250d08fa-5fa6-439d-992e-067dcbb58d27" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3f10497c-85db-487f-b1f5-e624a911b8ec", "8f5cad38-5bf2-41f0-8a0c-70551f4d7003" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4a25666e-a7cc-4d26-af42-5a0f8d37b8ed", "8f5cad38-5bf2-41f0-8a0c-70551f4d7003" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f10497c-85db-487f-b1f5-e624a911b8ec");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a25666e-a7cc-4d26-af42-5a0f8d37b8ed");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "250d08fa-5fa6-439d-992e-067dcbb58d27");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8f5cad38-5bf2-41f0-8a0c-70551f4d7003");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "030a6ad6-dcab-4955-b04e-afa3cbea05a4", "20b643c3-f349-4cb0-bafc-5d34141ec35c", "Admin", null },
                    { "b0aaeedc-c0e4-4091-8eed-651260aa6ceb", "2bf0af8f-2e8f-4dc9-9f06-4391a6f7b5a4", "User", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PostIds", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "31f8d175-658e-44f7-b5ae-90ca7b5b15b2", 0, "1d29dd70-e1a5-4523-8668-8f8a819ef467", "test_user@test.com", true, "Test", "User", false, null, "TEST_USER@TEST.COM", "TEST_USER@TEST.COM", "AQAAAAEAACcQAAAAEBjSTWCeeyprRZVsaS7gB/CoZD60o1+eMsQu46DONT+3nCyedoU7qKYEdNCyu7/XBg==", null, false, "[]", "51b483e7-4368-4be6-a8dd-b23776d4872a", false, "test_user@test.com" },
                    { "e6d5370e-e83c-4dd8-bcc9-64ca3501428e", 0, "5a15a491-4542-4765-8e37-493712d097f1", "kralmarko123@gmail.com", true, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAEAACcQAAAAEKVL1tnA2R15rJd/LHgwxCCgnqbq2ZffdH7fqOAQbfPjXar7CeHH+k9Vfu85n+DDyA==", null, false, "[]", "aaac75a5-136b-4cdd-a6c5-8f56a98041f4", false, "kralmarko123@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "030a6ad6-dcab-4955-b04e-afa3cbea05a4", "31f8d175-658e-44f7-b5ae-90ca7b5b15b2" },
                    { "b0aaeedc-c0e4-4091-8eed-651260aa6ceb", "31f8d175-658e-44f7-b5ae-90ca7b5b15b2" },
                    { "030a6ad6-dcab-4955-b04e-afa3cbea05a4", "e6d5370e-e83c-4dd8-bcc9-64ca3501428e" },
                    { "b0aaeedc-c0e4-4091-8eed-651260aa6ceb", "e6d5370e-e83c-4dd8-bcc9-64ca3501428e" }
                });
        }
    }
}
