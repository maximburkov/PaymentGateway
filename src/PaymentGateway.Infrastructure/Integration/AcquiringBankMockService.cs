using System.Net.Http.Json;
using PaymentGateway.Core;

namespace PaymentGateway.Infrastructure.Integration;

public class AcquiringBankMockService : IAcquiringBankService
{
    private readonly HttpClient _httpClient;
    
    public AcquiringBankMockService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<bool> MakePayment(Payment payment)
    {
        // TODO: remove
        // var res = await _httpClient.GetAsync("payment");
        // var t = await res.Content.ReadAsStringAsync();

        var res = await _httpClient.PostAsJsonAsync("payment", payment);

        return true;
    }
}