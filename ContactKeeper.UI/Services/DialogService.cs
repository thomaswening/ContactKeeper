using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using ContactKeeper.UI.Events;
using ContactKeeper.UI.ViewModels;
using ContactKeeper.UI.Views;

using MaterialDesignThemes.Wpf;

namespace ContactKeeper.UI.Services;

/// <summary>
/// Provides services for displaying dialogs in response to various application events.
/// This service subscribes to specific events and presents modals to the user, allowing for
/// interaction and confirmation for actions such as overwriting contacts or closing with unsaved changes.
/// </summary>
internal class DialogService
{
    private const string DialogIdentifier = "RootDialogHost";

    //public void OnErrorOccurred(ErrorOccurredEvent args)
    //{
    //    MessageBox.Show(args.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //}

    /// <summary>
    /// Handles the <see cref="ConfirmCloseWithUnsavedChangesRequest"/> event by displaying a confirmation dialog
    /// to the user regarding unsaved changes.
    /// </summary>
    /// <param name="request">The event data containing details about the request.</param>
    public async void OnConfirmCloseWithUnsavedChanges(object? sender, AwaitableEventArgs<bool> args)
    {
        var options = new DialogOptions()
        {
            Title = "Unsaved Changes",
            OkText = "Yes",
            CancelText = "No",
        };

        var msg = "You have unsaved changes. Do you want to discard them?";
        var result = await ShowDialogAsync(msg, options);
        args.SetResult(result == true);
    }

    /// <summary>
    /// Handles the <see cref="ConfirmContactOverwriteRequest"/> event by displaying a confirmation dialog
    /// to the user regarding overwriting contact information.
    /// </summary>
    /// <param name="request">The event data containing details about the request.</param>
    public async void OnConfirmContactOverwrite(object? sender, AwaitableEventArgs<bool> args)
    {
        var options = new DialogOptions()
        {
            Title = "Confirm Overwrite",
            OkText = "Yes",
        };

        var msg = "A contact with the same first and last name already exists. Do you want to overwrite it?";
        var result = await ShowDialogAsync(msg, options);
        args.SetResult(result == true);
    }

    /// <summary>
    /// Displays a dialog asynchronously with the specified message and options.
    /// </summary>
    /// <param name="message">The message to be displayed in the dialog.</param>
    /// <param name="dialogOptions">The options configuring the appearance and behavior of the dialog.</param>
    /// <returns>A task that represents the asynchronous operation, with a result indicating the user's action.</returns>
    private static async Task<bool?> ShowDialogAsync(string message, DialogOptions dialogOptions)
    {
        var viewModel = new DialogVm(message, dialogOptions);
        viewModel.CloseRequested += (sender, e) => CloseDialog();

        var dialog = new ModalDialogView { DataContext = viewModel };
        await DialogHost.Show(dialog, DialogIdentifier);

        return viewModel.DialogResult;
    }

    /// <summary>
    /// Closes the currently displayed dialog.
    /// </summary>
    private static void CloseDialog() => DialogHost.Close(DialogIdentifier);
}
