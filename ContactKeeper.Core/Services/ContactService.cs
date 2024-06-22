using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
    
    public List<Contact>? Contacts { get; private set; }


    /// </inheritdoc>
    public async Task<Contact?> AddContactAsync(ContactInfo contactInfo)
    {               
        try
        {
            logger.Information("Adding new contact.");

            if (Contacts is null)
            {
                throw new InvalidOperationException("Contacts have not been initialized.");
            }

            var newContact = contactInfo.ToContact();
            Contacts.Add(newContact);
            await repository.SaveContactsAsync(Contacts);

            logger.Information($"New contact added successfully with ID {newContact.Id}.");

            return newContact;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to add new contact.");
            throw;
        }
    }

    /// </inheritdoc>
    public async Task<Guid?> DeleteContactAsync(Guid id)
    {
        try
        {
            logger.Information($"Deleting contact with ID {id}.");

            if (Contacts is null)
            {
                throw new InvalidOperationException("Contacts have not been initialized.");
            }

            var foundContact = Contacts.FirstOrDefault(c => c.Id == id);
            if (foundContact is null)
            {
                logger.Warning($"Contact with ID {id} not found.");
                return null;
            }

            Contacts.Remove(foundContact);
            await repository.SaveContactsAsync(Contacts);

            logger.Information($"Contact with ID {id} deleted successfully.");

            return foundContact.Id;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Failed to delete contact with ID {id}.");
            throw;
        }
    }

    /// </inheritdoc>
    public async Task<Contact?> GetContactAsync(Guid id)
    {
        try
        {
            logger.Information($"Getting contact with ID {id}.");

            if (Contacts is null)
            {
                throw new InvalidOperationException("Contacts have not been initialized.");
            }

            var foundContact = await Task.Run(() =>
            {
                return Contacts.FirstOrDefault(c => c.Id == id);
            });

            if (foundContact is null)
            {
                logger.Warning($"Contact with ID {id} not found.");
            }

            return foundContact;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Failed to get contact with ID {id}.");
            throw;
        }
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Contact>> GetContactsAsync()
    {
        // Return cached contacts if available
        if (Contacts is not null)
        {
            return Contacts;
        }

        // Otherwise, fetch contacts from the repository
        try
        { 
            Contacts = (await repository.GetContactsAsync()).ToList();
            return Contacts;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to get contacts.");
            return [];
        }
    }

    /// </inheritdoc>
    public async Task<Contact?> UpdateContactAsync(Guid id, ContactInfo updateInfo)
    {        
        try
        {
            logger.Information($"Updating contact with ID {id}.");

            if (Contacts is null)
            {
                throw new InvalidOperationException("Contacts have not been initialized.");
            }

            var existingContact = Contacts.FirstOrDefault(c => c.Id == id);
            if (existingContact is null)
            {
                logger.Warning($"Contact with ID {id} not found.");
                return null;
            }

            existingContact.FirstName = updateInfo.FirstName ?? existingContact.FirstName;
            existingContact.LastName = updateInfo.LastName ?? existingContact.LastName;
            existingContact.Email = updateInfo.Email ?? existingContact.Email;
            existingContact.Phone = updateInfo.Phone ?? existingContact.Phone;

            await repository.SaveContactsAsync(Contacts);

            logger.Information($"Contact with ID {id} updated successfully.");

            return existingContact;            
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Failed to update contact with ID {id}.");
            throw;
        }
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Contact>> FindContact(ContactInfo contactInfo)
    {
        try
        {
            logger.Information("Finding contact.");

            if (Contacts is null)
            {
                throw new InvalidOperationException("Contacts have not been initialized.");
            }

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
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to find contact.");
            return [];
        }
    }
}
