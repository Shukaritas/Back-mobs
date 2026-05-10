using RentalPeAPI.Payments.Domain.Model.Enums;
using RentalPeAPI.Payments.Domain.Model.ValueObjects;

namespace RentalPeAPI.Payments.Domain.Model.Aggregates;

public partial class Payment
{
    public int Id { get; }

    // Referencia al espacio (Space)
    public long SpaceId { get; private set; }

    // Usuario que paga (Remodeler)
    public Guid PayerUserId { get; private set; }

    // Usuario que recibe (Homeowner)
    public Guid PayeeUserId { get; private set; }

    // Número de cuota/parte
    public int Installment { get; private set; }

    public Money Money { get; private set; } = null!;

    public PaymentMethodSummary Method { get; private set; } = null!;

    public PaymentStatus Status { get; private set; }

    public string? Reference { get; private set; }

    // Fecha lógica del pago
    public DateTimeOffset Date { get; private set; }

    protected Payment()
    {
        Status = PaymentStatus.PENDING;
        Date = DateTimeOffset.UtcNow;
    }

    public Payment(
        long spaceId,
        Guid payerUserId,
        Guid payeeUserId,
        int installment,
        Money money,
        PaymentMethodSummary method,
        string? reference = null,
        DateTimeOffset? date = null)
    {
        if (spaceId <= 0) throw new ArgumentOutOfRangeException(nameof(spaceId));
        if (payerUserId == Guid.Empty) throw new ArgumentException("Payer user ID cannot be empty", nameof(payerUserId));
        if (payeeUserId == Guid.Empty) throw new ArgumentException("Payee user ID cannot be empty", nameof(payeeUserId));
        if (installment <= 0) throw new ArgumentOutOfRangeException(nameof(installment));
        if (money is null) throw new ArgumentNullException(nameof(money));
        if (method is null) throw new ArgumentNullException(nameof(method));

        SpaceId = spaceId;
        PayerUserId = payerUserId;
        PayeeUserId = payeeUserId;
        Installment = installment;
        Money = money;
        Method = method;
        Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim();
        Status = PaymentStatus.PENDING;
        Date = date ?? DateTimeOffset.UtcNow;
    }

    public void InitiatePayment()
    {
        if (Status != PaymentStatus.PENDING)
            throw new InvalidOperationException("Payment already initiated");
    }

    public void ConfirmPayment()
    {
        if (Status != PaymentStatus.PENDING)
            throw new InvalidOperationException("Only PENDING payments can be confirmed");
        Status = PaymentStatus.SETTLED;
        Date = DateTimeOffset.UtcNow;
    }

    public void CancelPayment()
    {
        if (Status != PaymentStatus.PENDING)
            throw new InvalidOperationException("Only PENDING payments can be cancelled");
        Status = PaymentStatus.CANCELLED;
        Date = DateTimeOffset.UtcNow;
    }

    public void RefundPayment()
    {
        if (Status != PaymentStatus.SETTLED)
            throw new InvalidOperationException("Only SETTLED payments can be refunded");
        Status = PaymentStatus.REFUNDED;
        Date = DateTimeOffset.UtcNow;
    }

    public void ChangeInstallment(int installment)
    {
        if (installment <= 0) throw new ArgumentOutOfRangeException(nameof(installment));
        Installment = installment;
    }

    public void ChangeSpace(long spaceId)
    {
        if (spaceId <= 0) throw new ArgumentOutOfRangeException(nameof(spaceId));
        SpaceId = spaceId;
    }
}