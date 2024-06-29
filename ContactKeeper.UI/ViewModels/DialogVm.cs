using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ContactKeeper.UI.ViewModels;

public partial class ModalDialogVm : ObservableObject
{
    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? message;

    [ObservableProperty]
    private string? okText;

    [ObservableProperty]
    private string? cancelText;

    public event EventHandler? CloseRequested;

    public ModalDialogVm(string message, DialogOptions? dialogOptions = null, Action? okAction = null, Action? cancelAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
        
        // Use default options if none are provided
        dialogOptions ??= new DialogOptions();

        Message = message;
        Title = dialogOptions.Title;
        OkText = dialogOptions.OkText;
        CancelText = dialogOptions.CancelText;

        OkCommand = okAction is null
            ? new RelayCommand(() => SetDialogResult(true))
            : new RelayCommand(() =>
            {
                okAction.Invoke();
                SetDialogResult(true);
            });

        CancelCommand = cancelAction is null
            ? new RelayCommand(() => SetDialogResult(false))
            : new RelayCommand(() =>
            {
                cancelAction.Invoke();
                SetDialogResult(false);
            });
    }

    public RelayCommand OkCommand { get; }
    public RelayCommand CancelCommand { get; }

    public bool? DialogResult { get; private set; }

    private void SetDialogResult(bool result)
    {
        DialogResult = result;
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
