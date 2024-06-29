
using ContactKeeper.UI.ViewModels;

namespace ContactKeeper.UI.Utilities;

internal static class ModalDialogViewModelFactory
{
    public static ModalDialogVm CreateConfirmCloseWithUnsavedChangesDialogVm()
    {
        var options = new DialogOptions()
        {
            Title = "Unsaved Changes",
            OkText = "Yes",
            CancelText = "No",
        };

        var msg = "You have unsaved changes. Do you want to discard them?";

        return new ModalDialogVm(msg, options);
    }

    public static ModalDialogVm CreateConfirmOverwriteDialogVm()
    {
        var options = new DialogOptions()
        {
            Title = "Confirm Overwrite",
            OkText = "Yes",
        };

        var msg = "A contact with the same first and last name already exists. Do you want to overwrite it?";

        return new ModalDialogVm(msg, options);
    }

    public static ModalDialogVm CreateErrorDialogVm(string errorMessage)
    {
        var options = new DialogOptions()
        {
            Title = "Error",
            OkText = "OK",
            ShowCancelButton = false,
        };

        return new ModalDialogVm(errorMessage, options);
    }
}