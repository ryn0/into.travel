using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IntoTravel.Data.Migrations
{
    public partial class BlogProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs");

            migrationBuilder.RenameTable(
                name: "Blogs",
                newName: "BlogEntry");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BlogEntry",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlogPublishDateTimeUtc",
                table: "BlogEntry",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "BlogEntry",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "BlogEntry",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "BlogEntry",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogEntry",
                table: "BlogEntry",
                column: "BlogEntryId");

            migrationBuilder.CreateTable(
                name: "BlogEntryPhoto",
                columns: table => new
                {
                    BlogEntryPhotoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BlogEntryId = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhotoUrl = table.Column<string>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogEntryPhoto", x => x.BlogEntryPhotoId);
                    table.ForeignKey(
                        name: "FK_BlogEntryPhoto_BlogEntry_BlogEntryId",
                        column: x => x.BlogEntryId,
                        principalTable: "BlogEntry",
                        principalColumn: "BlogEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "BlogEntryTag",
                columns: table => new
                {
                    BlogEntryId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogEntryTag", x => new { x.BlogEntryId, x.TagId });
                    table.ForeignKey(
                        name: "FK_BlogEntryTag_BlogEntry_BlogEntryId",
                        column: x => x.BlogEntryId,
                        principalTable: "BlogEntry",
                        principalColumn: "BlogEntryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogEntryTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogEntryPhoto_BlogEntryId",
                table: "BlogEntryPhoto",
                column: "BlogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogEntryTag_TagId",
                table: "BlogEntryTag",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogEntryPhoto");

            migrationBuilder.DropTable(
                name: "BlogEntryTag");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogEntry",
                table: "BlogEntry");

            migrationBuilder.DropColumn(
                name: "BlogPublishDateTimeUtc",
                table: "BlogEntry");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "BlogEntry");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "BlogEntry");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "BlogEntry");

            migrationBuilder.RenameTable(
                name: "BlogEntry",
                newName: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Blogs",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs",
                column: "BlogEntryId");
        }
    }
}
