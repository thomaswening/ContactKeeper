using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.UI.Services;
using ContactKeeper.UI.Validation;
using ContactKeeper.UI.ViewModels;

using Serilog;

namespace ContactKeeper.UI.Utilities;

internal class ViewModelFactory(IContactService contactService, DialogService dialogService, ILogger logger)
{
    public async Task<ContactsOverviewVm> CreateContactsOverviewVmAsync()
    {
        var vm = new ContactsOverviewVm(contactService);
        await vm.InitializeContacts();
        return vm;
    }

    public EditContactVm CreateEditContactVm(ContactVm? contact = null)
    {
        var validator = new EditContactVmValidator();
        var viewModel = new EditContactVm(contactService, validator, logger, contact);

        viewModel.ConfirmContactOverwriteRequested += dialogService.OnConfirmContactOverwrite;
        viewModel.ConfirmCloseWithUnsavedChangesRequested += dialogService.OnConfirmCloseWithUnsavedChanges;

        return viewModel;
    }
}