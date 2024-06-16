using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace ContactKeeper.Infrastructure.Utilities;

/// <summary>
/// Initializes the application data.
/// </summary>
public static class AppDataInitializer
{
    /// <summary>
    /// Initializes the application data.
    /// </summary>
    /// <param name="baseDirectory">The base directory to use for the application data.</param>
    /// <param name="defaultFileName">The default file name to use for the application data.</param>
    /// <returns>The full path to the application data file.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
    public static string Initialize(string baseDirectory, string defaultFileName = "contacts.json")
    {
        EnsureDirectoryExists(baseDirectory);
        return Path.Combine(baseDirectory, defaultFileName);
    }

    private static void EnsureDirectoryExists(string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);

        if (!Directory.Exists(baseDirectory))
        {
            Directory.CreateDirectory(baseDirectory);
        }
    }
}