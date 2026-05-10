using System;

namespace RentalPeAPI.Monitoring.Domain.Entities;

public class Reading
{
    public long Id { get; set; } 
    
    
    public long IoTDeviceId { get; set; } 
    public long SpaceId { get; set; }
    
    public string MetricName { get; set; } = string.Empty; 
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty; 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Reading() { }

    public Reading(long ioTDeviceId, long spaceId, string metricName, decimal value, string unit, DateTime timestamp)
    {
        SpaceId = spaceId;
        IoTDeviceId = ioTDeviceId;
        MetricName = metricName;
        Value = value;
        Unit = unit;
        Timestamp = timestamp;
    }
}