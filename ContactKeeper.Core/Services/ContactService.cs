using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ContactKeeper.Core.Exceptions;
using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Models;
using ContactKeeper.Core.Utilities;

using Serilog;

namespace ContactKeeper.Core.Services;
/// <summary>
/// Represents a service for managing contacts.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ContactService"/> class.
/// </remarks>
/// <param name="repository">The repository to use for managing contacts.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="repository"/> is <see langword="null"/>.</exception>
public class ContactService(ILogger logger, IContactRepository repository) : IContactService
{
    private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IContactRepository repository = repository ?? throw new ArgumentNullException(nameof(repository));

    private bool isInitialized = false;

    public List<Contact> Contacts { get; private set; } = [];

    public event EventHandler? ContactsChanged;

    /// </inheritdoc>
    public async Task<Contact?> AddContactAsync(ContactInfo contactInfo)
    {
        logger.Information("Adding new contact.");
        var newContact = contactInfo.ToContact();
        Contacts.Add(newContact);

        try
        {            
            await repository.SaveContactsAsync(Contacts);
            ContactsChanged?.Invoke(this, EventArgs.Empty);

            logger.Information($"New contact added successfully with ID {newContact.Id}.");

            return newContact;
        }
        catch (Exception ex)
        {
            Contacts.Remove(newContact);

            var msg = "Failed to add new contact.";
            logger.Error(ex, msg);

            throw;
        }
    }

    /// </inheritdoc>
    public async Task<Guid?> DeleteContactAsync(Guid id)
    {
        logger.Information($"Deleting contact with ID {id}.");

        var foundContact = Contacts.FirstOrDefault(c => c.Id == id);
        if (foundContact is null)
        {
            logger.Warning($"Contact with ID {id} not found.");
            return null;
        }

        try
        {           
            Contacts.Remove(foundContact);
            await repository.SaveContactsAsync(Contacts);
            ContactsChanged?.Invoke(this, EventArgs.Empty);

            logger.Information($"Contact with ID {id} deleted successfully.");

            return foundContact.Id;
        }
        catch (Exception ex)
        {
            Contacts.Add(foundContact);

            logger.Error(ex, $"Failed to delete contact with ID {id}.");
            throw;
        }
    }

    /// </inheritdoc>
    public async Task<Contact?> GetContactAsync(Guid id)
    {
        logger.Information($"Getting contact with ID {id}.");

        var foundContact = await Task.Run(() =>
        {
            return Contacts.FirstOrDefault(c => c.Id == id);
        });

        if (foundContact is null)
        {
            logger.Warning($"Contact with ID {id} not found.");
            return null;
        }

        return foundContact;
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Contact>> GetContactsAsync()
    {
        // If contacts have already been cached, return them
        if (isInitialized)
        {
            return Contacts;
        }

        // Otherwise, fetch contacts from the repository
        try
        { 
            Contacts = (await repository.GetContactsAsync()).ToList();
            isInitialized = true;
            return Contacts;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to get contacts.");
            throw;
        }
    }

    /// </inheritdoc>
    public async Task<Contact?> UpdateContactAsync(Guid id, ContactInfo updateInfo)
    {
        logger.Information($"Updating contact with ID {id}.");

        var existingContact = Contacts.FirstOrDefault(c => c.Id == id);
        if (existingContact is null)
        {
            logger.Warning($"Contact with ID {id} not found.");
            return null;
        }

        var backupInfo = ContactInfo.FromContact(existingContact);
        updateInfo.OverwriteOnto(existingContact);

        try
        {
            await repository.SaveContactsAsync(Contacts);
            ContactsChanged?.Invoke(this, EventArgs.Empty);

            logger.Information($"Contact with ID {id} updated successfully.");

            return existingContact;            
        }
        catch (Exception ex)
        {
            backupInfo.OverwriteOnto(existingContact);
            logger.Error(ex, $"Failed to update contact with ID {id}.");
            throw;
        }
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Contact>> FindContact(ContactInfo contactInfo)
    {
        logger.Information("Finding contact.");

        var foundContacts = await Task.Run(() =>
        {
            return Contacts.Where(c =>
                (contactInfo.FirstName is null || c.FirstName == contactInfo.FirstName) &&
                (contactInfo.LastName is null || c.LastName == contactInfo.LastName) &&
                (contactInfo.Email is null || c.Email == contactInfo.Email) &&
                (contactInfo.Phone is null || c.Phone == contactInfo.Phone)
            );
        });

        return foundContacts;
    }
}
