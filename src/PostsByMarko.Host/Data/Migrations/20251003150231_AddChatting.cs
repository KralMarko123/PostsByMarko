using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PostsByMarko.Host.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChatting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    ProfilePicture = table.Column<byte[]>(type: "bytea", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ParticipantIds = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SenderId = table.Column<string>(type: "text", nullable: false),
                    ChatId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "13b641a9-5875-4974-b1c9-645e98288f70", null, "Admin", "ADMIN" },
                    { "fc516090-f789-4cdc-96f0-e760d98faae9", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "04a8917f-d72b-4731-9f96-9485a7174838", 0, "6cac8784-fffd-4fbe-a559-6c170f855a14", new DateTime(2025, 10, 20, 0, 38, 27, 613, DateTimeKind.Utc).AddTicks(4785), "Kay.Kiehn43@yahoo.com", true, "Nicklaus", "Schaefer", false, null, "KAY.KIEHN43@YAHOO.COM", "KAY.KIEHN43@YAHOO.COM", "AQAAAAIAAYagAAAAEH9Nm42WKB8xWX3JnPCMNbclOGleWLDMU4VyjJb3I4TkdgsQvnagYDr/dn+Um5osJw==", null, false, null, "d5e486a5-0d31-49e0-94e5-605baf3e89b7", false, "Kay.Kiehn43@yahoo.com" },
                    { "1ed4a5bc-23bb-47dd-b295-68a35d63ed21", 0, "ffd93b23-8e22-40b2-a083-2984e8834655", new DateTime(2025, 10, 4, 18, 24, 47, 37, DateTimeKind.Utc).AddTicks(9953), "Edythe.Bergnaum@yahoo.com", true, "Dana", "Zboncak", false, null, "EDYTHE.BERGNAUM@YAHOO.COM", "EDYTHE.BERGNAUM@YAHOO.COM", "AQAAAAIAAYagAAAAEIzPRzvlz+q7LikR7/HCOMDpQ6yxkY7tKkw0bo5lKRBhpWfUp/jsfg6BWkf0G//yuw==", null, false, null, "8174f4bc-20c6-40e0-993d-3f52ee15dc24", false, "Edythe.Bergnaum@yahoo.com" },
                    { "25a4ec9f-50dd-44fc-89d1-a7685e022306", 0, "ac407112-267a-49b5-aaf5-b0b737536ca8", new DateTime(2025, 10, 3, 15, 2, 30, 790, DateTimeKind.Utc).AddTicks(1623), "kralmarko123@gmail.com", true, "Marko", "Markovikj", false, null, "KRALMARKO123@GMAIL.COM", "KRALMARKO123@GMAIL.COM", "AQAAAAIAAYagAAAAEPhq9cuTHMDurviBNXkjz5czSY/BzqqXbehWeVZPxKrs61nr/S2yunJ+OgnOhZMWnw==", null, false, null, "f9d4fe9a-dc1a-468d-bb61-ef5db3b24e04", false, "kralmarko123@gmail.com" },
                    { "319ad6ef-e205-40f6-8572-f07a91545682", 0, "57f50ae6-38d4-4be5-b594-bacb0bf651c8", new DateTime(2025, 10, 3, 15, 2, 30, 790, DateTimeKind.Utc).AddTicks(3032), "test@test.com", true, "Test", "User", false, null, "TEST@TEST.COM", "TEST@TEST.COM", "AQAAAAIAAYagAAAAEPwwTxWkZd4bOIOWSlaOVyhKuoy7l0snDPsoXlxdeDvE8dAFMepiqzMEkaKS+eC+kg==", null, false, null, "ea1d4f9c-d69a-44f6-bf00-51b34ae63ee6", false, "test@test.com" },
                    { "b2b5b822-cb50-426e-a504-6f4915f21273", 0, "d84c6432-4000-42f0-80c0-7113cdfa3f48", new DateTime(2025, 10, 16, 6, 22, 56, 391, DateTimeKind.Utc).AddTicks(6378), "Carol_Fritsch85@hotmail.com", true, "Garrick", "Padberg", false, null, "CAROL_FRITSCH85@HOTMAIL.COM", "CAROL_FRITSCH85@HOTMAIL.COM", "AQAAAAIAAYagAAAAEJZ5EY0SjZbMiQXc6hPbi1hbVaRLn1goJjfkKlBV6PhUN+9VSKK8M/qLNCtT0Ccs2w==", null, false, null, "e79edb56-6a8c-4284-b4d0-1cb32d399b48", false, "Carol_Fritsch85@hotmail.com" },
                    { "cdf29e06-135e-499b-b905-e8e874fcb8e8", 0, "17dc81ed-3495-4753-92eb-47d1d10b8bd8", new DateTime(2025, 10, 24, 8, 11, 12, 436, DateTimeKind.Utc).AddTicks(6456), "Mae.Ferry@gmail.com", true, "Elisha", "Wintheiser", false, null, "MAE.FERRY@GMAIL.COM", "MAE.FERRY@GMAIL.COM", "AQAAAAIAAYagAAAAEBFOrtMix++xycJrig9ICb2L3lwUj7KulGtGwt/13+C3nLRP6TNWzEFjGiWP6wRPXg==", null, false, null, "e0600719-fe70-4ffd-b362-3e7f1c78d9a0", false, "Mae.Ferry@gmail.com" },
                    { "f8686982-ee4d-499e-a4e0-4c0959937b3a", 0, "ecc057b3-2d73-442f-9da5-43a0d567819f", new DateTime(2025, 10, 13, 19, 9, 23, 519, DateTimeKind.Utc).AddTicks(5260), "Naomi_Gusikowski@hotmail.com", true, "Elmo", "Heidenreich", false, null, "NAOMI_GUSIKOWSKI@HOTMAIL.COM", "NAOMI_GUSIKOWSKI@HOTMAIL.COM", "AQAAAAIAAYagAAAAEPVE9MApcJ162eIYCStw6hSrx45QC88RReamqcKSL+38Yb1MNlaRlVODPuvyJfvGTQ==", null, false, null, "10b83614-9a11-4fe8-8989-dd4ff7814fe0", false, "Naomi_Gusikowski@hotmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "fc516090-f789-4cdc-96f0-e760d98faae9", "04a8917f-d72b-4731-9f96-9485a7174838" },
                    { "fc516090-f789-4cdc-96f0-e760d98faae9", "1ed4a5bc-23bb-47dd-b295-68a35d63ed21" },
                    { "13b641a9-5875-4974-b1c9-645e98288f70", "25a4ec9f-50dd-44fc-89d1-a7685e022306" },
                    { "fc516090-f789-4cdc-96f0-e760d98faae9", "25a4ec9f-50dd-44fc-89d1-a7685e022306" },
                    { "fc516090-f789-4cdc-96f0-e760d98faae9", "319ad6ef-e205-40f6-8572-f07a91545682" },
                    { "fc516090-f789-4cdc-96f0-e760d98faae9", "b2b5b822-cb50-426e-a504-6f4915f21273" },
                    { "fc516090-f789-4cdc-96f0-e760d98faae9", "cdf29e06-135e-499b-b905-e8e874fcb8e8" },
                    { "fc516090-f789-4cdc-96f0-e760d98faae9", "f8686982-ee4d-499e-a4e0-4c0959937b3a" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedDate", "IsHidden", "LastUpdatedDate", "Title" },
                values: new object[,]
                {
                    { "0852fb74-bc2f-4689-bc4d-69429f7e1463", "b2b5b822-cb50-426e-a504-6f4915f21273", "My co-worker Reed has one of these. He says it looks microscopic.", new DateTime(2025, 10, 3, 15, 36, 2, 450, DateTimeKind.Utc).AddTicks(1862), false, new DateTime(2025, 10, 5, 19, 36, 37, 176, DateTimeKind.Utc).AddTicks(6662), "Practical Concrete Pants" },
                    { "098d0b22-f237-48d4-a031-5f1c6fd95ccf", "cdf29e06-135e-499b-b905-e8e874fcb8e8", "My co-worker Merwin has one of these. He says it looks bubbly.", new DateTime(2025, 10, 25, 0, 59, 29, 674, DateTimeKind.Utc).AddTicks(3392), true, new DateTime(2025, 10, 28, 11, 57, 4, 340, DateTimeKind.Utc).AddTicks(6149), "Rustic Wooden Shirt" },
                    { "3e56dd78-23ed-4452-8b89-1c0c8c41ccf3", "04a8917f-d72b-4731-9f96-9485a7174838", "this Incredible Metal Shirt is hyper.", new DateTime(2025, 10, 12, 19, 53, 38, 718, DateTimeKind.Utc).AddTicks(5090), true, new DateTime(2025, 10, 20, 9, 35, 40, 849, DateTimeKind.Utc).AddTicks(6353), "Licensed Steel Computer" },
                    { "9553c5f8-0a61-4487-a318-7fb4cce40d53", "f8686982-ee4d-499e-a4e0-4c0959937b3a", "My co-worker Linnie has one of these. He says it looks wide.", new DateTime(2025, 10, 25, 19, 46, 41, 56, DateTimeKind.Utc).AddTicks(9295), true, new DateTime(2025, 10, 31, 20, 16, 22, 206, DateTimeKind.Utc).AddTicks(9018), "Sleek Wooden Towels" },
                    { "99e08326-9fe0-41ec-b2dd-902486e86e66", "b2b5b822-cb50-426e-a504-6f4915f21273", "one of my hobbies is baking. and when i'm baking this works great.", new DateTime(2025, 10, 22, 1, 28, 22, 932, DateTimeKind.Utc).AddTicks(3384), false, new DateTime(2025, 10, 6, 6, 21, 12, 455, DateTimeKind.Utc).AddTicks(2984), "Handmade Steel Shoes" },
                    { "ae336be4-74f1-4199-8148-700931729d79", "04a8917f-d72b-4731-9f96-9485a7174838", "This Handcrafted Soft Shirt works considerably well. It mildly improves my basketball by a lot.", new DateTime(2025, 10, 8, 17, 28, 4, 827, DateTimeKind.Utc).AddTicks(9228), false, new DateTime(2025, 10, 31, 14, 32, 24, 277, DateTimeKind.Utc).AddTicks(1013), "Small Rubber Soap" },
                    { "b6724dca-3eaf-45f3-862a-b4aa1eb69c35", "1ed4a5bc-23bb-47dd-b295-68a35d63ed21", "talk about bliss!!", new DateTime(2025, 10, 27, 1, 28, 35, 889, DateTimeKind.Utc).AddTicks(505), true, new DateTime(2025, 10, 20, 20, 59, 46, 678, DateTimeKind.Utc).AddTicks(6577), "Tasty Metal Ball" },
                    { "d37ec644-c72f-47c9-9395-24ce59ec4da6", "cdf29e06-135e-499b-b905-e8e874fcb8e8", "i use it until further notice when i'm in my nightclub.", new DateTime(2025, 10, 23, 17, 58, 50, 482, DateTimeKind.Utc).AddTicks(9045), false, new DateTime(2025, 10, 3, 21, 26, 56, 734, DateTimeKind.Utc).AddTicks(5583), "Gorgeous Soft Mouse" },
                    { "f4071906-6663-4cbf-85e1-a996f041e19e", "1ed4a5bc-23bb-47dd-b295-68a35d63ed21", "I tried to scratch it but got cheeseburger all over it.", new DateTime(2025, 10, 31, 12, 28, 2, 252, DateTimeKind.Utc).AddTicks(7494), false, new DateTime(2025, 10, 3, 22, 9, 54, 929, DateTimeKind.Utc).AddTicks(9574), "Gorgeous Concrete Shoes" },
                    { "f4379633-226c-4af4-b139-b0c37463da7c", "f8686982-ee4d-499e-a4e0-4c0959937b3a", "i use it from now on when i'm in my safehouse.", new DateTime(2025, 10, 4, 4, 9, 48, 464, DateTimeKind.Utc).AddTicks(8755), false, new DateTime(2025, 10, 16, 10, 51, 41, 748, DateTimeKind.Utc).AddTicks(3208), "Handmade Soft Tuna" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
