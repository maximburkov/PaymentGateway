namespace PaymentGateway.Infrastructure.Integration;

public record PaymentRequest(
    string CardNumber, 
    string Name, 
    string Cvv, 
    int ExpYear,
    int ExpMonth,
    string Currency, 
    decimal Amount);