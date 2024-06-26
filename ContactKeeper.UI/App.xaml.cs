﻿using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Abstractions;
using System.Windows;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Services;
using ContactKeeper.Infrastructure.Repositories;
using ContactKeeper.Infrastructure.Utilities;
using ContactKeeper.UI.Factories;
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
    private const string dialogHostIdentifier = "RootDialogHost";

    private readonly ILogger logger;
    private readonly IContactService contactService;
    private readonly IDialogService dialogService;
    private readonly INavigationService navigationService;
    private readonly ViewModelFactory viewModelFactory;
    private readonly FatalErrorHandler fatalErrorHandler;

    public App()
    {
        logger = InitializeSeriLogger();
        fatalErrorHandler = new FatalErrorHandler(logger);
        contactService = InitializeContactService(logger);
        dialogService = InitializeDialogService();
        navigationService = new NavigationService(logger);
        viewModelFactory = new ViewModelFactory(navigationService, contactService, dialogService, logger);
    }

    private static DialogService InitializeDialogService()
    {
        //var dialogHost = new MaterialDesignDialogHost(dialogHostIdentifier);
        var dialogHost = new WindowDialogHost();
        var viewFactory = new ModalDialogViewFactory();
        return new DialogService(dialogHost, viewFactory);
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
            .WriteTo.File(Path.Combine(logPath, "ContactKeeper-.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
            .WriteTo.Debug()
            .CreateLogger();

        return logger;
    }

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
    
        var viewModel = await viewModelFactory.CreateMainWindowVmAsync();
        var mainWindow = new MainWindow() { DataContext = viewModel };
        mainWindow.Show();
    }

    // TO DO: Encapsulate fatal exception handling in a separate class

    private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        fatalErrorHandler.OnAppDomainUnhandledException(sender, e);
    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        fatalErrorHandler.OnDispatcherUnhandledException(sender, e);
    }
}
