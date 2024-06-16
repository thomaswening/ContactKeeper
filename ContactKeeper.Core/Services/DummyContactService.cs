using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Models;
using ContactKeeper.Core.Utilities;

namespace ContactKeeper.Core.Services;

/// <summary>
/// Represents a dummy contact service that does not interact with a real data source.
/// </summary>
public class DummyContactService : IContactService
{
    public List<Contact> Contacts { get; private set; } = [];

    /// </inheritdoc>
    public event EventHandler? ContactsChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyContactService"/> class and generates dummy contacts.
    /// </summary>
    public DummyContactService()
    {
        DummyContactGenerator.GenerateContacts(10);
    }

    /// </inheritdoc>
    public async Task<Contact?> AddContactAsync(Contact contact)
    {
        return await Task.Run(() =>
        {
            Contacts.Add(contact);
            ContactsChanged?.Invoke(this, EventArgs.Empty);
            return contact;
        });
    }

    /// </inheritdoc>
    public async Task<Guid?> DeleteContactAsync(Guid id)
    {
        return await Task.Run(() =>
        {
            var foundContact = Contacts.FirstOrDefault(c => c.Id == id);
            if (foundContact is not null)
            {
                Contacts.Remove(foundContact);
                ContactsChanged?.Invoke(this, EventArgs.Empty);
            }

            return foundContact?.Id;
        });
    }

    /// </inheritdoc>
    public async Task<Contact?> GetContactAsync(Guid id)
    {
        return await Task.Run(() => Contacts.FirstOrDefault(c => c.Id == id));
    }

    /// </inheritdoc>
    public async Task InitializeContactsAsync()
    {
        await Task.Delay(1000);
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Contact>> GetContactsAsync()
    {
        return await Task.Run(() => Contacts);
    }

    /// </inheritdoc>
    public async Task<Contact?> UpdateContactAsync(Contact contact)
    {
        return await Task.Run(() =>
        {
            var existingContact = Contacts.FirstOrDefault(c => c.Id == contact.Id);
            if (existingContact is not null)
            {
                existingContact.FirstName = contact.FirstName;
                existingContact.LastName = contact.LastName;
                existingContact.Email = contact.Email;
                existingContact.Phone = contact.Phone;
                ContactsChanged?.Invoke(this, EventArgs.Empty);
            }

            return existingContact;
        });
    }
}
