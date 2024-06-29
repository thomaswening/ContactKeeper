using ContactKeeper.UI.Views;

namespace ContactKeeper.UI.Services;

/// <summary>
/// Represents a host for dialogs, e.g. a window or the MaterialDesign DialogHost.
/// </summary>
internal interface IDialogHost
{
    /// <summary>
    /// Closes the dialog.
    /// </summary>
    void Close();

    /// <summary>
    /// Shows the dialog asynchronously.
    /// </summary>
    /// <param name="dialog">The dialog to show.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ShowAsync(ModalDialogView dialog);
}