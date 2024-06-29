using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using ContactKeeper.UI.Views;

using MaterialDesignThemes.Wpf;

namespace ContactKeeper.UI.Services;

/// <summary>
/// This class is responsible for showing dialogs using the Material Design dialog host.
/// </summary>
/// <param name="dialogHostIdentifier">The identifier of the dialog host to use.</param>
class MaterialDesignDialogHost(string dialogHostIdentifier) : IDialogHost
{
    /// <summary>
    /// Closes the dialog.
    /// </summary>
    public void Close()
    {
        DialogHost.Close(dialogHostIdentifier);
    }

    /// <summary>
    /// Shows the dialog asynchronously.
    /// </summary>
    /// <param name="dialog">The dialog to show.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowAsync(ModalDialogView dialog)
    {
        await Application.Current.Dispatcher.Invoke(async () =>
        {
            await DialogHost.Show(dialog, dialogHostIdentifier);
        });
    }
}
