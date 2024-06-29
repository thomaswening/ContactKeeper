using ContactKeeper.Core.Utilities;
using ContactKeeper.UI.ViewModels;
using ContactKeeper.Core.Services;

namespace ContactKeeper.UI.Utilities;

/// <summary>
/// Manages contact operations for the <see cref="EditContactVm"/> view model.
/// </summary>
internal interface IEditContactManager
{
    /// <inheritdoc cref="IContactService.AddContactAsync"/>
    Task AddContactAsync(ContactInfo contactInfo);

    /// <summary>
    /// Checks if a contact with the provided full name already exists.
    /// </summary>
    /// <param name="firstName">The first name of the contact.</param>
    /// <param name="lastName">The last name of the contact.</param>
    /// <returns>The ID of the duplicate contact if found, otherwise null.
    /// If <paramref name="firstName"/> or <paramref name="lastName"/> is empty or whitespace, null is returned and an error is logged.</returns>
    /// <exception cref="InvalidOperationException">Thrown when multiple contacts with the same full name are found.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="firstName"/> or <paramref name="lastName"/> is null.</exception>
    Task<Guid?> FindFullNameDuplicateAsync(string firstName, string lastName);

    /// <summary>
    /// Handles the case when a new contact is being added and a full name duplicate contact was found
    /// or when an existing contact is being updated and a full name duplicate contact of the new contact info was found.
    /// The duplicate contact will be updated with the new contact info 
    /// and if a contact is being updated, the old contact will be deleted.
    /// </summary>
    /// <param name="duplicateId">The ID of the duplicate contact.</param>
    /// <param name="contactInfoToSave">The contact information to save.</param>
    /// <param name="contactToUpdate">The contact to update. If null, no contact will be updated.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contactInfoToSave"/> is null.</exception>
    Task HandleDuplicateContact(Guid duplicateId, ContactInfo contactInfoToSave, ContactVm? contactToUpdate);

    /// <inheritdoc cref="IContactService.UpdateContactAsync"/>
    Task UpdateContactAsync(Guid id, ContactInfo contactInfo);

    /// <summary>
    /// Used to check if there are unsaved changes in the contact information.
    /// </summary>
    /// <param name="contactInfoToCheck">The contact information to check.</param>
    /// <param name="contactToCompare">The contact view model to compare with. May be null.</param>
    /// <returns>True if there are unsaved changes or if <paramref name="contactToCompare"/> is null, 
    /// otherwise false.</returns>
    public bool CheckForUnsavedChanges(ContactInfo contactInfoToCheck, ContactVm? contactToCompare);
}