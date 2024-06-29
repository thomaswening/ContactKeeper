using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Abstractions;
using System.Windows;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Services;
using ContactKeeper.Infrastructure.Repositories;
using ContactKeeper.Infrastructure.Utilities;
using ContactKeeper.UI.Services;
using ContactKeeper.UI.Utilities;
using ContactKeeper.UI.ViewModels;
using ContactKeeper.UI.Views;

using Serilog;
using Serilog.Core;

namespace ContactKeeper.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string ApplicationName = "ContactKeeper";
    private readonly ILogger logger;
    private readonly IContactService contactService;
    private readonly DialogService dialogService;
    private readonly NavigationService navigationService;
    private readonly ViewModelFactory viewModelFactory;

    public App()
    {
        logger = InitializeSeriLogger();
        contactService = InitializeContactService(logger);
        dialogService = new DialogService();
        navigationService = new NavigationService(logger);
        viewModelFactory = new ViewModelFactory(navigationService, contactService, dialogService, logger);
    }

    private static ContactService InitializeContactService(ILogger logger)
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appDirectory = Path.Combine(appDataPath, ApplicationName);

        var fileSystem = new FileSystem();
        var filePath = new AppDataInitializer(fileSystem, logger).Initialize(appDirectory);
        var jsonContactRepository = new JsonContactRepository(filePath, fileSystem, logger);
        var contactService = new ContactService(logger, jsonContactRepository);

        return contactService;
    }

    private static Logger InitializeSeriLogger()
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

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var viewModel = await viewModelFactory.CreateMainWindowVmAsync();
        var mainWindow = new MainWindow() { DataContext = viewModel };
        mainWindow.Show();
    }
}
