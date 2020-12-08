﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace CloudPhoto.Data.Migrations
{
    public partial class AddUserPayPalEmailAdress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastNae",
                table: "AspNetUsers",
                newName: "PayPalEmail");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PayPalEmail",
                table: "AspNetUsers",
                newName: "LastNae");
        }
    }
}
