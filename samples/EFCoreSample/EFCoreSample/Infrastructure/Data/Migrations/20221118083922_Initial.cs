using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EFCoreSample.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "IsPublic", "Name", "Price", "TenantId" },
                values: new object[,]
                {
                    { new Guid("2fedacbf-bdd0-4083-8c98-8b97f6257663"), false, "Dell XPS 15", 2300.0m, new Guid("26b27fd0-6507-42e1-9674-b777136d2256") },
                    { new Guid("51e2887c-8442-48db-8e27-026638692b7e"), true, "Dell Precision", 4000.0m, new Guid("26b27fd0-6507-42e1-9674-b777136d2256") },
                    { new Guid("9099eaf9-1e11-43f8-aebe-7dad26e53208"), true, "iPhone", 1200.0m, new Guid("f2ae191d-8708-4ecb-9c53-f93b668a1744") },
                    { new Guid("ab73d4b5-03c3-4fc4-8135-10773df6d757"), false, "AirPods", 600.0m, new Guid("f2ae191d-8708-4ecb-9c53-f93b668a1744") },
                    { new Guid("d9ee475d-eec4-4731-a638-37c4c36fca58"), false, "Dell Monitor", 800.0m, new Guid("26b27fd0-6507-42e1-9674-b777136d2256") },
                    { new Guid("ec5f6fff-4def-4b93-b3f6-392fb4487d39"), false, "iPad", 700.0m, new Guid("f2ae191d-8708-4ecb-9c53-f93b668a1744") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
