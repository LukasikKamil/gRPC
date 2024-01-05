using Grpc.Core; // ServerCallContext
using ALLinONE.Shared; // NorthwindContext
using CustomerEntity = ALLinONE.Shared.Customer;

namespace Northwind.Grpc.Service.Services
{
    public class CustomerService : Customer.CustomerBase
    {
        protected readonly ILogger<CustomerService> _logger;
        protected readonly NorthwindContext db;

        public CustomerService(ILogger<CustomerService> logger, NorthwindContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public override async Task<CustomerReply?> GetCustomer(CustomerRequest request, ServerCallContext context)
        {
            _logger.LogWarning("This request has a deadline of {0:T}. It is now {1:T}.", context.Deadline, DateTime.UtcNow);

            // Sprawdź, czy wywołanie RPC zostało anulowane
            if(context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("The RPC call was cancelled before it could complete.");
                return null;
            }

            CustomerEntity? customer = await db.Customers.FindAsync(request.Country);

            if(customer == null)
            {
                _logger.LogWarning($"Customer with ID {request.Country} not found.");
                return null;
            }
            else
            {
                return ToCustomerReply(customer);
            }
        }

        private CustomerReply? ToCustomerReply(CustomerEntity customer)
        {
            return new CustomerReply
            {
                Country = customer.Country,
                CompanyName = customer.CompanyName,
                ContactName = customer.ContactName,
                Address = customer.Address,
                City = customer.City,
                Phone = customer.Phone
            };
        }
    }
}
