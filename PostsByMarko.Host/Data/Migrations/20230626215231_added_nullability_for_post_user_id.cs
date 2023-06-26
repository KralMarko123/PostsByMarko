using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspnetserver.Data.Migrations
{
    public partial class added_nullability_for_post_user_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4cddd91b-03fd-499c-b899-2523eaa1bf40", "171f313b-b676-44c6-9994-9f809f7aef72" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "9beb52fd-570d-42df-b966-d17da8ffc431", "171f313b-b676-44c6-9994-9f809f7aef72" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4cddd91b-03fd-499c-b899-2523eaa1bf40", "8fe5f090-84ff-4caa-9519-177fb86ef973" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "9beb52fd-570d-42df-b966-d17da8ffc431", "8fe5f090-84ff-4caa-9519-177fb86ef973" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4cddd91b-03fd-499c-b899-2523eaa1bf40");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9beb52fd-570d-42df-b966-d17da8ffc431");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "171f313b-b676-44c6-9994-9f809f7aef72");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8fe5f090-84ff-4caa-9519-177fb86ef973");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts");

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

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4cddd91b-03fd-499c-b899-2523eaa1bf40", "c3e2b444-fa78-4536-9aa8-2a0fa9f6e41b", "Admin", "ADMIN" },
                    { "9beb52fd-570d-42df-b966-d17da8ffc431", "68b5e8fd-5182-4a80-83ef-90fe2812ccec", "Editor", "EDITOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "171f313b-b676-44c6-9994-9f809f7aef72", 0, "34587d6d-f8ff-4191-975b-4a206608056e", "kralmarko123@gmail.com", true, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAEAACcQAAAAEOAQZbR3JOuI0H9i2134t2Zc7I9DrXQOc9cpMnkVQ76GvH9wxxzwewmPP7u29Di/Hg==", null, false, "470c7786-b44a-4e59-99e3-a668df7edf5d", false, "kralmarko123@gmail.com" },
                    { "8fe5f090-84ff-4caa-9519-177fb86ef973", 0, "d6aecd21-8259-4aa6-89af-c546a82392dd", "test@test.com", true, "Test", "Testerson", false, null, "TEST@TEST.COM", "TEST@TEST.COM", "AQAAAAEAACcQAAAAEL919QOVRQ84aesECDIRomVp0r9nXn8i4ylOGRHcP6BilXVCyBUvzbyuwLsCxI0THg==", null, false, "1ad309b0-7c57-42d1-aa36-f5d0dab87e72", false, "test@test.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "4cddd91b-03fd-499c-b899-2523eaa1bf40", "171f313b-b676-44c6-9994-9f809f7aef72" },
                    { "9beb52fd-570d-42df-b966-d17da8ffc431", "171f313b-b676-44c6-9994-9f809f7aef72" },
                    { "4cddd91b-03fd-499c-b899-2523eaa1bf40", "8fe5f090-84ff-4caa-9519-177fb86ef973" },
                    { "9beb52fd-570d-42df-b966-d17da8ffc431", "8fe5f090-84ff-4caa-9519-177fb86ef973" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
