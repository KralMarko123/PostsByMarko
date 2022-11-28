using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspnetserver.Data.Migrations
{
    public partial class postUserRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "PostId",
                keyValue: 3);

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

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "64e963a4-86fd-40c0-9871-424288327b5f", "a07dd0d5-3401-4647-a655-df26778f7121", "Editor", "EDITOR" },
                    { "720415f1-b329-43bc-aaa0-affe485b3edd", "85fd1f25-786c-41f2-8210-08f2b718a9cf", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "721848a7-34d4-44f3-b729-8fd2195dea21", 0, "c6de267b-d1bf-4212-bf8d-071f915569d9", "kralmarko123@gmail.com", false, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAEAACcQAAAAELOimvYz5E1AYbku8rETmWnFgnnJWiHV4AgJth7lbQnug+L46H8Bq7dwpnxKmzE/IQ==", null, false, "1e42d422-2cb2-42a9-a62c-e36c4a9e0396", false, "kralmarko123@gmail.com" },
                    { "bdfb2d39-956d-4911-8925-53459b97b303", 0, "b5c151e3-2722-49cb-a527-fbd7a3952130", "test@test.com", false, "Test", "Testerson", false, null, "TEST@TEST.COM", "TEST@TEST.COM", "AQAAAAEAACcQAAAAEGcgFRNmLLpm2s6iaVLBeXLo4wzyugGNBKllP37P70tjylEY7pRls4ccUDzwnFcueQ==", null, false, "5af8d68a-070f-4437-82b2-7ec12779fa72", false, "test@test.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "64e963a4-86fd-40c0-9871-424288327b5f", "721848a7-34d4-44f3-b729-8fd2195dea21" },
                    { "720415f1-b329-43bc-aaa0-affe485b3edd", "721848a7-34d4-44f3-b729-8fd2195dea21" },
                    { "64e963a4-86fd-40c0-9871-424288327b5f", "bdfb2d39-956d-4911-8925-53459b97b303" },
                    { "720415f1-b329-43bc-aaa0-affe485b3edd", "bdfb2d39-956d-4911-8925-53459b97b303" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "64e963a4-86fd-40c0-9871-424288327b5f", "721848a7-34d4-44f3-b729-8fd2195dea21" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "720415f1-b329-43bc-aaa0-affe485b3edd", "721848a7-34d4-44f3-b729-8fd2195dea21" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "64e963a4-86fd-40c0-9871-424288327b5f", "bdfb2d39-956d-4911-8925-53459b97b303" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "720415f1-b329-43bc-aaa0-affe485b3edd", "bdfb2d39-956d-4911-8925-53459b97b303" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64e963a4-86fd-40c0-9871-424288327b5f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "720415f1-b329-43bc-aaa0-affe485b3edd");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "721848a7-34d4-44f3-b729-8fd2195dea21");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bdfb2d39-956d-4911-8925-53459b97b303");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "Posts");

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

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Content", "Title" },
                values: new object[,]
                {
                    { 1, "This is Post No.1's content.", "Post No.1" },
                    { 2, "This is Post No.2's content.", "Post No.2" },
                    { 3, "This is Post No.3's content.", "Post No.3" }
                });

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
    }
}
