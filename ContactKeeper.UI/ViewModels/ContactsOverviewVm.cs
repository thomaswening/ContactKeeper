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

internal partial class ContactsOverviewVm(IContactService contactService) : ObservableObject
{
    private readonly IContactService contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));

    public event EventHandler? AddContactRequested;
    public event EventHandler<EditContactEventArgs>? EditContactRequested;

    [ObservableProperty]
    private ObservableCollection<ContactVm> contacts = [];

    public async Task InitializeContacts()
    {
        Contacts = new ObservableCollection<ContactVm>(ContactMapper.Map(await contactService.GetContactsAsync()));
    }

    [RelayCommand]
    private void AddContact()
    {
        AddContactRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand(CanExecute = nameof(IsContactNotNull))]
    private async Task DeleteContactAsync(ContactVm contact)
    {
        var ret = await contactService.DeleteContactAsync(contact.Id);
        if (ret is not null)
        {
            Contacts.Remove(contact);
        }
    }

    [RelayCommand(CanExecute = nameof(IsContactNotNull))]
    private void EditContact(ContactVm contact)
    {        
        EditContactRequested?.Invoke(this, new EditContactEventArgs(contact));
    }

    private static bool IsContactNotNull(ContactVm? contact) => contact is not null;
}
