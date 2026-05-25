using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SuperHeroAPIs.Migrations
{
    /// <inheritdoc />
    public partial class SeedSuperHeroes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SuperHeroes",
                columns: new[] { "Id", "FirstName", "LastName", "Name", "Place" },
                values: new object[,]
                {
                    { 1, "Peter", "Parker", "Spider-Man", "New York" },
                    { 2, "Tony", "Stark", "Iron Man", "Malibu" },
                    { 3, "Thor", "Odinson", "Thor", "Asgard" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SuperHeroes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SuperHeroes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SuperHeroes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
