using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

using ContactKeeper.UI.Views;

using Serilog;

namespace ContactKeeper.UI;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string ApplicationName = "ContactKeeper";

    public App()
    {
        InitializeLogger();
    }

    private static void InitializeLogger()
    {
        var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName, "logs");

        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(logPath, "ContactKeeperUI-.log"), rollingInterval: RollingInterval.Day)
            .WriteTo.Debug()
            .CreateLogger();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
