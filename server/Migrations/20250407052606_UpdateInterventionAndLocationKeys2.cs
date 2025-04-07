using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInterventionAndLocationKeys2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Intervention");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ClinicalTrialNctId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interventions_ClinicalTrials_ClinicalTrialNctId",
                        column: x => x.ClinicalTrialNctId,
                        principalTable: "ClinicalTrials",
                        principalColumn: "NctId");
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Zip = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    ClinicalTrialNctId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_ClinicalTrials_ClinicalTrialNctId",
                        column: x => x.ClinicalTrialNctId,
                        principalTable: "ClinicalTrials",
                        principalColumn: "NctId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_ClinicalTrialNctId",
                table: "Interventions",
                column: "ClinicalTrialNctId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ClinicalTrialNctId",
                table: "Locations",
                column: "ClinicalTrialNctId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.CreateTable(
                name: "Intervention",
                columns: table => new
                {
                    ClinicalTrialNctId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervention", x => new { x.ClinicalTrialNctId, x.Id });
                    table.ForeignKey(
                        name: "FK_Intervention_ClinicalTrials_ClinicalTrialNctId",
                        column: x => x.ClinicalTrialNctId,
                        principalTable: "ClinicalTrials",
                        principalColumn: "NctId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    ClinicalTrialNctId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Zip = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => new { x.ClinicalTrialNctId, x.Id });
                    table.ForeignKey(
                        name: "FK_Location_ClinicalTrials_ClinicalTrialNctId",
                        column: x => x.ClinicalTrialNctId,
                        principalTable: "ClinicalTrials",
                        principalColumn: "NctId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
