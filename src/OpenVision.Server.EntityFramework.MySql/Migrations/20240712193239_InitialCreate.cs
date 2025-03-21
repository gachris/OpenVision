using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision.Web.Api.EntityFramework.MySql.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Databases",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false),
                UserId = table.Column<string>(type: "longtext", nullable: false),
                Name = table.Column<string>(type: "longtext", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Updated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Databases", x => x.Id);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "ApiKeys",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false),
                DatabaseId = table.Column<Guid>(type: "char(36)", nullable: false),
                Key = table.Column<string>(type: "longtext", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Updated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
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
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "ImageTargets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false),
                DatabaseId = table.Column<Guid>(type: "char(36)", nullable: false),
                Name = table.Column<string>(type: "longtext", nullable: false),
                PreprocessImage = table.Column<byte[]>(type: "longblob", nullable: false),
                AfterProcessImage = table.Column<byte[]>(type: "longblob", nullable: false),
                AfterProcessImageWithKeypoints = table.Column<byte[]>(type: "longblob", nullable: false),
                Keypoints = table.Column<byte[]>(type: "longblob", nullable: false),
                Descriptors = table.Column<byte[]>(type: "longblob", nullable: false),
                DescriptorsRows = table.Column<int>(type: "int", nullable: false),
                DescriptorsCols = table.Column<int>(type: "int", nullable: false),
                Width = table.Column<float>(type: "float", nullable: false),
                Height = table.Column<float>(type: "float", nullable: false),
                Recos = table.Column<int>(type: "int", nullable: false),
                Rating = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "longtext", nullable: true),
                Type = table.Column<int>(type: "int", nullable: false),
                ActiveFlag = table.Column<int>(type: "int", nullable: false),
                Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Updated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
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
            })
            .Annotation("MySQL:Charset", "utf8mb4");

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
