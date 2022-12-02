using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Dto;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Endpoints;

public static class PaymentEndpoints
{
    public static void UsePaymentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/payments", async ([FromServices] ApplicationDbContext context) =>
            {
                var payments = await context.Payments
                    .AsNoTracking()
                    .ToListAsync();

                return Results.Ok(payments.Select(PaymentDetailsResponse.FromPayment));
            })
            .Produces<List<PaymentDetailsResponse>>();

        app.MapGet("/payments/{id}", async (Guid id, ApplicationDbContext context) =>
            {
                var payment = await context.Payments.FirstOrDefaultAsync(payment => payment.Id == id);
                return payment is null
                    ? Results.NotFound()
                    : Results.Ok(PaymentDetailsResponse.FromPayment(payment));
            })
            .Produces<PaymentDetailsResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/payments", async (PaymentRequest paymentRequest,
                [FromServices] ApplicationDbContext context,
                [FromServices] IPublisher publisher,
                IValidator<PaymentRequest> validator) =>
            {
                var validationResult = await validator.ValidateAsync(paymentRequest);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                if (await context.Payments.AnyAsync(payment => payment.IdempotencyKey == paymentRequest.IdempotencyKey))
                {
                    return Results.Conflict("Duplicated Idempotency key.");
                }

                var payment = paymentRequest.AsPayment();
                context.Payments.Add(payment);
                await context.SaveChangesAsync();

                await publisher.Send(payment);

                return Results.Accepted($"/payments/{payment.Id}", PaymentDetailsResponse.FromPayment(payment));
            })
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }
}