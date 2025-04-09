using ClinicalTrialMatcher.Data;
using ClinicalTrialMatcher.Interfaces;
using ClinicalTrialMatcher.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialMatcher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrialsController(TrialsContext dbContext, ITrialMatchingService matchingService, IClinicalTrialsService trialsService) : ControllerBase
    {
        private readonly TrialsContext _dbContext = dbContext;
        private readonly ITrialMatchingService _matchingService = matchingService;
        private readonly IClinicalTrialsService _trialsService = trialsService;

        /// <summary>
        /// Fetches 1,000 clinical trials from the external API, vectorizes them, and saves them to the database.
        /// </summary>
        [HttpPost("vectorize")]
        public async Task<IActionResult> VectorizeTrials()
        {
            try
            {
                var trials = await _trialsService.FetchAndVectorizeTrialsAsync();
                return Ok(new
                {
                    Message = "Trials vectorized and saved successfully.",
                    Count = trials.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Matches clinical trials based on patient input and returns the top matching trials.
        /// </summary>
        [HttpPost("match")]
        public async Task<IActionResult> MatchTrials([FromBody] PatientInput patientInput)
        {
            if (patientInput == null || string.IsNullOrWhiteSpace(patientInput.Notes))
            {
                return BadRequest("Patient input with notes is required.");
            }

            var matches = await _matchingService.MatchTrialsAsync(patientInput);
            return Ok(matches);
        }
    }
}
