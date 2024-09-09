using System.Collections.Concurrent;

namespace CustomerRankWeb.Services
{
    public class CustomerStoreService
    {
        private readonly ConcurrentDictionary<long, Customer> Customers;

        public CustomerStoreService()
        {
            Customers = new ConcurrentDictionary<long, Customer>();
        }
        public Customer AddOrUpdateCustomer(Customer customer, decimal score)
        {
            return Customers.AddOrUpdate(customer.Id, customer,
    (customerName, existingCustomer) =>
    {
        existingCustomer.Score += score;
        return existingCustomer;
    });
        }

        public IEnumerable<Customer> GetNeedRankCustomers()
        {
            return Customers.Values.Where(c => c.Score > 0)
                .Select(x => (Customer)x.Clone());
        }
    }
}
