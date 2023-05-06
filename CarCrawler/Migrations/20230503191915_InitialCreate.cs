using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CarCrawler.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Sqlite:InitSpatialMetaData", true);

            migrationBuilder.CreateTable(
                name: "AdDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: true),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    FuelType = table.Column<int>(type: "INTEGER", nullable: true),
                    ISOCurrencySymbol = table.Column<string>(type: "TEXT", nullable: true),
                    MileageKilometers = table.Column<uint>(type: "INTEGER", nullable: true),
                    Model = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", nullable: true),
                    RegistrationDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "TEXT", nullable: true),
                    SellerCoordinates = table.Column<Point>(type: "POINT", nullable: true),
                    SellerPhones = table.Column<string>(type: "json", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    VIN = table.Column<string>(type: "TEXT", nullable: true),
                    Year = table.Column<string>(type: "TEXT", nullable: true),
                    TravelDuration = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    TravelDistance = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdDetails");
        }
    }
}
