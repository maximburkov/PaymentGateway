using PaymentGateway.Core;

namespace PaymentGateway.UnitTests;

public class MaskingExtensionTests
{
    [Fact]
    public void MaskCardNumber_Returns_CorrectValue()
    {
        // Arrange
        string notMaskedNumber = new string(Enumerable.Repeat('1', 16).ToArray());

        // Act
        var masked = notMaskedNumber.MaskCardNumber('*');

        // Assert
        masked.Should().Be("1111********1111");
    }
    
    [Fact]
    public void MaskName_Returns_CorrectValue()
    {
        // Arrange
        string noMasked = "Test Name";

        // Act
        var masked = noMasked.MaskName('*');

        // Assert
        masked.Should().Be("Tes******");
    }
    
    [Fact]
    public void MaskCvv_Returns_CorrectValue()
    {
        // Arrange
        string notMasked = "123";

        // Act
        var masked = notMasked.MaskCvv('*');

        // Assert
        masked.Should().Be("**3");
    }
}