using Mediator;
using Microsoft.Extensions.Logging;

namespace ConferenceBooking.Application.Common.Behaviors;

//Логування запитів та команд
public class LoggingBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<LoggingBehavior<TMessage, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TMessage, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TMessage).Name;
        _logger.LogInformation("Обробка запиту {RequestName}", requestName);

        try
        {
            var response = await next(message, cancellationToken);
            _logger.LogInformation("Запит {RequestName} успішно оброблено", requestName);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка під час обробки {RequestName}", requestName);
            throw;
        }
    }
}