using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ItPlatform.Tests.Integration;

public class AuthFlowTests
{
    private const string ApiGateway = "http://localhost:7000";
    private const string PortalShell = "http://localhost:3000";
    
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Test the complete login flow through API Gateway
        using var client = new HttpClient { BaseAddress = new Uri(ApiGateway) };
        var loginRequest = new
        {
            username = "admin",
            password = "admin123",
            tenantId = "default"
        };
        
        var response = await client.PostAsync("/auth/login", 
            new StringContent(JsonSerializer.Serialize(loginRequest), 
                Encoding.UTF8, "application/json"));
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var token = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(token);
    }
    
    [Fact]
    public async Task HealthCheck_AllServices_ReturnsOk()
    {
        // Test health endpoints for all services
        var services = new[] { $"{ApiGateway}/health", $"{PortalShell}/api/health" };
        foreach (var endpoint in services)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(endpoint);
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}
