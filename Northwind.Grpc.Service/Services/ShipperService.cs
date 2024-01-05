using Grpc.Core; // ServerCallContext
using ALLinONE.Shared; // NorthwindContext
using ShipperEntity = ALLinONE.Shared.Shipper;

namespace Northwind.Grpc.Service.Services
{
    public class ShipperService : Shipper.ShipperBase
    {
        protected readonly ILogger<ShipperService> _logger;
        protected readonly NorthwindContext db;

        public ShipperService(ILogger<ShipperService> logger, NorthwindContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public override async Task<ShipperReply?> GetShipper(ShipperRequest request, ServerCallContext context)
        {
            _logger.LogWarning("This request has a deadline of {0:T}. It is now {1:T}.", context.Deadline, DateTime.UtcNow);

            // Sprawdź, czy wywołanie RPC zostało anulowane
            if(context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("The RPC call was cancelled before it could complete.");
                return null;
            }

            //await Task.Delay(TimeSpan.FromSeconds(5));

            //// Ponownie sprawdź, czy wywołanie RPC zostało anulowane
            //if(context.CancellationToken.IsCancellationRequested)
            //{
            //    _logger.LogCritical("The RPC call was cancelled before it could complete.");
            //    return null;
            //}

            //return ToShipperReply(await db.Shippers.FindAsync(request.ShipperId));

            
            ShipperEntity? shipper = await db.Shippers.FindAsync(request.ShipperId);

            if (shipper == null) 
            {
                _logger.LogWarning($"Shipper with ID {request.ShipperId} not found.");
                return null;
            }
            else
            {
                return ToShipperReply(shipper);
            }
            
        }

        private ShipperReply ToShipperReply(ShipperEntity shipper)
        {
            return new ShipperReply
            {
                ShipperId = shipper.ShipperId,
                CompanyName = shipper.CompanyName,
                Phone = shipper.Phone
            };
        }
    }
}
