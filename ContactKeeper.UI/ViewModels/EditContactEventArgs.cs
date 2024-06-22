namespace ContactKeeper.UI.ViewModels;

/// <summary>
/// Event arguments for the <see cref="EditContactRequested"/> event.
/// </summary>
/// <param name="contact">The contact to edit.</param>
public class EditContactEventArgs(ContactVm contact)
{
    /// <summary>
    /// Gets the contact to edit.
    /// </summary>
    public ContactVm Contact { get; } = contact;
}