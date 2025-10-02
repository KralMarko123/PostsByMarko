using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PostsByMarko.Host.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "607eea06-5a81-4333-82c8-5ef79a5e6288", "09758977-92c3-4ae5-8f3d-9dc62e722e32" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0505823b-26ae-4215-aaca-5f76121e58f6", "557f75e3-b015-44d0-9806-20ccc89c81cf" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "607eea06-5a81-4333-82c8-5ef79a5e6288", "557f75e3-b015-44d0-9806-20ccc89c81cf" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0505823b-26ae-4215-aaca-5f76121e58f6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "607eea06-5a81-4333-82c8-5ef79a5e6288");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "09758977-92c3-4ae5-8f3d-9dc62e722e32");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "557f75e3-b015-44d0-9806-20ccc89c81cf");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "289103e7-d4bd-4cb8-8f49-0047a888a811", null, "User", "USER" },
                    { "3760e078-c3fb-4d15-8c0b-63c5247668f7", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "04e54d11-5489-4a27-bb5e-5ba89ea961a3", 0, "95147b46-0929-43e3-abe8-490d5835014b", new DateTime(2025, 10, 2, 13, 50, 58, 282, DateTimeKind.Utc).AddTicks(2479), "kralmarko123@gmail.com", true, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAIAAYagAAAAEMJ7YV1bprWxC6z3dz1Z29dQletH5Nr/2lht7ZhDA7YOcGv3Hk6IojQgAC12VkIyfg==", null, false, null, "5972e16d-e5b5-4851-ab2e-e793a50d2611", false, "kralmarko123@gmail.com" },
                    { "0eafdfb4-8ed2-4072-9fd5-1b4c44d4c840", 0, "db06f99b-5530-4359-9ef9-f118fdf6549a", new DateTime(2025, 10, 2, 13, 50, 58, 282, DateTimeKind.Utc).AddTicks(2963), "test@test.com", true, "Test", "User", false, null, "TEST@TEST.COM", "TEST@TEST.COM", "AQAAAAIAAYagAAAAEDf+os0nozU70ylEm9JSer2SpyG/VArG6F34c7IQzeXOVWFDVCIsxBGZTPjpFSU1JQ==", null, false, null, "68e5d95e-14ac-47c8-a159-dac683554b47", false, "test@test.com" },
                    { "22b10fc8-60e0-4a76-b3a8-3f6d774a0cf1", 0, "1ca5e8ec-9d8d-4f1e-8e21-4e5628830416", new DateTime(2025, 10, 2, 13, 50, 58, 282, DateTimeKind.Utc).AddTicks(2979), "ryanfirth@generic.com", true, "Ryan", "Firth", false, null, "RYANFIRTH@GENERIC.COM", "RYANFIRTH@GENERIC.COM", "AQAAAAIAAYagAAAAECn45if8UnldIXOSQ70jzDkmcAXqn6gORyPk5WZ46F6e7G7dCn4JhbNP33TK09ADOw==", null, false, null, "f368cf1d-b16d-4ffb-9062-07b39736567f", false, "ryanfirth@generic.com" },
                    { "512fc521-1e45-4333-8aa5-0d728829cc6a", 0, "e347b04f-5811-415c-b018-039f2f9a57f4", new DateTime(2025, 10, 2, 13, 50, 58, 408, DateTimeKind.Utc).AddTicks(8012), "Chasity.Nader@yahoo.com", true, "Ally", "Kirlin", false, null, "CHASITY.NADER@YAHOO.COM", "CHASITY.NADER@YAHOO.COM", "AQAAAAIAAYagAAAAEAk4KthOHWRxVSND3HZbpNqeXGx6VSbEvQdqVcu+NzKvWl61m0KfhWiw6u6JMexOiA==", null, false, null, "95d8ea03-33dc-4506-91a2-95a3c01b9dd7", false, "Chasity.Nader@yahoo.com" },
                    { "589490e5-f0c5-4789-bcca-6dc281c1dade", 0, "84659402-781f-410b-9bc8-b007f0b17bcb", new DateTime(2025, 10, 2, 13, 50, 58, 408, DateTimeKind.Utc).AddTicks(7961), "Jolie_Kunde18@gmail.com", true, "Peyton", "Lueilwitz", false, null, "JOLIE_KUNDE18@GMAIL.COM", "JOLIE_KUNDE18@GMAIL.COM", "AQAAAAIAAYagAAAAEOTa0ilsXgo3KFJggrpGA1bfqdjx8P+w4T47k7b+27wg6yIgHaYWq9W/kMbkzaoBKQ==", null, false, null, "15f54b25-5827-44e9-889d-fdcaa4b3ae24", false, "Jolie_Kunde18@gmail.com" },
                    { "e6107e0a-5cef-49a6-b051-59b8d0f840da", 0, "7a1ce3f7-f5c1-461a-a90c-0ac5ef47ed8d", new DateTime(2025, 10, 2, 13, 50, 58, 408, DateTimeKind.Utc).AddTicks(2352), "Lisa_Emard@hotmail.com", true, "Makenna", "Franecki", false, null, "LISA_EMARD@HOTMAIL.COM", "LISA_EMARD@HOTMAIL.COM", "AQAAAAIAAYagAAAAEOaAxciXy8iQDuC2wiW9654TpmzG5d1D+na7EakkSTcLJ4tGsv8Ja+T/pVP7ays1AA==", null, false, null, "11e2f54a-a621-4926-96b1-018e48b6860b", false, "Lisa_Emard@hotmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "289103e7-d4bd-4cb8-8f49-0047a888a811", "04e54d11-5489-4a27-bb5e-5ba89ea961a3" },
                    { "3760e078-c3fb-4d15-8c0b-63c5247668f7", "04e54d11-5489-4a27-bb5e-5ba89ea961a3" },
                    { "289103e7-d4bd-4cb8-8f49-0047a888a811", "0eafdfb4-8ed2-4072-9fd5-1b4c44d4c840" },
                    { "289103e7-d4bd-4cb8-8f49-0047a888a811", "22b10fc8-60e0-4a76-b3a8-3f6d774a0cf1" },
                    { "289103e7-d4bd-4cb8-8f49-0047a888a811", "512fc521-1e45-4333-8aa5-0d728829cc6a" },
                    { "289103e7-d4bd-4cb8-8f49-0047a888a811", "589490e5-f0c5-4789-bcca-6dc281c1dade" },
                    { "289103e7-d4bd-4cb8-8f49-0047a888a811", "e6107e0a-5cef-49a6-b051-59b8d0f840da" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedDate", "IsHidden", "LastUpdatedDate", "Title" },
                values: new object[,]
                {
                    { "00693ea7-573c-4a7c-ae9c-7b48d56b1e31", "512fc521-1e45-4333-8aa5-0d728829cc6a", "It only works when I'm Argentina.", new DateTime(2025, 9, 12, 2, 11, 3, 891, DateTimeKind.Utc).AddTicks(283), false, new DateTime(2025, 9, 22, 13, 23, 29, 499, DateTimeKind.Utc).AddTicks(9250), "Ergonomic Frozen Salad" },
                    { "04077f50-65a6-4b36-b010-32120e34e23a", "512fc521-1e45-4333-8aa5-0d728829cc6a", "This Handcrafted Cotton Bacon works so well. It imperfectly improves my baseball by a lot.", new DateTime(2025, 9, 23, 9, 11, 1, 333, DateTimeKind.Utc).AddTicks(5414), false, new DateTime(2025, 9, 21, 17, 42, 18, 979, DateTimeKind.Utc).AddTicks(5762), "Rustic Plastic Pizza" },
                    { "2028d319-293e-4381-836c-9d67ebe866b7", "e6107e0a-5cef-49a6-b051-59b8d0f840da", "I saw this on TV and wanted to give it a try.", new DateTime(2025, 9, 27, 15, 12, 35, 942, DateTimeKind.Utc).AddTicks(6057), true, new DateTime(2025, 9, 27, 3, 11, 52, 85, DateTimeKind.Utc).AddTicks(2246), "Fantastic Granite Towels" },
                    { "26c4ad3d-41dc-442b-9694-2b008c0729ae", "589490e5-f0c5-4789-bcca-6dc281c1dade", "My neighbor Elisha has one of these. She works as a fortune teller and she says it looks floppy.", new DateTime(2025, 9, 19, 6, 43, 31, 790, DateTimeKind.Utc).AddTicks(8137), false, new DateTime(2025, 9, 27, 7, 10, 57, 655, DateTimeKind.Utc).AddTicks(2317), "Incredible Cotton Car" },
                    { "47807512-6f2d-45ad-8bb6-209b2d00cfe1", "512fc521-1e45-4333-8aa5-0d728829cc6a", "This Refined Wooden Pants works outstandingly well. It beautifully improves my basketball by a lot.", new DateTime(2025, 9, 23, 4, 18, 0, 752, DateTimeKind.Utc).AddTicks(4635), true, new DateTime(2025, 9, 29, 16, 37, 30, 244, DateTimeKind.Utc).AddTicks(4867), "Refined Steel Towels" },
                    { "6eb8e634-d1f8-40a0-b89e-140610a4cdb4", "e6107e0a-5cef-49a6-b051-59b8d0f840da", "My neighbor Georgie has one of these. She works as a busboy and she says it looks brown.", new DateTime(2025, 9, 26, 14, 41, 42, 759, DateTimeKind.Utc).AddTicks(4517), false, new DateTime(2025, 9, 3, 12, 51, 52, 277, DateTimeKind.Utc).AddTicks(7300), "Handmade Granite Cheese" },
                    { "7fe9d149-1f66-49f7-9db8-3fe9cc440e38", "e6107e0a-5cef-49a6-b051-59b8d0f840da", "I saw one of these in Tanzania and I bought one.", new DateTime(2025, 9, 27, 20, 58, 15, 695, DateTimeKind.Utc).AddTicks(8109), false, new DateTime(2025, 9, 2, 19, 9, 8, 667, DateTimeKind.Utc).AddTicks(3511), "Rustic Metal Sausages" },
                    { "b95b82f4-7e6d-46a6-b61f-54a7b14971f8", "589490e5-f0c5-4789-bcca-6dc281c1dade", "I tried to slay it but got truffle all over it.", new DateTime(2025, 9, 5, 2, 39, 21, 850, DateTimeKind.Utc).AddTicks(7076), false, new DateTime(2025, 10, 1, 9, 37, 48, 200, DateTimeKind.Utc).AddTicks(4627), "Fantastic Granite Pizza" },
                    { "cbabfbe7-9c91-43e5-86de-e3dcf25a3ebb", "589490e5-f0c5-4789-bcca-6dc281c1dade", "The box this comes in is 5 foot by 6 inch and weights 17 pound!!!", new DateTime(2025, 9, 3, 10, 53, 26, 282, DateTimeKind.Utc).AddTicks(1810), false, new DateTime(2025, 9, 12, 1, 16, 11, 951, DateTimeKind.Utc).AddTicks(2811), "Sleek Metal Chair" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "289103e7-d4bd-4cb8-8f49-0047a888a811", "04e54d11-5489-4a27-bb5e-5ba89ea961a3" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3760e078-c3fb-4d15-8c0b-63c5247668f7", "04e54d11-5489-4a27-bb5e-5ba89ea961a3" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "289103e7-d4bd-4cb8-8f49-0047a888a811", "0eafdfb4-8ed2-4072-9fd5-1b4c44d4c840" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "289103e7-d4bd-4cb8-8f49-0047a888a811", "22b10fc8-60e0-4a76-b3a8-3f6d774a0cf1" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "289103e7-d4bd-4cb8-8f49-0047a888a811", "512fc521-1e45-4333-8aa5-0d728829cc6a" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "289103e7-d4bd-4cb8-8f49-0047a888a811", "589490e5-f0c5-4789-bcca-6dc281c1dade" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "289103e7-d4bd-4cb8-8f49-0047a888a811", "e6107e0a-5cef-49a6-b051-59b8d0f840da" });

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "00693ea7-573c-4a7c-ae9c-7b48d56b1e31");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "04077f50-65a6-4b36-b010-32120e34e23a");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "2028d319-293e-4381-836c-9d67ebe866b7");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "26c4ad3d-41dc-442b-9694-2b008c0729ae");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "47807512-6f2d-45ad-8bb6-209b2d00cfe1");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "6eb8e634-d1f8-40a0-b89e-140610a4cdb4");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "7fe9d149-1f66-49f7-9db8-3fe9cc440e38");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "b95b82f4-7e6d-46a6-b61f-54a7b14971f8");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "cbabfbe7-9c91-43e5-86de-e3dcf25a3ebb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "289103e7-d4bd-4cb8-8f49-0047a888a811");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3760e078-c3fb-4d15-8c0b-63c5247668f7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "04e54d11-5489-4a27-bb5e-5ba89ea961a3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0eafdfb4-8ed2-4072-9fd5-1b4c44d4c840");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22b10fc8-60e0-4a76-b3a8-3f6d774a0cf1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "512fc521-1e45-4333-8aa5-0d728829cc6a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "589490e5-f0c5-4789-bcca-6dc281c1dade");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e6107e0a-5cef-49a6-b051-59b8d0f840da");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0505823b-26ae-4215-aaca-5f76121e58f6", null, "Admin", null },
                    { "607eea06-5a81-4333-82c8-5ef79a5e6288", null, "User", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "09758977-92c3-4ae5-8f3d-9dc62e722e32", 0, "2a67a11a-a8c2-4ac4-b234-4136266689e0", "test@test.com", true, "Test", "User", false, null, "TEST@TEST.COM", "TEST@TEST.COM", "AQAAAAIAAYagAAAAECfasPvGmuW4YmAWr7xGPUVpGCvxwRYgus8diLzZjk+jrxGkk7Aa6+01stefktiP6Q==", null, false, null, "cf9b03f6-d69b-49a3-8a4e-d1867d761346", false, "test@test.com" },
                    { "557f75e3-b015-44d0-9806-20ccc89c81cf", 0, "ba2cb4f3-ab33-45f1-a905-613045c4b457", "kralmarko123@gmail.com", true, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAIAAYagAAAAEDR6R1Z/GCJwCA77U0laZwlALp5vkJ/3ZX/ihUmLeIAH/6bH0ib6eHvMvdolOYr66Q==", null, false, null, "2fed7ec6-d640-437a-b1e6-86144286e670", false, "kralmarko123@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "607eea06-5a81-4333-82c8-5ef79a5e6288", "09758977-92c3-4ae5-8f3d-9dc62e722e32" },
                    { "0505823b-26ae-4215-aaca-5f76121e58f6", "557f75e3-b015-44d0-9806-20ccc89c81cf" },
                    { "607eea06-5a81-4333-82c8-5ef79a5e6288", "557f75e3-b015-44d0-9806-20ccc89c81cf" }
                });
        }
    }
}
