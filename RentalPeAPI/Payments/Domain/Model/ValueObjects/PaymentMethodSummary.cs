using RentalPeAPI.Payments.Domain.Model.Enums;

namespace RentalPeAPI.Payments.Domain.Model.ValueObjects;

public sealed record PaymentMethodSummary
{
    public PaymentMethodType Type { get; init; }
    public string? Label { get; init; }
    public string? Last4 { get; init; }

    // EF
    private PaymentMethodSummary() { }

    public PaymentMethodSummary(PaymentMethodType type, string? label = null, string? last4 = null)
    {
        if (!string.IsNullOrEmpty(last4) && last4.Length != 4)
            throw new ArgumentException("last4 must have length 4", nameof(last4));

        Type = type;
        Label = string.IsNullOrWhiteSpace(label) ? null : label.Trim();
        Last4 = string.IsNullOrWhiteSpace(last4) ? null : last4;
    }
}