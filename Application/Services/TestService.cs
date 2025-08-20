using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services;
internal class TestService(ILogger<TestService> logger) : ITestService
{
    public async Task Test()
    {
        logger.LogInformation("Starting Test Job");
        await Task.Delay(1000);
        logger.LogInformation("Finish Test Job");
    }
}
