# OrderStatusApi

Healthcare-style order status API built with ASP.NET Core, Entity Framework Core, and SQL-backed storage. This project models a realistic order-tracking workflow and demonstrates backend development patterns used in healthcare and integration-focused environments.

## Overview

This application provides a simple but practical order lifecycle workflow for a healthcare setting. It allows a system or user to create an order, track its current status, review the status timeline, and generate an HL7-style payload that could be used for downstream healthcare messaging or integration workflows.

The project was designed to reflect real business needs, including:

- order creation and validation
- status updates across a lifecycle
- status history and audit tracking
- filtering by current status
- integration-friendly data output
- automated API verification

## Why this project

This project is grounded in a real-world healthcare-style process: a vendor or internal workflow needs a reliable way to track the state of an order from acceptance to completion. The API demonstrates how a backend system can manage that flow while preserving a record of each change.

It is useful as a portfolio project because it shows:

- practical API design
- data persistence
- business rule validation
- workflow-driven development
- test coverage for core functionality
- healthcare integration concepts without needing a full production system

## Tech Stack

- C#
- ASP.NET Core
- .NET 10
- Entity Framework Core
- SQL Server / SQLite
- xUnit
- Swagger
- GitHub

## Features

- Create orders with patient and vendor information
- Validate required fields before saving
- Track the current order status
- Maintain a full order status history
- Retrieve a single order with related timeline data
- Filter orders by current status
- Generate an HL7-style payload for healthcare-style messaging
- Run automated integration tests against the API

## Project Structure

- `Program.cs` - API endpoints and startup configuration
- `AppDbContext.cs` - Entity Framework Core database context
- `Models.cs` - order, status history, and request models
- `Hl7MessageService.cs` - HL7-style payload generation
- `OrderStatusApi.Tests/` - integration tests for API behavior

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server LocalDB or a compatible database
- Git
- Visual Studio or VS Code

### Clone the repository

```bash
git clone https://github.com/YOUR-USERNAME/OrderStatusApi.git
cd OrderStatusApi
```

### Restore dependencies

```bash
dotnet restore
```

### Run the application

```bash
dotnet run
```

### Open Swagger

Once the app is running, open the Swagger UI in a browser:

```text
https://localhost:<port>/swagger
```

## API Endpoints

### Create an order

`POST /orders`

Example request:

```json
{
  "vendor": "TestVendor",
  "patientName": "Jane Smith"
}
```

### Get all orders

`GET /orders`

Optional filter:

```text
?status=Processed
```

### Get an order by ID

`GET /orders/{id}`

### Update order status

`POST /orders/{id}/status`

Example request:

```json
{
  "status": "Processed"
}
```

### Get status history

`GET /orders/{id}/status-history`

Optional filter:

```text
?status=Processed
```

### Get HL7-style payload

`GET /orders/{id}/hl7`

Example response:

```json
{
  "orderId": 1,
  "messageType": "HL7",
  "payload": "MSH|^~\\&|ORDERSTATUSAPI|HEALTHCARE|..."
}
```

## Example Workflow

1. Create a new order with a patient name and vendor name.
2. The API saves the record and creates an initial status entry.
3. Update the order to a new status as the workflow progresses.
4. Review the complete status timeline for the order.
5. Filter and query orders based on their current status.
6. Generate an HL7-style payload to represent the order/status message in a healthcare-style format.

## Testing

The project includes automated API integration tests using xUnit and a SQLite in-memory database.

Run the tests with:

```bash
dotnet test
```

Covered scenarios include:

- creating an order
- rejecting invalid or empty patient name values
- updating status values
- preserving order status history
- filtering by current status
- validating HL7-style output structure

## Future Enhancements

- add more advanced HL7 message variants
- expand validation and business rules
- introduce authentication and authorization
- support more workflow states and transitions
- add Docker support for easier environment setup
- add additional examples and mock healthcare payload scenarios

## License

This project is currently available without a license.

## Notes

This project is intentionally practical and business-focused. It demonstrates a backend workflow that is relevant to healthcare and integration-driven environments while remaining approachable for broader .NET and API development work.

It is designed to show real-world software skills without depending on a large, overly abstract sample application.
