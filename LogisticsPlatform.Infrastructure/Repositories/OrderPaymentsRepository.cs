using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Payments;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories;

public sealed class OrderPaymentsRepository(AppDbContext dbContext) : IOrderPaymentsRepository
{
    public async Task<string?> GetOrderNumberAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => o.Number)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> SumActiveSuppliesCentsAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.OrderSupplies
            .AsNoTracking()
            .Where(s => s.OrderId == orderId)
            .SumAsync(s => s.LineTotalCents, cancellationToken);
    }

    public async Task<bool> HasPaidAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.OrderPayments
            .AsNoTracking()
            .AnyAsync(
                p => p.OrderId == orderId && p.Status == OrderPaymentStatus.Paid,
                cancellationToken);
    }
        
    public async Task<OrderPaymentData> CreatePendingAsync(
        Guid orderId,
        long amountCents,
        string currency,
        CancellationToken cancellationToken)
    {
        var entity = new OrderPayment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            AmountCents = amountCents,
            Currency = currency,
            Status = OrderPaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.OrderPayments.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToData(entity);
    }

    public async Task SetStripeSessionIdAsync(
        Guid paymentId,
        string stripeSessionId,
        CancellationToken cancellationToken)
    {
        OrderPayment? entity = await dbContext.OrderPayments
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);

        if (entity is null)
            return;

        entity.StripeSessionId = stripeSessionId;
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderPaymentData?> GetByStripeSessionIdAsync(
        string stripeSessionId,
        CancellationToken cancellationToken)
    {
        OrderPayment? entity = await dbContext.OrderPayments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.StripeSessionId == stripeSessionId, cancellationToken);

        return entity is null ? null : ToData(entity);
    }

    public async Task<bool> MarkPaidAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        OrderPayment? entity = await dbContext.OrderPayments
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);

        if (entity is null)
            return false;

        if (entity.Status == OrderPaymentStatus.Paid)
            return true;

        bool orderAlreadyPaid = await dbContext.OrderPayments.AnyAsync(
            p => p.OrderId == entity.OrderId &&
                 p.Id != entity.Id &&
                 p.Status == OrderPaymentStatus.Paid,
            cancellationToken);

        if (orderAlreadyPaid)
        {
            entity.Status = OrderPaymentStatus.Canceled;
            await dbContext.SaveChangesAsync(cancellationToken);
            return false;
        }

        entity.Status = OrderPaymentStatus.Paid;
        entity.PaidAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task CancelPendingExceptAsync(
        Guid orderId,
        Guid keepPaymentId,
        CancellationToken cancellationToken)
    {
        List<OrderPayment> pending = await dbContext.OrderPayments
            .Where(p =>
                p.OrderId == orderId &&
                p.Id != keepPaymentId &&
                p.Status == OrderPaymentStatus.Pending)
            .ToListAsync(cancellationToken);

        if (pending.Count == 0)
            return;

        foreach (OrderPayment payment in pending)
            payment.Status = OrderPaymentStatus.Canceled;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static OrderPaymentData ToData(OrderPayment entity) =>
        new(
            entity.Id,
            entity.OrderId,
            entity.AmountCents,
            entity.Currency,
            entity.StripeSessionId,
            entity.Status,
            entity.CreatedAt,
            entity.PaidAt);
}
