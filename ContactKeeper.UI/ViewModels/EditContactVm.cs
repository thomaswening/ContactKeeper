using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Models;
using ContactKeeper.Core.Utilities;
using ContactKeeper.UI.Events;
using ContactKeeper.UI.Utilities;

using ContactKeeper.UI.Validation;

using Serilog;
using Serilog.Core;

namespace ContactKeeper.UI.ViewModels;
internal partial class EditContactVm : ValidatedViewModel, IErrorPublisher
{
    private readonly IEditContactVmValidator validator;
    private readonly IEditContactManager contactManager;

    private ContactInfo ContactInfo => new()
    {
        Email = Email ?? string.Empty,
        FirstName = FirstName ?? string.Empty,
        LastName = LastName ?? string.Empty,
        Phone = Phone ?? string.Empty
    };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ContactInfo))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string firstName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ContactInfo))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string lastName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ContactInfo))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ContactInfo))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string phone = string.Empty;

    [ObservableProperty]
    private ContactVm? contactToEdit;

    public event EventHandler<AwaitableEventArgs<bool>>? ConfirmContactOverwriteRequested;
    public event EventHandler<AwaitableEventArgs<bool>>? ConfirmCloseWithUnsavedChangesRequested;
    public event EventHandler<string>? ErrorOccured;
    public event EventHandler? CloseRequested;

    public EditContactVm(IEditContactManager contactManager, IEditContactVmValidator validator, ContactVm? contact = null) : base(validator)
    {
        ArgumentNullException.ThrowIfNull(contactManager, nameof(contactManager));
        ArgumentNullException.ThrowIfNull(validator, nameof(validator));

        this.contactManager = contactManager;
        this.validator = validator;

        ContactToEdit = contact;

        if (ContactToEdit is not null)
        {
            FirstName = ContactToEdit.FirstName;
            LastName = ContactToEdit.LastName;
            Email = ContactToEdit.Email;
            Phone = ContactToEdit.Phone;
        }

        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            // Validate for Lastname too in case user skipped the FirstName field
            case nameof(FirstName):
            case nameof(LastName):
                validator.ValidateFirstName(FirstName);
                break;

            case nameof(Email):
                validator.ValidateFirstName(FirstName);
                validator.ValidateEmail(Email);
                break;

            case nameof(Phone):
                validator.ValidateFirstName(FirstName);
                validator.ValidatePhone(Phone);
                break;

            default:
                break;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        if (HasErrors) return;

        if (!HasUnsavedChanges)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
            return;
        }

        try
        {
            var duplicateId = await contactManager.FindFullNameDuplicateAsync(FirstName, LastName);

            if (duplicateId is not null && ContactToEdit?.Id != duplicateId)
            {
                var isOverwrite = await ConfirmOverwriteAsync();
                if (!isOverwrite)
                    return;

                await contactManager.HandleDuplicateContact(duplicateId.Value, ContactInfo, ContactToEdit);
            }
            else if (ContactToEdit is not null)
            {
                await contactManager.UpdateContactAsync(ContactToEdit.Id, ContactInfo);
            }
            else
            {
                await contactManager.AddContactAsync(ContactInfo);
            }
        }
        catch (Exception ex)
        {
            var msg = $"{ex.Message} Cannot save contact.";
            ErrorOccured?.Invoke(this, ex.Message);
        }

        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private async Task<bool> ConfirmOverwriteAsync()
    {
        var args = new AwaitableEventArgs<bool>();
        ConfirmContactOverwriteRequested?.Invoke(this, args);

        return await args.Task;
    }

    private bool CanSave() => !HasErrors;

    [RelayCommand]
    private async Task CancelAsync()
    {
        if (HasUnsavedChanges)
        {
            var args = new AwaitableEventArgs<bool>();
            ConfirmCloseWithUnsavedChangesRequested?.Invoke(this, args);

            var isClose = await args.Task;
            if (!isClose) return;
        }

        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private bool HasUnsavedChanges => contactManager.CheckForUnsavedChanges(ContactInfo, ContactToEdit);
}