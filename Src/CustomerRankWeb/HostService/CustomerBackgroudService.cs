namespace CustomerRankWeb.Services
{
    public class CustomerBackgroudService : IHostedService

    {
        private readonly CustomerRankService _customerRankService;
        public CustomerBackgroudService(CustomerRankService customerRankService)
        {
            _customerRankService = customerRankService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _customerRankService.StartHandleLeaderBoardAsync(cancellationToken);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
