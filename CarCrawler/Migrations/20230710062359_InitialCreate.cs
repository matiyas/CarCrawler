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
                    SellerPhones = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    VIN = table.Column<string>(type: "TEXT", nullable: true),
                    Year = table.Column<string>(type: "TEXT", nullable: true),
                    TravelDuration = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    TravelDistance = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleHistoryReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdDetailsId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfOwnersInTheCountry = table.Column<int>(type: "INTEGER", nullable: true),
                    FirstRegistrationAbroad = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    FirstRegistrationInTheCountry = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleHistoryReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleHistoryReports_AdDetails_AdDetailsId",
                        column: x => x.AdDetailsId,
                        principalTable: "AdDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleHistoryReports_AdDetailsId",
                table: "VehicleHistoryReports",
                column: "AdDetailsId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleHistoryReports");

            migrationBuilder.DropTable(
                name: "AdDetails");
        }
    }
}
