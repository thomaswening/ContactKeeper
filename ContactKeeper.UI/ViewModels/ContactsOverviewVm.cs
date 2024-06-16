using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ContactKeeper.UI.ViewModels;
internal partial class ContactsOverviewVm : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ContactVm> contacts = [];


    [RelayCommand]
    private void AddContact()
    {
        throw new NotImplementedException();
    }

    [RelayCommand(CanExecute = nameof(IsContactNotNull))]
    private void DeleteContact(ContactVm contact)
    {
        throw new NotImplementedException();

    }

    [RelayCommand(CanExecute = nameof(IsContactNotNull))]
    private void EditContact(ContactVm contact)
    {
        throw new NotImplementedException();

    }

    private static bool IsContactNotNull(ContactVm? contact) => contact is not null;
}
