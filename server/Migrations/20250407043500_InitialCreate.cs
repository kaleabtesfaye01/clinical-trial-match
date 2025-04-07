using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClinicalTrials",
                columns: table => new
                {
                    NctId = table.Column<string>(type: "TEXT", nullable: false),
                    BriefTitle = table.Column<string>(type: "TEXT", nullable: true),
                    OverallStatus = table.Column<string>(type: "TEXT", nullable: true),
                    BriefSummary = table.Column<string>(type: "TEXT", nullable: true),
                    Conditions = table.Column<string>(type: "TEXT", nullable: true),
                    Keywords = table.Column<string>(type: "TEXT", nullable: true),
                    StudyType = table.Column<string>(type: "TEXT", nullable: true),
                    EligibilityCriteria = table.Column<string>(type: "TEXT", nullable: true),
                    HealthyVolunteers = table.Column<string>(type: "TEXT", nullable: true),
                    Sex = table.Column<string>(type: "TEXT", nullable: true),
                    GenderBased = table.Column<string>(type: "TEXT", nullable: true),
                    GenderDescription = table.Column<string>(type: "TEXT", nullable: true),
                    MinimumAge = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumAge = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<string>(type: "TEXT", nullable: true),
                    StudyFirstSubmitDate = table.Column<string>(type: "TEXT", nullable: true),
                    StudyFirstPostDate = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdateSubmitDate = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdatePostDate = table.Column<string>(type: "TEXT", nullable: true),
                    Phases = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalTrials", x => x.NctId);
                });

            migrationBuilder.CreateTable(
                name: "Intervention",
                columns: table => new
                {
                    ClinicalTrialNctId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
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
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Zip = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Intervention");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "ClinicalTrials");
        }
    }
}
