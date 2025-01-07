using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindfulEase.Data.Migrations
{
    public partial class AddTherapistAtributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReputationPoints",
                table: "AspNetUsers",
                newName: "NumberOfReviews");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTherapist",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Studies",
                table: "AspNetUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsTherapist",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Studies",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "NumberOfReviews",
                table: "AspNetUsers",
                newName: "ReputationPoints");
        }
    }
}
