using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations;

[Migration("20260903090000_AddMediaFileContent")]
public partial class AddMediaFileContent : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "FileContent",
            table: "Media",
            type: "BLOB",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "MediaId",
            table: "SocialVideos",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Platform",
            table: "SocialVideos",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "FileContent", table: "Media");
        migrationBuilder.DropColumn(name: "MediaId", table: "SocialVideos");
        migrationBuilder.DropColumn(name: "Platform", table: "SocialVideos");
    }
}
