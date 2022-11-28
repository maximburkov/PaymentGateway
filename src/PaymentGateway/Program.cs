using PaymentGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/payment", async () =>
{
    var env = Environment.GetEnvironmentVariable("BANK_API_URL");
    using HttpClient client = new HttpClient();
    var res = await client.GetAsync($"{env}/payment");
    return "ohh of payments";
});
app.MapPost("/payment", (PaymentRequest payment) => Results.Ok(42));

app.UseSwagger();
app.UseSwaggerUI();

app.Run();