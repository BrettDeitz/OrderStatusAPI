using OrderStatusApi.Models;

namespace OrderStatusApi;

public static class Hl7MessageService
{
    public static string CreateOrderStatusMessage(Order order)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var messageId = Guid.NewGuid().ToString("N");
        var patientName = EscapeHl7(order.PatientName);
        var vendor = EscapeHl7(order.Vendor);

        return string.Join(
            Environment.NewLine,
            $"MSH|^~\\&|ORDERSTATUSAPI|HEALTHCARE|LAB|VENDOR|{timestamp}||ORM^O01|{messageId}|P|2.5",
            $"PID|1||{order.Id}||{patientName}^^^^L",
            $"ORC|NW|{order.Id}|{order.Id}|{order.Id}||{order.Status}",
            $"OBX|1|TX|ORDER_STATUS||{order.Status}",
            $"OBX|2|TX|PATIENT_NAME||{patientName}",
            $"OBX|3|TX|VENDOR||{vendor}"
        );
    }

    private static string EscapeHl7(string value)
    {
        return value
            .Replace("\\", "\\E\\")
            .Replace("|", "\\F\\")
            .Replace("^", "\\S\\")
            .Replace("~", "\\R\\")
            .Replace("&", "\\T\\");
    }
}
