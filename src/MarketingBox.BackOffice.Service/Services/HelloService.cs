using System.Threading.Tasks;
using MarketingBox.BackOffice.Service.Grpc;
using MarketingBox.BackOffice.Service.Grpc.Models;
using Microsoft.Extensions.Logging;

namespace MarketingBox.BackOffice.Service.Services
{
    public class HelloService: IHelloService
    {
        private readonly ILogger<HelloService> _logger;

        public HelloService(ILogger<HelloService> logger)
        {
            _logger = logger;
        }

        public Task<HelloMessage> SayHelloAsync(HelloRequest request)
        {
            _logger.LogInformation("Hello from {name}", request.Name);

            return Task.FromResult(new HelloMessage
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
