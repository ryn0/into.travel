using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IntoTravel.Data.Migrations
{
    public partial class TagKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tag",
                maxLength: 75,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Tag",
                maxLength: 75,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "Tag");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tag",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 75,
                oldNullable: true);
        }
    }
}
