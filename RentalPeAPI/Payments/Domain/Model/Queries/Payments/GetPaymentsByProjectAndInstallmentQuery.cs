namespace RentalPeAPI.Payments.Domain.Model.Queries.Payments;

public sealed record GetPaymentsByProjectAndInstallmentQuery(int ProjectId, int Installment);