using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Visa.Marketter.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    client_name = table.Column<string>(maxLength: 256, nullable: true),
                    client_secret_salt = table.Column<byte[]>(nullable: true),
                    client_secret_hash = table.Column<byte[]>(nullable: true),
                    trusted = table.Column<bool>(nullable: false),
                    product_name = table.Column<string>(maxLength: 256, nullable: true),
                    copyright_text = table.Column<string>(maxLength: 256, nullable: true),
                    sender = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 256, nullable: true),
                    email = table.Column<string>(maxLength: 256, nullable: true),
                    card_number = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 256, nullable: true),
                    email = table.Column<string>(maxLength: 256, nullable: true),
                    password_salt = table.Column<byte[]>(nullable: true),
                    password_hash = table.Column<byte[]>(nullable: true),
                    password_set_at = table.Column<DateTimeOffset>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    updated_at = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pacs",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    userid = table.Column<int>(nullable: true),
                    name = table.Column<string>(maxLength: 256, nullable: true),
                    password_salt = table.Column<byte[]>(nullable: true),
                    password_hash = table.Column<byte[]>(nullable: true),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    updated_at = table.Column<DateTimeOffset>(nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacs", x => x.id);
                    table.ForeignKey(
                        name: "FK_pacs_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    exchange_code_salt = table.Column<byte[]>(nullable: true),
                    exchange_code_hash = table.Column<byte[]>(nullable: true),
                    refresh_token_salt = table.Column<byte[]>(nullable: true),
                    refresh_token_hash = table.Column<byte[]>(nullable: true),
                    user_id = table.Column<int>(nullable: false),
                    client_id = table.Column<Guid>(nullable: true),
                    clientid = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    userid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_clients_clientid",
                        column: x => x.clientid,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pacs_userid",
                table: "pacs",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_clientid",
                table: "refresh_tokens",
                column: "clientid");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_userid",
                table: "refresh_tokens",
                column: "userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer");

            migrationBuilder.DropTable(
                name: "pacs");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
