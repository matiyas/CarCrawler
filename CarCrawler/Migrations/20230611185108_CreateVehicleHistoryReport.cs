using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarCrawler.Migrations
{
    public partial class CreateVehicleHistoryReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Sqlite:InitSpatialMetaData", true);

            migrationBuilder.CreateTable(
                name: "VehicleHistoryReport",
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
                    table.PrimaryKey("PK_VehicleHistoryReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleHistoryReport_AdDetails_AdDetailsId",
                        column: x => x.AdDetailsId,
                        principalTable: "AdDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleHistoryReport_AdDetailsId",
                table: "VehicleHistoryReport",
                column: "AdDetailsId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleHistoryReport");
        }
    }
}
