using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class TestService(ILogger<TestService> logger) : ITestService
    {
        public async Task Test()
        {
            logger.LogInformation("Starting Test Job");
            Console.WriteLine("Test from RecurringJob");
            logger.LogInformation("Finish Test Job");
        }
    }
}
