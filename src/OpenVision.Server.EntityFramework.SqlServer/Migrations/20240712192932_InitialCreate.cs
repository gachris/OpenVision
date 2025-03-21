using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Web.Api.EntityFramework.SqlServer.Migrations;

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
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Databases", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ApiKeys",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DatabaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DatabaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PreprocessImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                AfterProcessImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                AfterProcessImageWithKeypoints = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                Keypoints = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                Descriptors = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                DescriptorsRows = table.Column<int>(type: "int", nullable: false),
                DescriptorsCols = table.Column<int>(type: "int", nullable: false),
                Width = table.Column<float>(type: "real", nullable: false),
                Height = table.Column<float>(type: "real", nullable: false),
                Recos = table.Column<int>(type: "int", nullable: false),
                Rating = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Type = table.Column<int>(type: "int", nullable: false),
                ActiveFlag = table.Column<int>(type: "int", nullable: false),
                Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
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
