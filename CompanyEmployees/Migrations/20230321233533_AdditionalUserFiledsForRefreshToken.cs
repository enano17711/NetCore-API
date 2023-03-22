using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalUserFiledsForRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b9ba149-aee4-40c4-8c6f-ac93aebd049d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f15f6be-00af-4060-9c98-a409274caa5f");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "85bb0288-cbc7-4b73-be48-6a069e6b57ed", "44b1aca2-4fe7-400e-9898-94ff7fea70b5", "Manager", "MANAGER" },
                    { "c6467500-2f9a-4444-bc74-7f77d17a11e6", "1eef4103-4d52-4e2e-bb86-9fd2a80501f7", "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "85bb0288-cbc7-4b73-be48-6a069e6b57ed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c6467500-2f9a-4444-bc74-7f77d17a11e6");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6b9ba149-aee4-40c4-8c6f-ac93aebd049d", "951d6da7-855e-49aa-a976-1a3ffd10ea64", "Administrator", "ADMINISTRATOR" },
                    { "9f15f6be-00af-4060-9c98-a409274caa5f", "d2a22bd5-30e2-44d7-9fe9-5867144da39e", "Manager", "MANAGER" }
                });
        }
    }
}
