using AcquiringBankMock;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/payment", () => Results.Ok(new PaymentResponse("Test")));

app.MapPost("/payment", (PaymentRequest payment) => Results.Ok(new PaymentResponse("ok")));

app.Run();