using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Dtos;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace LibraryManagement.UnitTest;

public class AuthServiceTests
{
    private AppDbContext GetInMemoryContext()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"Db_{Guid.NewGuid()}")
            .UseInternalServiceProvider(serviceProvider) 
            .Options;

        return new AppDbContext(options);
    }

    private IConfiguration GetFakeConfiguration()
    {
        var configData = new Dictionary<string, string>
        {
            { "Jwt:Key", "this_is_a_secret_key_for_testing" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser()
    {
        using var context = GetInMemoryContext();
        var config = GetFakeConfiguration();
        var logger = NullLogger<AuthService>.Instance;
        var service = new AuthService(context, config, logger);

        var dto = new UserRegistrationDto
        {
            UserName = "testuser",
            Password = "password123",
            Firstname = "Test",
            Lastname = "User"
        };

        await service.RegisterAsync(dto);

        Assert.True(await service.UserExistsAsync("testuser"));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_IfCredentialsAreValid()
    {
        using var context = GetInMemoryContext();
        var config = GetFakeConfiguration();
        var logger = NullLogger<AuthService>.Instance;
        var service = new AuthService(context, config, logger);

        var dto = new UserRegistrationDto
        {
            UserName = "validuser",
            Password = "securepass",
            Firstname = "Valid",
            Lastname = "User"
        };

        await service.RegisterAsync(dto);
        var token = await service.LoginAsync("validuser", "securepass");

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_IfInvalidCredentials()
    {
        using var context = GetInMemoryContext();
        var config = GetFakeConfiguration();
        var logger = NullLogger<AuthService>.Instance;
        var service = new AuthService(context, config, logger);

        var token = await service.LoginAsync("nonexistent", "wrongpass");

        Assert.Null(token);
    }
}
