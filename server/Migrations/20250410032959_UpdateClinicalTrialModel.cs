﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClinicalTrialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "ClinicalTrials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "ClinicalTrials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "ClinicalTrials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialTitle",
                table: "ClinicalTrials",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "ClinicalTrials");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "ClinicalTrials");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "ClinicalTrials");

            migrationBuilder.DropColumn(
                name: "OfficialTitle",
                table: "ClinicalTrials");
        }
    }
}
