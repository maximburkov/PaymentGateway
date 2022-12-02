using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PaymentGateway.Core;

namespace PaymentGateway.Infrastructure.Integration;

public class AcquiringBankMockService : IAcquiringBankService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AcquiringBankMockService> _logger;
    
    public AcquiringBankMockService(HttpClient httpClient, ILogger<AcquiringBankMockService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<(bool IsSuccesseful, string? Error)> MakePayment(Payment payment)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("payment", payment);
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AcquiringBankResponse>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result is { Status: true } ? (true, null) : (false, result!.Error);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Acquiring bank network error.");
            return (false, "Acquiring bank unavailable");
        }
    }
}