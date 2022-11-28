namespace PaymentGateway;

public record PaymentRequest(Guid Id, string CardNumber, string Name, int Amount);