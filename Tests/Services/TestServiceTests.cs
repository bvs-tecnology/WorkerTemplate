using Application.Services;
using Microsoft.Extensions.Logging;

namespace Tests.Services;

public class TestServiceTests
{
    private readonly Mock<ILogger<TestService>> _logger = new();
    private readonly TestService _service;

    public TestServiceTests() => _service = new TestService(_logger.Object);
}