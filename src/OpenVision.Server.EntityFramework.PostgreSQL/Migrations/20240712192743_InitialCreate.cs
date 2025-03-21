using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Web.Api.EntityFramework.PostgreSQL.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Databases",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Databases", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ApiKeys",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DatabaseId = table.Column<Guid>(type: "uuid", nullable: false),
                Key = table.Column<string>(type: "text", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApiKeys", x => x.Id);
                table.ForeignKey(
                    name: "FK_ApiKeys_Databases_DatabaseId",
                    column: x => x.DatabaseId,
                    principalTable: "Databases",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ImageTargets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DatabaseId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                PreprocessImage = table.Column<byte[]>(type: "bytea", nullable: false),
                AfterProcessImage = table.Column<byte[]>(type: "bytea", nullable: false),
                AfterProcessImageWithKeypoints = table.Column<byte[]>(type: "bytea", nullable: false),
                Keypoints = table.Column<byte[]>(type: "bytea", nullable: false),
                Descriptors = table.Column<byte[]>(type: "bytea", nullable: false),
                DescriptorsRows = table.Column<int>(type: "integer", nullable: false),
                DescriptorsCols = table.Column<int>(type: "integer", nullable: false),
                Width = table.Column<float>(type: "real", nullable: false),
                Height = table.Column<float>(type: "real", nullable: false),
                Recos = table.Column<int>(type: "integer", nullable: false),
                Rating = table.Column<int>(type: "integer", nullable: false),
                Metadata = table.Column<string>(type: "text", nullable: true),
                Type = table.Column<int>(type: "integer", nullable: false),
                ActiveFlag = table.Column<int>(type: "integer", nullable: false),
                Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ImageTargets", x => x.Id);
                table.ForeignKey(
                    name: "FK_ImageTargets_Databases_DatabaseId",
                    column: x => x.DatabaseId,
                    principalTable: "Databases",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ApiKeys_DatabaseId",
            table: "ApiKeys",
            column: "DatabaseId");

        migrationBuilder.CreateIndex(
            name: "IX_ImageTargets_DatabaseId",
            table: "ImageTargets",
            column: "DatabaseId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ApiKeys");

        migrationBuilder.DropTable(
            name: "ImageTargets");

        migrationBuilder.DropTable(
            name: "Databases");
    }
}
