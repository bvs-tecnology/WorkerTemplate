using System.Reflection.Metadata.Ecma335;

namespace Application.Services
{
    public class TestService : ITestService
    {
        public async Task Test()
        {
            Console.WriteLine("Test from RecurringJob");
        }
    }
}
