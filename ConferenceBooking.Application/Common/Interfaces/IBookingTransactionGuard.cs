

namespace ConferenceBooking.Application.Common.Interfaces;

public interface IBookingTransactionGuard
{
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default);
}
