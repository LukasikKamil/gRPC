﻿using Microsoft.AspNetCore.Mvc;
using Northwind.Grpc.Client.Mvc.Models;
using System.Diagnostics;
using Grpc.Net.Client; // GrpcChannel
using Grpc.Net.ClientFactory; // GrpcClientFactory
using Grpc.Core; // AsyncUnaryCall<T>

namespace Northwind.Grpc.Client.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        protected readonly Greeter.GreeterClient greeterClient;
        protected readonly Shipper.ShipperClient shipperClient;
        protected readonly Customer.CustomerClient customerClient;

        public HomeController(ILogger<HomeController> logger, GrpcClientFactory factory)
        {
            _logger = logger;
            greeterClient = factory.CreateClient<Greeter.GreeterClient>("Greeter");
            shipperClient = factory.CreateClient<Shipper.ShipperClient>("Shipper");
            customerClient = factory.CreateClient<Customer.CustomerClient>("Customer");
        }

        public async Task<IActionResult> Index(string name = "Henrietta", int id = 1, string country = "Poland")
        {
            try
            {
                HelloReply reply = await greeterClient.SayHelloAsync(new HelloRequest { Name = name });

                ViewData["greeting"] = "Greeting from gRPC service: " + reply.Message;

                ShipperReply shipperReply = await shipperClient.GetShipperAsync(new ShipperRequest { ShipperId = id });

                // the same call as above but not awaited
                //AsyncUnaryCall<ShipperReply> shipperCall = shipperClient.GetShipperAsync(new ShipperRequest { ShipperId = id }, deadline: DateTime.UtcNow.AddSeconds(3));


                //Metadata metadata = await shipperCall.ResponseHeadersAsync;

                //foreach (Metadata.Entry entry in metadata)
                //{
                //    // not really critical, just doing this to make it easier to see
                //    _logger.LogWarning($"Key: {entry.Key}, Value: {entry.Value}");
                //}

                //ShipperReply shipperReply = await shipperCall.ResponseAsync;

                ViewData["shipper"] = "Shipper from gRPC service: "
                    + $"ID: {shipperReply.ShipperId}, Name: {shipperReply.CompanyName},"
                    + $" Phone: {shipperReply.Phone}.";

                CustomerReply customerReply = await customerClient.GetCustomerAsync(new CustomerRequest { Country = country });

                ViewData["customer"] = $"Customer from gRPC service: " +
                    $"Company: {customerReply.CompanyName}, Name: {customerReply.ContactName}, Address: {customerReply.Address}, {customerReply.City}, Phone: {customerReply.Phone}.";
            }
            //catch (RpcException rpcex) when (rpcex.StatusCode == global::Grpc.Core.StatusCode.DeadlineExceeded)
            //{
            //    _logger.LogWarning("Northwind.Grpc.Service deadline exceeded.");
            //    ViewData["exception"] = rpcex.Message;
            //}
            catch(Exception ex)
            {
                _logger.LogWarning($"Northwind.Grpc.Service is not responding.");
                ViewData["exception"] = ex.Message;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
