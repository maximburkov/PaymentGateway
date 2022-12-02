using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PaymentGateway.Dto;

namespace PaymentGateway.Api.IntegrationTests;

public class PaymentEndpointsTests : IClassFixture<WebApplicationFactory<IApiMarker>>
{
    private readonly WebApplicationFactory<IApiMarker> _factory;
    
    // TODO: Add TestContainers
    // TODO: Add WireMock for testing 3rd party service

    public PaymentEndpointsTests(WebApplicationFactory<IApiMarker> factory)
    {
        _factory = factory;
    }
    
    [Fact(Skip = "Need to properly spin up environment.")]
    public async Task RequestPayment_CreatesPaymentWithAcceptedStatus_WhenPaymentIsValid()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var paymentRequest = new PaymentRequest
        {
            Amount = 10,
            CardDetails = CreateCardDetails(),
            Currency = "GPB",
            IdempotencyKey = Guid.NewGuid()
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/payment", paymentRequest);
        var acceptedPayment = await response.Content.ReadFromJsonAsync<PaymentDetailsResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        acceptedPayment.Should().NotBeNull();
        acceptedPayment!.Amount.Should().Be(paymentRequest.Amount);
    }

    private static CardDetails CreateCardDetails()
    {
        return new CardDetails
        {
            Cvv = "087",
            ExpMonth = 11,
            ExpYear = 2028,
            Name = "Hermione Granger",
            Number = "1234-1344-1234-1234"
        };
    }
}