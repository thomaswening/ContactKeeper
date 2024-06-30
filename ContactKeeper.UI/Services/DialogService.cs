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

/// <inheritdoc/>
internal class DialogService(IDialogHost dialogHost, IModalDialogViewFactory viewFactory) : IDialogService
{
    /// <inheritdoc/>
    public async Task ShowModalDialogWithReturnValueAsync(ModalDialogVm viewModel, AwaitableEventArgs<bool> args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        await ShowModalDialogAsync(viewModel);
        args.SetResult(viewModel.DialogResult == true);
    }

    /// <inheritdoc/>
    public async Task ShowModalDialogAsync(ModalDialogVm viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel, nameof(viewModel));

        viewModel.CloseRequested += (s, e) => dialogHost.Close();
        var dialog = viewFactory.CreateModalDialogView();
        dialog.DataContext = viewModel;

        await dialogHost.ShowAsync(dialog).ConfigureAwait(false);
    }

}
