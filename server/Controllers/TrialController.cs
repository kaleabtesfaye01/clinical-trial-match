using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using _.Data;
using _.Models;
using System.Linq;

namespace _.Controllers
{
    /// <summary>
    /// Controller for managing clinical trials.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TrialsController"/> class.
    /// </remarks>
    /// <param name="context">The database context for clinical trials.</param>
    [ApiController]
    [Route("api/[controller]")]
    public class TrialsController(TrialsContext context) : ControllerBase
    {
        private readonly TrialsContext _context = context;

        /// <summary>
        /// Retrieves a paginated list of clinical trials.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of records per page (default is 10).</param>
        /// <returns>A paginated list of clinical trials, including total record count.</returns>
        /// <response code="200">Returns the paginated list of trials.</response>
        /// <response code="400">If the page number or page size is invalid.</response>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTrialsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            var totalRecords = await _context.ClinicalTrials.CountAsync();

            var trials = await _context.ClinicalTrials
                .Include(ct => ct.Interventions)
                .Include(ct => ct.Locations)
                .OrderBy(ct => ct.NctId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Trials = trials
            });
        }
    }
}