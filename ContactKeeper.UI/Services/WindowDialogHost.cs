using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using ContactKeeper.UI.Factories;
using ContactKeeper.UI.Views;

namespace ContactKeeper.UI.Services;

/// <summary>
/// This class is responsible for showing dialogs in windows.
/// If a dialog is already open, the new dialog will be queued.
/// </summary>
internal class WindowDialogHost : IDialogHost
{
    private readonly Queue<DialogWindow> dialogQueue = new();

    private DialogWindow? openDialogWindow;

    /// <summary>
    /// Shows the dialog asynchronously. If a dialog is already open, the new dialog will be queued.
    /// </summary>
    /// <param name="dialog">The dialog to show.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialog"/> is <see langword="null"/>.</exception>
    public async Task ShowAsync(IModalDialogView dialog)
    {
        ArgumentNullException.ThrowIfNull(dialog, nameof(dialog));

        var window = new DialogWindow
        {
            Content = dialog
        };

        await ShowDialogWindowAsync(window);
    }

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    public void Close()
    {
        openDialogWindow?.Close();
    }

    private async Task ShowDialogWindowAsync(DialogWindow window)
    {
        if (openDialogWindow is null)
        {
            openDialogWindow = window;
            await ShowDialogAsync(window);
        }
        else
        {
            QueueDialog(window);
        }
    }

    private async Task ShowDialogAsync(DialogWindow window)
    {
        var tcs = new TaskCompletionSource<bool>();

        Application.Current.Dispatcher.Invoke(() =>
        {
            window.Closed += (sender, args) =>
            {
                tcs.SetResult(true);
            };
            window.ShowDialog();
        });

        await tcs.Task;

        await Application.Current.Dispatcher.Invoke(async () =>
        {
            openDialogWindow = null;
            if (dialogQueue.Count != 0)
            {
                var nextDialog = dialogQueue.Dequeue();
                await ShowDialogWindowAsync(nextDialog);
            }
        });
    }

    private void QueueDialog(DialogWindow window)
    {
        dialogQueue.Enqueue(window);
        window.Closed += OnDialogWindowClosed;
    }

    private void OnDialogWindowClosed(object? sender, EventArgs e)
    {
        if (dialogQueue.Count > 0)
        {
            openDialogWindow = dialogQueue.Dequeue();
            openDialogWindow.Closed += OnDialogWindowClosed;
            openDialogWindow.ShowDialog();
        }
        else
        {
            openDialogWindow = null;
        }
    }
}
