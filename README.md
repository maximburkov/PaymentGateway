# PaymentGateway

## How to run

To avoid issues with setting up the environemt I implemented a solution with docker-compose (You need docker to run the solution). To run Payment Gateway using your terminal (on any OS with docker):
- Open terminal
- Change the current directory to PaymentGateway/src - `cd FOLDER_WITH_SOLUTION/PaymentGateway/src`
- Run `docker-compose up` command (it could take couple of minutes to download and build images and start the container). 

This will build and spin up 3 containers: Payment gateway API, Acquiring bank API, and database.

Docker Desktop example:

![image](docs/docker-desktop.png)

`docker ps` example:

![ps](docs/docker-ps.png)

- Open http://localhost:1111/swagger/index.html for using Swagger. 
- To avoid any issues with certificates during solution testing I use HTTP that's **only for testing purposes** (we should always use HTTPS).

## Architecture

The solution is implemented using the Clean Architecture (or [Hex. architecture](https://en.wikipedia.org/wiki/Hexagonal_architecture_(software))) approach. PaymentGateway.Core doesn't depend on Api and Infrastructure projects.

Projects:

src:
- `PaymentGateway` - main API wich is communicating through HTTP with Acquiring bank mock service and stores information about payments in the PostgreS database. Implemented using Minimal API. 
- `PaymentGateway.Core` - this project will contain all code specific to the domain layer. Doesn't depend on Api and Infrastructure project
- `PaymentGateway.Infrastructure` - this project contains classes for accessing external resources such as web services and databases, such as service for communication with Bank Mock API and database context.

test:
- `PaymentGateway.Core.UnitTests` - project with Unit Tests. Using xUnit, FluentAssertions and NSubstitute.
- `PaymentGateway.Api.IntegrationTests` - TBD.Currently skipped. Should spin up the environment correctly.

## API description

`Get /payments` - returns all payments.

Response example:
```json
[
  {
    "id": "df11509b-65b9-4081-89d8-67d7b58c450e",
    "merchantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "cardNumber": "3742********1211",
    "name": "Har*********",
    "cvv": "**8",
    "currency": "GBP",
    "amount": 10,
    "status": "Succeeded",
    "rejectionReason": null
  },
  {
    "id": "f15d4982-0bcb-4a49-a21e-7007a0f51e4d",
    "merchantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "cardNumber": "3742********1211",
    "name": "Har*********",
    "cvv": "**8",
    "currency": "GBP",
    "amount": 10000,
    "status": "Failed",
    "rejectionReason": "Not enough money for operation."
  }
]
```

`Post /payments` - creates new payment in the system. Payments are handled asynchronously. When this endpoint is called, Gateway creates payment and return `HTTP status 202` with location of created payment or error response.

Request example:
```json
{
  "idempotencyKey": "3fa85f64-5717-4562-b3fc-2c963f664fa6",
  "cardDetails": {
    "number": "3742454554001211",
    "name": "Harry Potter",
    "cvv": "888",
    "expYear": 2024,
    "expMonth": 4
  },
  "amount": 10,
  "currency": "GBP"
}
```

Response example:
```json
{
  "id": "df11509b-65b9-4081-89d8-67d7b58c450e",
  "merchantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cardNumber": "3742********1211",
  "name": "Har*********",
  "cvv": "**8",
  "currency": "GBP",
  "amount": 10,
  "status": "Pending",
  "rejectionReason": null
}
```

`Get /payments{id}` - retrieves payment by id.

Response example:
```json
{
  "id": "df11509b-65b9-4081-89d8-67d7b58c450e",
  "merchantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cardNumber": "3742********1211",
  "name": "Har*********",
  "cvv": "**8",
  "currency": "GBP",
  "amount": 10,
  "status": "Succeeded",
  "rejectionReason": null
}
```
### Payment handling algorithm
- Verify payment details (validation)
- Save payment with "Pending" status
- Send payment to message pusblisher. `IPublisher` is event bus abtraction. Currently payments are consumed by in memory event bus implemented with MediatR. This Event Bus starts Payment processing in the background. In production code in-memory event bus shoul be replaced with Message Queue and Message queue consumer.
- `Payment Processor` is sending payment to Acquiring bank and handles response and update payment Status and Rejection reason depending on a response from the bank.

## Testing Payment Gateway

Acquiring Bank mock contains predefined bank accounts. Payment could be rejected by bank in the following cases:

1. If user's bank account doesn't support specific currency
2. Invalid card details
3. Card expired
4. Not enough money for the operation

Predefined accounts (in memory, not persisted), that can be used to verify Payment Gateway:

```csharp
            new()
            {
                CardDetails = new CardDetails("3742454554001261", "Elon Musk", "777", 2025, 12),
                SpecificCurrencyAccounts = new List<SpecificCurrencyAccount>
                {
                    SpecificCurrencyAccount.Create("USD", 100_000_000_000),
                    SpecificCurrencyAccount.Create("GBP", 1000)
                }
            },
            new()
            {
                CardDetails = new CardDetails("3742454554001211", "Harry Potter", "888", 2024, 4),
                SpecificCurrencyAccounts = new List<SpecificCurrencyAccount>
                {
                    SpecificCurrencyAccount.Create("GBP", 100)
                }
            },
            new()
            {
                CardDetails = new CardDetails("3742454554001221", "Empty Dollar", "880", 2022, 4),
                SpecificCurrencyAccounts = new List<SpecificCurrencyAccount>
                {
                    SpecificCurrencyAccount.CreateEmpty("USD")
                }
            }
```


In Swagger I defined  request sample for payment. Harry Potter has only 100 GBP on his account, in this case if payment with 1000 GBP is requested bank will reject:

```json
{
  "idempotencyKey": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cardDetails": {
    "number": "3742454554001211",
    "name": "Harry Potter",
    "cvv": "888",
    "expYear": 2024,
    "expMonth": 4
  },
  "amount": 10,
  "currency": "GBP"
}

```

## Assumptions

- Payment should be handled in an asynchronous fashion. However, in the real world, we can use a combination of sync/async handling depending on some period. 
- We should not handle duplicated payments. To achieve this requirement I decided to use `IdempotencyKey` that should be generated on the client side on each new payment request. Currently, this key is unique across all payments but in production system, it could be user-specific. idempotency key could be stored in cache and be evicted after a while.
- For Unit testing and low-coupling I used the Dependency Inversion principle.
- For code readability, I used libraries with fluent syntaxes such as Fluent Assertions for Unit tests and Fluent Validation for validation.

## Areas for improvement
- Https 
- Add authentication
- Add Retry handling, use Polly
- Add logging consumer and dashboard, use Serilog. Currently only logging to console.
- Merchant should only be able to see his payments
- Common contracts could be distributed with Nuget. Currently it's duplicated. Remove contracts duplication.
- Replace in memory queue with distributed queue
- Move Payment Processing to separate worker service for scalability
- Use No-Sql database for high performance. I used EF core for the sake of simplicity. If Relational database is used add migrations.
- Finish Integration tests
- Use Bogus to create fake data (card numbers etc)
- Use WireMock in Integration tests for mocking network calls to Bank api
- Add parametrized tests
- Improve and refactor domain models
- Improve Code Coverage
- etc.

## Cloud Technologies

- I would use Amazon ECS or K8s for containers orchestration
- For data storage I'd use high performant No-Sql database Such as DynamoDB in AWS or CosmosDB in Azure. We can use merchant Id as a partition key to improve performance.
- I'd use Message bus such as Azure Service Bus or SQS in AWS
- Azure KeyVault or AWS KMS for key management
- For Tracing I'd use tool that's support OpenTracing (e.g. Jaeger)






