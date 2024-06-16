using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.Core.Models;

namespace ContactKeeper.Core.Interfaces;

/// <summary>
/// Represents a repository for managing contacts.
/// </summary>
public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetContactsAsync();
    Task SaveContactsAsync(IEnumerable<Contact> contacts);
}
