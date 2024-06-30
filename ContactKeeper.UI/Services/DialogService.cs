using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using ContactKeeper.UI.Events;
using ContactKeeper.UI.Factories;
using ContactKeeper.UI.ViewModels;
using ContactKeeper.UI.Views;

namespace ContactKeeper.UI.Services;

/// <summary>
/// Represents a service for managing dialogs.
/// </summary>
internal class DialogService(IDialogHost dialogHost, IModalDialogViewFactory viewFactory)
{
    /// <summary>
    /// Shows a modal dialog asynchronously and returns a value indicating whether the dialog was accepted.
    /// </summary>
    /// <param name="vm">The view model to use for the dialog.</param>
    /// <param name="args">The event arguments to return the result of the dialog.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowModalDialogWithReturnValueAsync(ModalDialogVm vm, AwaitableEventArgs<bool> args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        await ShowModalDialogAsync(vm);
        args.SetResult(vm.DialogResult == true);
    }

    /// <summary>
    /// Shows a modal dialog asynchronously.
    /// </summary>
    /// <param name="viewModel">The view model to use for the dialog.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is <see langword="null"/>.</exception>
    public async Task ShowModalDialogAsync(ModalDialogVm viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel, nameof(viewModel));

        viewModel.CloseRequested += (s, e) => dialogHost.Close();
        var dialog = viewFactory.CreateModalDialogView();
        dialog.DataContext = viewModel;

        await dialogHost.ShowAsync(dialog).ConfigureAwait(false);
    }

}
