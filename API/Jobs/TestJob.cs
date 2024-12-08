using Application.Services;
using Hangfire;

namespace API.Jobs
{
    public class TestJob(ITestService service)
    {
        private readonly ITestService _service = service;

        public async Task Test()
        {
            await _service.Test();
        }
    }
}
