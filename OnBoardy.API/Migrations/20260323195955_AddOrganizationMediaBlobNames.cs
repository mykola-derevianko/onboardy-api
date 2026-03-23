using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnBoardy.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationMediaBlobNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "banner_blob_name",
                table: "organizations",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "logo_blob_name",
                table: "organizations",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "banner_blob_name",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "logo_blob_name",
                table: "organizations");
        }
    }
}
