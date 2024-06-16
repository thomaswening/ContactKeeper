using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Services;
using ContactKeeper.UI.Utilities;
using ContactKeeper.UI.ViewModels;
using ContactKeeper.UI.Views;

using Serilog;

namespace ContactKeeper.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string ApplicationName = "ContactKeeper";
    private readonly ILogger logger;
    private readonly IContactService contactService;
    private readonly ViewModelFactory viewModelFactory;

    public App()
    {
        logger = InitializeLogger();
        contactService = new DummyContactService();
        viewModelFactory = new ViewModelFactory(contactService);
    }

    private static ILogger InitializeLogger()
    {
        var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName, "logs");

        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }

        var logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(logPath, "ContactKeeperUI-.log"), rollingInterval: RollingInterval.Day)
            .WriteTo.Debug()
            .CreateLogger();

        return logger;
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var viewModel = new MainWindowVm(viewModelFactory);
        var mainWindow = new MainWindow()
        {
            DataContext = viewModel
        };
        mainWindow.Show();
    }
}
