﻿using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ContactKeeper.Core.Exceptions;
using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Models;

using Serilog;

namespace ContactKeeper.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for managing contacts that uses JSON files.
/// </summary>
public class JsonContactRepository : IContactRepository
{
    private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);
    private readonly string filePath;
    private readonly ILogger logger;
    private readonly IFileSystem fileSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonContactRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="fileSystem">The file system to use for file operations.</param>
    /// <param name="filePath">The path to the JSON file to use for storing contacts.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/>, <paramref name="filePath"/>, or <paramref name="fileSystem"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is empty or whitespace.</exception>    
    public JsonContactRepository(string filePath, IFileSystem fileSystem, ILogger logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));

        this.filePath = filePath;
        this.logger = logger;
        this.fileSystem = fileSystem;
    }

    /// <summary>
    /// Gets the contacts from the JSON file.
    /// </summary>
    /// <returns>A task returning an <see cref="IEnumerable{Contact}"/> of the contacts in the JSON file.</returns>
    /// <exception cref="JsonException">Thrown when the contact data file is invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when read access to the contact data file is denied.</exception>
    /// <exception cref="Exception">Thrown when an error occurs while retrieving contacts.</exception>
    public async Task<IEnumerable<Contact>> GetContactsAsync()
    {
        logger.Information($"Attempting to retrieve contacts from JSON repository.");

        if (!fileSystem.File.Exists(filePath))
        {
            logger.Warning($"JSON repository not found. Creating a new one.");
            await SaveContactsAsync([]);
        }

        await semaphoreSlim.WaitAsync();

        try
        {
            using var stream = fileSystem.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            List<Contact> contacts;
            if (stream.Length == 0)
            {
                contacts = [];
            }
            else
            {
                contacts = await JsonSerializer.DeserializeAsync<List<Contact>>(stream) ?? [];
            }

            logger.Information($"Successfully retrieved {contacts.Count} contacts.");
            return contacts;
        }
        catch (JsonException ex)
        {
            var msg = $"Contact data file '{filePath}' is invalid.";
            logger.Error(ex, msg);

            throw new RepositoryCorruptedException(msg, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.Error(ex, $"Read access to the contact data file JSON repository is denied.");
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"An error occurred while retrieving contacts from JSON repository.");
            throw;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    /// <summary>
    /// Saves the contacts to the JSON file.
    /// </summary>
    /// <param name="contacts">Contacts to save to the JSON file.</param>
    /// <returns>A task representing the saving of the contacts.</returns>
    /// <exception cref="JsonException">Thrown when the contact data file is invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when write access to the contact data file is denied.</exception>
    /// <exception cref="Exception">Thrown when an error occurs while saving contacts.</exception>
    public async Task SaveContactsAsync(IEnumerable<Contact> contacts)
    {
        try
        {
            logger.Information("Attempting to save contacts to JSON repository.");

            await semaphoreSlim.WaitAsync();
            using var outputStream = fileSystem.File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(outputStream, contacts);

            logger.Information("Successfully saved contacts.");
        }
        catch (JsonException ex)
        {
            var msg = $"JSON repository is corrupted.";
            logger.Error(ex, msg);

            throw new RepositoryCorruptedException(msg, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.Error(ex, $"Write access to the JSON repository is denied.");
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"An error occurred while saving contacts to JSON repository.");
            throw;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
}
