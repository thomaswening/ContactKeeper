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

    [ObservableProperty]
    private ObservableCollection<ContactVm> contacts = [];

    public async Task InitializeContacts()
    {
        Contacts = new ObservableCollection<ContactVm>(ContactHelper.Map(await contactService.GetContactsAsync()));
    }

    [RelayCommand]
    private void AddContact()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();

    }

    private static bool IsContactNotNull(ContactVm? contact) => contact is not null;
}
