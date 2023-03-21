using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6b9ba149-aee4-40c4-8c6f-ac93aebd049d", "951d6da7-855e-49aa-a976-1a3ffd10ea64", "Administrator", "ADMINISTRATOR" },
                    { "9f15f6be-00af-4060-9c98-a409274caa5f", "d2a22bd5-30e2-44d7-9fe9-5867144da39e", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b9ba149-aee4-40c4-8c6f-ac93aebd049d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f15f6be-00af-4060-9c98-a409274caa5f");
        }
    }
}
