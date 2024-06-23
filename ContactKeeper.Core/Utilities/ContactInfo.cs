using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.Core.Models;

namespace ContactKeeper.Core.Utilities;

/// <summary>
/// Represents information about a contact that can be used for querying or updating purposes.
/// This class allows for operations like searching for or updating a contact based on partial details.
/// The information provided can be incomplete, with any combination of fields being used.
/// Properties of this class are mutable, allowing for adjustments after instantiation if necessary.
/// </summary>
public class ContactInfo
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    /// <summary>
    /// Creates a new <see cref="Contact"/> object from the provided <see cref="ContactQueryInfo"/> object.
    /// </summary>
    /// <param name="contactInfo">The contact query information to create a contact from.</param>
    /// <returns>A new <see cref="Contact"/> object created from the provided contact query information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any of the contact query information properties are null.</exception>
    public Contact ToContact()
    {
        if (FirstName is null)
            throw new InvalidCastException("First name cannot be null.");
        if (LastName is null)
            throw new InvalidCastException("Last name cannot be null.");
        if (Phone is null)
            throw new InvalidCastException("Phone number cannot be null.");
        if (Email is null)
            throw new InvalidCastException("Email address cannot be null.");

        return new Contact(FirstName, LastName, Phone, Email);
    }

    /// <summary>
    /// Creates a new <see cref="ContactInfo"/> object from the provided <see cref="Contact"/> object.
    /// </summary>
    /// <param name="contact">The contact to create a contact information object from.</param>
    /// <returns>A new <see cref="ContactInfo"/> object created from the provided contact.</returns>
    public static ContactInfo FromContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact, nameof(contact));

        return new ContactInfo
        {
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
            Phone = contact.Phone
        };
    }


    /// <summary>
    /// Determines whether the provided <see cref="Contact"/> object matches the information in this <see cref="ContactInfo"/> object.
    /// Only the properties that are not null in this object are used for comparison.
    /// </summary>
    /// <param name="contact">The contact to compare against.</param>
    /// <returns>True if the contact matches the information in this object; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the contact is null.</exception>
    public bool IsMatch(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact, nameof(contact));

        return (FirstName is null || contact.FirstName.Equals(FirstName, StringComparison.OrdinalIgnoreCase)) &&
               (LastName is null || contact.LastName.Equals(LastName, StringComparison.OrdinalIgnoreCase)) &&
               (Email is null || contact.Email.Equals(Email, StringComparison.OrdinalIgnoreCase)) &&
               (Phone is null || contact.Phone.Equals(Phone, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Updates the provided <see cref="Contact"/> object with the information in this <see cref="ContactInfo"/> object.
    /// Only the properties that are not null in this object are used for updating.
    /// </summary>
    /// <param name="contact">The contact to update.</param>
    /// <exception cref="ArgumentNullException">Thrown when the contact is null.</exception>
    public void OverwriteOnto(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact, nameof(contact));

        if (FirstName is not null)
            contact.FirstName = FirstName;
        if (LastName is not null)
            contact.LastName = LastName;
        if (Email is not null)
            contact.Email = Email;
        if (Phone is not null)
            contact.Phone = Phone;
    }
}

