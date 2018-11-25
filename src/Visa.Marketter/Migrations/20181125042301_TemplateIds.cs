using Microsoft.EntityFrameworkCore.Migrations;

namespace Visa.Marketter.Migrations
{
    public partial class TemplateIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "card_type",
                table: "campaigns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "template_id",
                table: "campaigns",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "card_type",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "template_id",
                table: "campaigns");
        }
    }
}
