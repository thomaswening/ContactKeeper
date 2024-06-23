using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.Core.Exceptions;

using Serilog;

namespace ContactKeeper.Infrastructure.Utilities;

/// <summary>
/// Represents a utility for initializing application data.
/// </summary>
/// <param name="fileSystem">The file system to use for file operations.</param>
/// <param name="logger">The logger to use for logging.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="fileSystem"/> or <paramref name="logger"/> is <see langword="null"/>.</exception>
public class AppDataInitializer(IFileSystem fileSystem, ILogger logger)
{
    private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    /// <summary>
    /// Initializes the application data.
    /// </summary>
    /// <param name="baseDirectory">The base directory to use for the application data.</param>
    /// <param name="defaultFileName">The default file name to use for the application data.</param>
    /// <returns>The full path to the application data file.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="baseDirectory"/> is empty or whitespace.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the application does not have access to create the directory.</exception>
    /// <exception cref="Exception">Thrown when an error occurs while creating the directory.</exception>
    public string Initialize(string baseDirectory, string defaultFileName = "contacts.json")
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);
        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", nameof(baseDirectory));
        }

        EnsureDirectoryExists(baseDirectory);
        return fileSystem.Path.Combine(baseDirectory, defaultFileName);
    }

    private void EnsureDirectoryExists(string baseDirectory)
    {
        if (fileSystem.Directory.Exists(baseDirectory))
        {
            return;
        }

        try
        {
            logger.Information($"Directory {baseDirectory} does not exist. Creating directory.");
            fileSystem.Directory.CreateDirectory(baseDirectory);
            logger.Information($"Directory {baseDirectory} created.");
        }
        catch (UnauthorizedAccessException ex)
        {
            throw ExceptionHelper.LogAndThrow(logger, $"Failed to create directory {baseDirectory} due to lack of access.", ex);
        }
        catch (Exception ex)
        {
            throw ExceptionHelper.LogAndThrow(logger, $"Failed to create directory {baseDirectory}.", ex);
        }
    }
}