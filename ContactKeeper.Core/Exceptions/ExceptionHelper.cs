using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace ContactKeeper.Core.Exceptions;

public static class ExceptionHelper
{
    /// <summary>
    /// Logs the specified message and exception and returns a new exception of the specified type.
    /// </summary>
    /// <typeparam name="TException">The type of exception to throw.</typeparam>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <returns>A new exception of the specified type.</returns>
    /// <exception cref="TException">Always thrown.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the new exception cannot be created.</exception>
    public static TException LogAndThrow<TException>(ILogger logger, string message, Exception? exception = null) where TException : Exception
    {
        logger.Error(exception, message);

        var newException = Activator.CreateInstance(typeof(TException), message, exception) as TException
            ?? throw new InvalidOperationException($"Failed to create exception of type {typeof(TException)}.", exception);

        return newException;
    }

    /// <summary>
    /// Logs the specified message and exception and retuns the specified exception.
    /// </summary>
    /// <typeparam name="TException">The type of exception to throw.</typeparam>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <returns>The specified exception.</returns>
    /// <exception cref="TException">Always thrown.</exception>
    public static TException LogAndThrow<TException>(ILogger logger, string message, TException exception) where TException : Exception
    {
        logger.Error(exception, message);
        return exception;
    }
}
