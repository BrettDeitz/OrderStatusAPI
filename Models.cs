namespace OrderStatusApi.Models;

public class Order
{
    public int Id { get; set; }
    public string Vendor { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Status { get; set; } = "Accepted";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public List<OrderStatusHistory> StatusHistory { get; set; } = new();
}

public class OrderStatusHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}


public class CreateOrderRequest
{
    public string Vendor { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
}

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class GetSpecificStatusedOrders
{
    public string Status { get; set; } = String.Empty;
}