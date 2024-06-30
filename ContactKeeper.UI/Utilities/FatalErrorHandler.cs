using System;
using System.Windows;
using Serilog;

namespace ContactKeeper.UI.Utilities;

/// <summary>
/// Handles fatal errors that occur in the application.
/// </summary>
public class FatalErrorHandler
{
    private readonly ILogger logger;
    private bool hasFatalErrorBeenReportedToUser = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="FatalErrorHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging fatal errors.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public FatalErrorHandler(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this.logger = logger;
    }

    /// <summary>
    /// Handles an unhandled exception that occurs in the application domain, i.e. on a background thread,
    /// by logging the exception and showing an error message to the user.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        LogFatalException(e.ExceptionObject as Exception);
        hasFatalErrorBeenReportedToUser = true;
    }

    /// <summary>
    /// Handles an unhandled exception that occurs in the application dispatcher, i.e. on the UI thread,
    /// by logging the exception and showing an error message to the user.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        LogFatalException(e.Exception);
        hasFatalErrorBeenReportedToUser = true;
    }

    private void LogFatalException(Exception? ex)
    {
        var msg = "Fatal exception occurred. Exiting application.";

        if (ex is null)
        {
            logger.Fatal(msg);
        }
        else
        {
            logger.Fatal(ex, msg);
        }

        if (!hasFatalErrorBeenReportedToUser)
        {
            var userMsg = "An unexpected error occurred. The application will now close. " +
            "Any unsaved changes may have been lost.";

            MessageBox.Show(userMsg, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
