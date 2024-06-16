using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.Core.Models;
using ContactKeeper.Core.Utilities;
using ContactKeeper.UI.ViewModels;

namespace ContactKeeper.UI.Utilities;

/// <summary>
/// Contains helper methods for mapping between <see cref="Contact"/> and <see cref="ContactVm"/> objects.
/// </summary>
internal static class ContactHelper
{
    /// <summary>
    /// Maps a <see cref="Contact"/> object to a <see cref="ContactVm"/> object.
    /// </summary>
    /// <param name="contact">The contact to map.</param>
    /// <returns>A new <see cref="ContactVm"/> object populated with the contact's data.</returns>
    public static ContactVm Map(Contact contact)
    {
        return new ContactVm(contact.Id, contact.FirstName, contact.LastName, contact.Phone, contact.Email);
    }

    /// <summary>
    /// Maps a <see cref="ContactVm"/> object to a <see cref="Contact"/> object.
    /// </summary>
    /// <param name="contactVm">The contact view model to map.</param>
    /// <returns>A new <see cref="Contact"/> object populated with the contact view model's data.</returns>
    public static Contact Map(ContactVm contactVm)
    {
        return new Contact(contactVm.Id, contactVm.FirstName, contactVm.LastName, contactVm.Phone, contactVm.Email);
    }

    /// <summary>
    /// Maps a collection of <see cref="Contact"/> objects to a collection of <see cref="ContactVm"/> objects.
    /// </summary>
    /// <param name="contacts">The contacts to map.</param>
    /// <returns>A new collection of <see cref="ContactVm"/> objects populated with the contacts' data.</returns>
    public static IEnumerable<ContactVm> Map(IEnumerable<Contact> contacts)
    {
        return contacts.Select(contact => Map(contact));
    }

    /// <summary>
    /// Maps a collection of <see cref="ContactVm"/> objects back to a collection of <see cref="Contact"/> objects.
    /// </summary>
    /// <param name="contactVms">The contact view models to map.</param>
    /// <returns>A new collection of <see cref="Contact"/> objects populated with the contact view models' data.</returns>
    public static IEnumerable<Contact> Map(IEnumerable<ContactVm> contactVms)
    {
        return contactVms.Select(contactVm => Map(contactVm));
    }
}