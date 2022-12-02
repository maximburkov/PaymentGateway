using System.Net.Http.Json;
using System.Text.Json;
using PaymentGateway.Core;

namespace PaymentGateway.Infrastructure.Integration;

public class AcquiringBankMockService : IAcquiringBankService
{
    private readonly HttpClient _httpClient;
    
    public AcquiringBankMockService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<(bool isSuccesseful, string? error)> MakePayment(Payment payment)
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

            return result is { Status: true } ? (true, null) : (false, result!.RejectionReason);
        }
        catch (HttpRequestException ex)
        {
            // TODO: log
            return (false, "Acquiring bank unavailable");
        }
    }
}