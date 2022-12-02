namespace PaymentGateway.Infrastructure.Integration;

public record AcquiringBankResponse(bool Status, string? RejectionReason = null);