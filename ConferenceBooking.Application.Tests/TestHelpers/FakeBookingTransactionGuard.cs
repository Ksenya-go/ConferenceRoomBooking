using ConferenceBooking.Application.Common.Interfaces;

namespace ConferenceBooking.Application.Tests;


// Тестова заглушка виконує дію напряму, без реальної транзакції
// (InMemory provider не підтримує Serializable isolation).
public class FakeBookingTransactionGuard : IBookingTransactionGuard
{
    public Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        return action(cancellationToken);
    }
}