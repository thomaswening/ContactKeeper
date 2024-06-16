using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.UI.ViewModels;

namespace ContactKeeper.UI.Utilities;

internal class ViewModelFactory(IContactService contactService)
{
    public async Task<ContactsOverviewVm> CreateContactsOverviewVmAsync()
    {
        var vm = new ContactsOverviewVm(contactService);
        await vm.InitializeContacts();
        return vm;
    }
}