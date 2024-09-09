using CustomerRankWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRankWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerRankService _CustomerRankService;

        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger, CustomerRankService customerRankService)
        {
            _logger = logger;
            _CustomerRankService = customerRankService;
        }

        [HttpPost("{customerid}/score/{score}")]
        public async Task<Customer> AddOrUpdateCustomerScore(int customerid, decimal score)
        {
            var customer = new Customer() { Id = customerid, Score = score };
            return await _CustomerRankService.AddOrUpdateCustomerAsync(customer);
        }
    }
}
