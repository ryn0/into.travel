using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IntoTravel.Data.Migrations
{
    public partial class NoRequiredDetailsOnPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BlogEntryPhoto",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BlogEntryPhoto",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BlogEntryPhoto",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BlogEntryPhoto",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
