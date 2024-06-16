using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

    public JsonContactRepository(ILogger logger, string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(logger);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
        }

        this.filePath = filePath;
        this.logger = logger;
    }

    /// <summary>
    /// Gets the contacts from the JSON file.
    /// </summary>
    /// <returns>A task returning an <see cref="IEnumerable{Contact}"/> of the contacts in the JSON file.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the contact data file is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the contact data file is not found.</exception>
    public async Task<IEnumerable<Contact>> GetContactsAsync()
    {
        try
        {
            logger.Information("Attempting to retrieve contacts from {FilePath}", filePath);

            if (!File.Exists(filePath))
            {
                logger.Warning("File {FilePath} not found. Creating a new file.", filePath);
                await SaveContactsAsync(Enumerable.Empty<Contact>());
            }

            await semaphoreSlim.WaitAsync();
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contacts = await JsonSerializer.DeserializeAsync<List<Contact>>(stream) ?? new List<Contact>();

            logger.Information("Successfully retrieved {Count} contacts.", contacts.Count);
            return contacts;
        }
        catch (JsonException ex)
        {
            logger.Error(ex, "Contact data file is invalid.");
            throw new InvalidOperationException("Contact data file is invalid.", ex);
        }
        catch (Exception e)
        {
            logger.Error(e, "An error occurred while getting contacts.");
            throw new InvalidOperationException("An error occurred while getting contacts.", e);
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
    /// <exception cref="InvalidOperationException">Thrown when the contact data file is invalid.</exception>
    public async Task SaveContactsAsync(IEnumerable<Contact> contacts)
    {
        try
        {
            logger.Information("Attempting to save contacts to {FilePath}", filePath);

            await semaphoreSlim.WaitAsync();
            using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(outputStream, contacts);

            logger.Information("Successfully saved contacts.");
        }
        catch (JsonException ex)
        {
            logger.Error(ex, "Contact data file is invalid.");
            throw new InvalidOperationException("Contact data file is invalid.", ex);
        }
        catch (Exception e)
        {
            logger.Error(e, "An error occurred while saving contacts.");
            throw new InvalidOperationException("An error occurred while saving contacts.", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
}
