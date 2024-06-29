using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactKeeper.Core.Models;
using ContactKeeper.Core.Utilities;

namespace ContactKeeper.Core.Interfaces;
/// <summary>
/// Represents a service for managing contacts.
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Occurs when the contacts have changed.
    /// </summary>
    event EventHandler? ContactsChanged;

    /// <summary>
    /// Gets the current contacts.
    /// </summary>
    /// <returns>A task returning an <see cref="IEnumerable{Contact}"/> of the contacts.</returns>
    Task<IEnumerable<Contact>> GetContactsAsync();

    /// <summary>
    /// Gets a contact by ID.
    /// </summary>
    /// <param name="id">The ID of the contact to get.</param>
    /// <returns>A task returning the contact with the specified ID, or <see langword="null"/> if not found.</returns>
    Task<Contact?> GetContactAsync(Guid id);

    /// <summary>
    /// Finds a contact by contact information.
    /// </summary>
    /// <param name="contactInfo">The contact information to search for.
    /// Only non-<see langword="null"/> properties are used for searching.</param>
    /// <returns>A task returning an <see cref="IEnumerable{Contact}"/> of the contacts found.
    /// If no contacts are found, an empty collection is returned.</returns>
    Task<IEnumerable<Contact>> FindContactAsync(ContactInfo contactInfo);

    /// <summary>
    /// Adds a contact.
    /// </summary>
    /// <param name="contact">The contact to add.</param>
    /// <returns>A task returning the added contact, or <see langword="null"/> if an error occurs.</returns>
    Task<Contact?> AddContactAsync(ContactInfo contactInfo);

    /// <summary>
    /// Updates a contact.
    /// </summary>
    /// <param name="id">The GUID of the contact to update.</param>
    /// <param name="updateInfo">The information to update the contact with.</param>
    /// <returns>A task returning the updated contact, or <see langword="null"/> if not found.</returns>
    Task<Contact?> UpdateContactAsync(Guid id, ContactInfo updateInfo);

    /// <summary>
    /// Deletes a contact.
    /// </summary>
    /// <param name=")">The GUID of the contact to delete.</param>
    /// <returns>A task returning the ID of deleted contract, or <see langword="null"/> if not found.</returns>
    Task<Guid?> DeleteContactAsync(Guid id);
}
