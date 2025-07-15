using LibraryManagement.Infrastructure.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILoggerFactory _loggerFactory;

        public LogController(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        [HttpGet]
        public IActionResult GetLogs()
        {
            var provider = _loggerFactory as CustomInMemoryLoggerProvider;
            if (provider == null) return NotFound("Logging provider not found");
            var logs = provider.GetLogs();
            return Ok(logs);
        }
    }
}
