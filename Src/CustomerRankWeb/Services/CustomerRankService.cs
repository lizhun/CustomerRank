using System.Threading.Channels;

namespace CustomerRankWeb.Services
{
    public class CustomerRankService
    {
        private readonly ILogger<CustomerRankService> _logger;

        private readonly CustomerStoreService _CustomerStoreService;
        public CustomerRankService(CustomerStoreService customerStoreService, ILogger<CustomerRankService> logger)
        {
            _logger = logger;
            _CustomerStoreService = customerStoreService;

            //Ranking data storage
            LeaderRankDB = new SortedSet<Customer>();

            // Keeping two triggers is enough. When the ranking calculation time is too long, the latest request will be processed, and previous requests will be discarded directly to avoid invalid calculations
            _CustomerRankChannel = Channel.CreateBounded<DateTime>(new BoundedChannelOptions(2)
            {
                SingleReader = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        }


        private Channel<DateTime> _CustomerRankChannel;

        private SortedSet<Customer> LeaderRankDB = new SortedSet<Customer>();

        public void MakeCustomerRanking()
        {
            var dataList = _CustomerStoreService.GetNeedRankCustomers();
            var LeaderRank = new SortedSet<Customer>();
            foreach (var data in dataList)
            {
                LeaderRank.Add(data);
            }
            LeaderRankDB = LeaderRank;
        }

        //Ranking data processing
        public Task StartHandleLeaderBoardAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
              {
                  while (!_CustomerRankChannel.Reader.Completion.IsCompleted)
                  {
                      var data = await _CustomerRankChannel.Reader.ReadAsync(cancellationToken);
                      _logger.LogInformation($"Read Customer  {data} add or update");

                      MakeCustomerRanking();
                      //await Task.Delay(2000);
                  }
              }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task SendMakeCustomerRankEventAsync(Customer customer)
        {
            //TO DO :log customers info or other things ...
            // _logger.LogInformation($"Send Customer {customer.Id} score {customer.Score} add or update");
            await _CustomerRankChannel.Writer.WriteAsync(DateTime.Now);
        }

        public IList<LeaderboardItem> GetRankCustomers(int start, int offset)
        {
            if (start < 1 || offset < 1)
            {
                return new List<LeaderboardItem>();
            }

            //avoid LeaderRankDB   changed when new customer add or update
            var tempList = LeaderRankDB;
            var item = tempList.ElementAtOrDefault(start - 1);
            if (item != null)
            {
                return tempList.Skip(start - 1).Take(offset)
                    .Select((x, i) => new LeaderboardItem()
                    {
                        Id = x.Id,
                        Score = x.Score,
                        Rank = start + i
                    })
                    .ToList();
            }
            return new List<LeaderboardItem>();
        }

        public async Task<Customer> AddOrUpdateCustomerAsync(Customer customer)
        {
            if (customer.Score > 1000)
            {
                customer.Score = 1000;
            }
            else if (customer.Score < -1000)
            {
                customer.Score = -1000;
            }

            var result = _CustomerStoreService.AddOrUpdateCustomer(customer, customer.Score);

            // send make customer rank to backgroud task
            await SendMakeCustomerRankEventAsync(customer);
            return result;
        }
        public IList<LeaderboardItem> GetRankCustomers(long customersId, int high, int low)
        {
            if (high < 0 || low < 0)
            {
                return new List<LeaderboardItem>();
            }
            var customerIndex = -1;
            var tempList = LeaderRankDB;
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList.ElementAt(i).Id == customersId)
                {
                    customerIndex = i;
                    break;
                }

            }
            if (customerIndex == -1)
            {
                return new List<LeaderboardItem>();
            }
            var startIndex = Math.Max(customerIndex - high, 0) + 1;
            return GetRankCustomers(startIndex, high + low + 1);
        }
    }
}
