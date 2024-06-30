using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Services;
using ContactKeeper.Core.Utilities;
using ContactKeeper.UI.Events;
using ContactKeeper.UI.ViewModels;

using Serilog;

namespace ContactKeeper.UI.Utilities;

/// <summary>
/// Manages contact operations for the <see cref="EditContactVm"/> view model.
/// </summary>
/// <param name="contactService">The contact service to use for persistence operations.</param>
/// <param name="logger">The logger to use.</param>
internal class EditContactManager(IContactService contactService, ILogger logger) : IEditContactManager
{
    /// <inheritdoc />
    public async Task HandleDuplicateContact(Guid duplicateId, ContactInfo contactInfoToSave, ContactVm? contactToUpdate)
    {
        ArgumentNullException.ThrowIfNull(contactInfoToSave, nameof(contactInfoToSave));

        logger.Information($"Handling duplicate contacts. Found duplicate ID: {duplicateId}. " +
            $"Contact to update: {contactToUpdate?.Id}.");

        if (contactToUpdate is not null)
        {
            await contactService.DeleteContactAsync(contactToUpdate.Id);
        }

        await contactService.UpdateContactAsync(duplicateId, contactInfoToSave);
    }

    /// <inheritdoc />
    public async Task<Guid?> FindFullNameDuplicateAsync(string firstName, string lastName)
    {
        ArgumentNullException.ThrowIfNull(firstName, nameof(firstName));
        ArgumentNullException.ThrowIfNull(lastName, nameof(lastName));
        logger.Information($"Searching for duplicate contact with first name: {firstName} and last name: {lastName}.");

        if (string.IsNullOrWhiteSpace(firstName))
        {
            logger.Error($"Empty first name provided to {nameof(FindFullNameDuplicateAsync)}.");
            return null;
        }

        var queryInfo = new ContactInfo()
        {
            FirstName = firstName,
            LastName = lastName
        };

        var duplicateContacts = (await contactService.FindContactAsync(queryInfo)).ToList();

        switch (duplicateContacts.Count)
        {
            case 1:
                return duplicateContacts.First().Id;
            case 0:
                return null;
            default:
                logger.Error($"Found multiple contacts with the same name: {duplicateContacts}.");
                throw new InvalidOperationException("Multiple contacts with the same full name found. Cannot save any changes.");
        }
    }

    /// <inheritdoc />
    public async Task UpdateContactAsync(Guid id, ContactInfo contactInfo)
    {
        await contactService.UpdateContactAsync(id, contactInfo);
    }

    /// <inheritdoc />
    public async Task AddContactAsync(ContactInfo contactInfo)
    {
        await contactService.AddContactAsync(contactInfo);
    }

    /// <inheritdoc />
    public bool CheckForUnsavedChanges(ContactInfo contactInfoToCheck, ContactVm? contactToCompare)
    {
        ArgumentNullException.ThrowIfNull(contactInfoToCheck, nameof(contactInfoToCheck));
        return contactToCompare is null || !contactInfoToCheck.IsMatch(ContactMapper.Map(contactToCompare));
    }
}
