using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.UI.Utilities;

using Serilog;

namespace ContactKeeper.UI.ViewModels;

internal partial class ContactsOverviewVm : ObservableObject
{
    private readonly IContactService contactService;

    public event EventHandler? AddContactRequested;
    public event EventHandler<EditContactEventArgs>? EditContactRequested;

    [ObservableProperty]
    private List<ContactVm> contacts = [];

    public ContactsOverviewVm(IContactService contactService)
    {
        this.contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
        contactService.ContactsChanged += OnContactsChanged;
    }

    public async Task InitializeContacts()
    {
        Contacts = ContactMapper.Map(await contactService.GetContactsAsync()).ToList();
    }

    [RelayCommand]
    private void AddContact()
    {
        AddContactRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand(CanExecute = nameof(IsContactNotNull))]
    private async Task DeleteContactAsync(ContactVm contact)
    {
        await contactService.DeleteContactAsync(contact.Id);
    }

    [RelayCommand(CanExecute = nameof(IsContactNotNull))]
    private void EditContact(ContactVm contact)
    {        
        EditContactRequested?.Invoke(this, new EditContactEventArgs(contact));
    }

    private static bool IsContactNotNull(ContactVm? contact) => contact is not null;

    private async void OnContactsChanged(object? sender, EventArgs e) => await InitializeContacts();

}
