using PaymentGateway.Core;

namespace PaymentGateway.UnitTests;

public class PaymentProcessorTests
{
    private readonly PaymentProcessor _sut;
    private readonly IAcquiringBankService _bankServiceMock = Substitute.For<IAcquiringBankService>();

    public PaymentProcessorTests()
    {
        _sut = new PaymentProcessor(_bankServiceMock);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Process_ShouldReturnCorrectResult_DependingOnBankResponse(bool bankResponse)
    {
        // Arrange
        var payment = new Payment();
        _bankServiceMock.MakePayment(payment).Returns(Task.FromResult((bankResponse, "message")));

        // Act
        var result = await _sut.Process(payment);

        // Assert
        result.Should().Be(bankResponse);
    }
}