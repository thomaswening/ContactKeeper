using ContactKeeper.UI.Events;
using ContactKeeper.UI.ViewModels;

namespace ContactKeeper.UI.Services;

/// <summary>
/// Represents a service for managing dialogs.
/// </summary>
internal interface IDialogService
{
    /// <summary>
    /// Shows the about section.
    /// </summary>
    /// <param name="viewModel">The view model to use for the about section.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ShowAboutSectionAsync(AboutSectionVm viewModel);

    /// <summary>
    /// Shows a modal dialog asynchronously.
    /// </summary>
    /// <param name="viewModel">The view model to use for the dialog.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is <see langword="null"/>.</exception>
    Task ShowModalDialogAsync(ModalDialogVm viewModel);

    /// <summary>
    /// Shows a modal dialog asynchronously and returns a value indicating whether the dialog was accepted.
    /// </summary>
    /// <param name="viewModel">The view model to use for the dialog.</param>
    /// <param name="args">The event arguments to return the result of the dialog.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ShowModalDialogWithReturnValueAsync(ModalDialogVm viewModel, AwaitableEventArgs<bool> args);
}