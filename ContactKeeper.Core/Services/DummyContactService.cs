﻿using System;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyContactService"/> class and generates dummy contacts.
    /// </summary>
    public DummyContactService()
    {
        DummyContactGenerator.GenerateContacts(10);
    }

    /// </inheritdoc>
    public async Task<Contact?> AddContactAsync(ContactInfo contactInfo)
    {
        return await Task.Run(() =>
        {
            var contact = contactInfo.ToContact();
            Contacts.Add(contact);
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
    public static async Task InitializeContactsAsync()
    {
        await Task.Delay(1000);
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Contact>> GetContactsAsync()
    {
        return await Task.Run(() => Contacts);
    }

    /// </inheritdoc>
    public async Task<Contact?> UpdateContactAsync(Guid id, ContactInfo contactInfo)
    {
        return await Task.Run(() =>
        {
            var existingContact = Contacts.FirstOrDefault(c => c.Id == id);
            if (existingContact is not null)
            {
                existingContact.FirstName = contactInfo.FirstName ?? existingContact.FirstName;
                existingContact.LastName = contactInfo.LastName ?? existingContact.LastName;
                existingContact.Email = contactInfo.Email ?? existingContact.Email;
                existingContact.Phone = contactInfo.Phone ?? existingContact.Phone;
            }

            return existingContact;
        });
    }

    /// </inheritdoc>
    public async Task<IEnumerable<Contact>> FindContact(ContactInfo contactInfo)
    {
        return await Task.Run(() =>
        {
            var foundContacts = Contacts.Where(c =>
                (contactInfo.FirstName is null || c.FirstName == contactInfo.FirstName) &&
                (contactInfo.LastName is null || c.LastName == contactInfo.LastName) &&
                (contactInfo.Email is null || c.Email == contactInfo.Email) &&
                (contactInfo.Phone is null || c.Phone == contactInfo.Phone));

            return foundContacts;
        });
    }
}
