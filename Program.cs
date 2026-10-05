using Microsoft.EntityFrameworkCore;
using OrderStatusApi;
using OrderStatusApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/orders", async (CreateOrderRequest request, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.PatientName))
    {
        return Results.BadRequest(new { message = "Patient Name is required." });
    }

    if (string.IsNullOrWhiteSpace(request.Vendor))
    {
        return Results.BadRequest(new { message = "Vendor is required." });
    }

    var order = new Order
    {
        Vendor = request.Vendor,
        PatientName = request.PatientName,
        Status = "Accepted",
        CreatedAt = DateTime.UtcNow,
        LastUpdated = DateTime.UtcNow
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    var history = new OrderStatusHistory
    {
        OrderId = order.Id,
        Status = "Accepted",
        Timestamp = DateTime.UtcNow
    };

    db.OrderStatusHistory.Add(history);
    await db.SaveChangesAsync();

    return Results.Created($"/orders/{order.Id}", new
    {
        order.Id,
        order.Vendor,
        order.PatientName,
        order.Status,
        order.CreatedAt,
        order.LastUpdated
    });
});

app.MapGet("/orders", async (string? status, AppDbContext db) =>
{
    IQueryable<Order> query = db.Orders
        .Include(o => o.StatusHistory)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(status))
    {
        query = query.Where(o => o.Status == status);
    }

    var orders = await query
        .OrderByDescending(o => o.CreatedAt)
        .ToListAsync();

    var result = orders.Select(o => new
    {
        o.Id,
        o.Vendor,
        o.PatientName,
        o.Status,
        o.CreatedAt,
        o.LastUpdated,
        StatusHistory = o.StatusHistory.Select(h => new
        {
            h.Id,
            h.OrderId,
            h.Status,
            h.Timestamp
        }).ToList()
    }).ToList();

    return Results.Ok(result);
});

app.MapGet("/orders/{id:int}/status-history", async (int id, string? status, AppDbContext db) =>
{
    var query = db.OrderStatusHistory
        .Where(h => h.OrderId == id)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(status))
    {
        query = query.Where(h => h.Status == status);
    }

    var history = await query
        .OrderBy(h => h.Timestamp)
        .Select(h => new
        {
            h.Id,
            h.OrderId,
            h.Status,
            h.Timestamp
        })
        .ToListAsync();
    if (!history.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(history);
});

app.MapGet("/orders/{id:int}", async (int id, AppDbContext db) =>
{
    var order = await db.Orders
        .Include(o => o.StatusHistory)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order is null)
    {
        return Results.NotFound();
    }

    var result = new
    {
        order.Id,
        order.Vendor,
        order.PatientName,
        order.Status,
        order.CreatedAt,
        order.LastUpdated,
        StatusHistory = order.StatusHistory.Select(h => new
        {
            h.Id,
            h.OrderId,
            h.Status,
            h.Timestamp
        }).ToList()
    };

    return Results.Ok(result);
});

app.MapGet("/orders/{id:int}/hl7", async (int id, AppDbContext db) =>
{
    var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
    if (order is null)
    {
        return Results.NotFound();
    }

    var message = Hl7MessageService.CreateOrderStatusMessage(order);

    return Results.Ok(new
    {
        orderId = order.Id,
        messageType = "HL7",
        payload = message
    });
});

app.MapPost("/orders/{id:int}/status", async (int id, UpdateOrderStatusRequest request, AppDbContext db) =>
{
    var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
    if (order is null)
    {
        return Results.NotFound();
    }

    order.Status = request.Status;
    order.LastUpdated = DateTime.UtcNow;

    db.OrderStatusHistory.Add(new OrderStatusHistory
    {
        OrderId = order.Id,
        Status = request.Status,
        Timestamp = DateTime.UtcNow
    });

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        order.Id,
        order.Status,
        order.LastUpdated
    });
});

app.Run();

public partial class Program;