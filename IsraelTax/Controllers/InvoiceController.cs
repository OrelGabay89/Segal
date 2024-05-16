using IsraeliTaxTokenFetcher.Services;
using IsraelTax.Data;
using IsraelTax.Interfaces;
using IsraelTax.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace IsraelTax.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public InvoiceController(ILogger<InvoiceController> logger, IConfiguration configuration, ITokenService tokenService)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("_alive")]
        public IActionResult IsAlive()
        {
            return Ok("API is up and running!");
        }

        [HttpPost("process-data")]
        public async Task<IActionResult> ProcessData([FromBody] InvoiceCSVData data)
        {
            if (data == null)
            {
                _logger.LogError("Received null data");
                return BadRequest("Data cannot be null");
            }
            // Load credentials from configuration
            string clientId = _configuration["AppSettings:DefaultClientId"];
            string secret = _configuration["AppSettings:DefaultSecret"];

            var credentials = await _context.ClientCredentials.FirstOrDefaultAsync();
            if (credentials == null)
            {
                _logger.LogError("No credentials found in database");
                return StatusCode(500, "Server error: credentials not found");
            }

            // Use credentials.ClientId and credentials.Secret as needed
            _logger.LogInformation($"Data processed for {data.Id}");
            return Ok($"Data received with ID: {data.Id}");
        }
    }
}