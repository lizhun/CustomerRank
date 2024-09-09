using CustomerRankWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRankWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {

        private readonly CustomerRankService _CustomerRankService;

        private readonly ILogger<LeaderboardController> _logger;

        public LeaderboardController(ILogger<LeaderboardController> logger, CustomerRankService customerRankService)
        {
            _logger = logger;
            _CustomerRankService = customerRankService;
        }

        [HttpGet("")]
        public IEnumerable<LeaderboardItem> Get([FromQuery] int start, [FromQuery] int end)
        {
            return _CustomerRankService.GetRankCustomers(start, end);
        }

        [HttpGet("{customerid}")]
        public IEnumerable<LeaderboardItem> GetByCustomerid([FromRoute] long customerid, [FromQuery] int high, [FromQuery] int low)
        {
            return _CustomerRankService.GetRankCustomers(customerid, high, low);
        }
    }
}
