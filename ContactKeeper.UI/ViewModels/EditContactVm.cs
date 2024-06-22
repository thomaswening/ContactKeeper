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
internal partial class EditContactVm : ValidatedViewModel
{
    private readonly IEditContactVmValidator validator;
    private readonly IContactService contactService;
    private readonly ILogger logger;

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
    private ContactVm? contact;

    public event EventHandler<AwaitableEventArgs<bool>>? ConfirmContactOverwriteRequested;
    public event EventHandler<AwaitableEventArgs<bool>>? ConfirmCloseWithUnsavedChangesRequested;
    public event EventHandler? CloseRequested;

    public EditContactVm(IContactService contactService, IEditContactVmValidator validator, ILogger logger, ContactVm? contact = null) : base(validator)
    {
        this.contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Contact = contact;

        if (Contact is not null)
        {
            FirstName = Contact.FirstName;
            LastName = Contact.LastName;
            Email = Contact.Email;
            Phone = Contact.Phone;
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
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        if (HasUnsavedChanges)
        {
            // Is there already a contact with the same name?
            var duplicateId = await FindDuplicateContact();

            if (duplicateId is not null && Contact?.Id != duplicateId)
            {
                var isOverwrite = await ConfirmOverwriteAsync();
                if (!isOverwrite)
                    return;

                // Delete the contact being edited and instead update the existing one
                if (Contact is not null)
                {
                    await contactService.DeleteContactAsync(Contact.Id);
                }

                await contactService.UpdateContactAsync(duplicateId.Value, ContactInfo);
            }
            else if (Contact is not null)
            {
                await contactService.UpdateContactAsync(Contact.Id, ContactInfo);
            }
            else
            {
                await contactService.AddContactAsync(ContactInfo);
            }
        }        

        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private async Task<Guid?> FindDuplicateContact()
    {
        var queryInfo = new ContactInfo()
        {
            FirstName = FirstName,
            LastName = LastName
        };

        var duplicateContacts = (await contactService.FindContact(queryInfo)).ToList();

        switch (duplicateContacts.Count)
        {
            case 1:
                return duplicateContacts.First().Id;
            case 0:
                return null;
            default:
                logger.Error($"Found multiple contacts with the same name: {duplicateContacts}.");
                return null;
        }
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

    private bool HasUnsavedChanges => Contact is null || !ContactInfo.IsMatch(ContactMapper.Map(Contact));
}