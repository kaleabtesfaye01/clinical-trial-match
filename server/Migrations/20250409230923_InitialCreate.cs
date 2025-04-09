using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;

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
                    NctId = table.Column<string>(type: "text", nullable: false),
                    BriefTitle = table.Column<string>(type: "text", nullable: true),
                    OverallStatus = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<string>(type: "text", nullable: true),
                    StudyFirstSubmitDate = table.Column<string>(type: "text", nullable: true),
                    StudyFirstPostDate = table.Column<string>(type: "text", nullable: true),
                    LastUpdateSubmitDate = table.Column<string>(type: "text", nullable: true),
                    LastUpdatePostDate = table.Column<string>(type: "text", nullable: true),
                    BriefSummary = table.Column<string>(type: "text", nullable: true),
                    Conditions = table.Column<string>(type: "jsonb", nullable: true),
                    Keywords = table.Column<string>(type: "jsonb", nullable: true),
                    StudyType = table.Column<string>(type: "text", nullable: true),
                    Phases = table.Column<string>(type: "jsonb", nullable: true),
                    EligibilityCriteria = table.Column<string>(type: "text", nullable: true),
                    HealthyVolunteers = table.Column<string>(type: "text", nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    MinimumAge = table.Column<string>(type: "text", nullable: true),
                    MaximumAge = table.Column<string>(type: "text", nullable: true),
                    min_age_months = table.Column<int>(type: "integer", nullable: true),
                    max_age_months = table.Column<int>(type: "integer", nullable: true),
                    VectorizedData = table.Column<Vector>(type: "vector(1536)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalTrials", x => x.NctId);
                });

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ClinicalTrialNctId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interventions_ClinicalTrials_ClinicalTrialNctId",
                        column: x => x.ClinicalTrialNctId,
                        principalTable: "ClinicalTrials",
                        principalColumn: "NctId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Zip = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    ClinicalTrialNctId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_ClinicalTrials_ClinicalTrialNctId",
                        column: x => x.ClinicalTrialNctId,
                        principalTable: "ClinicalTrials",
                        principalColumn: "NctId",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.DropTable(
                name: "ClinicalTrials");
        }
    }
}
