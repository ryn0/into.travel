using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IntoTravel.Data.Migrations
{
    public partial class ContentSnippets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentSnippet",
                columns: table => new
                {
                    ContentSnippetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SnippetType = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentSnippet", x => x.ContentSnippetId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnippet_SnippetType",
                table: "ContentSnippet",
                column: "SnippetType",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentSnippet");
        }
    }
}
