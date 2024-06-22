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

internal class ViewModelFactory
{
    private readonly NavigationService navigationService;
    private readonly IContactService contactService;
    private readonly DialogService dialogService;
    private readonly ILogger logger;

    public ViewModelFactory(NavigationService navigationService, IContactService contactService, DialogService dialogService, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(navigationService, nameof(navigationService));
        ArgumentNullException.ThrowIfNull(contactService, nameof(contactService));
        ArgumentNullException.ThrowIfNull(dialogService, nameof(dialogService));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        this.navigationService = navigationService;
        this.contactService = contactService;
        this.dialogService = dialogService;
        this.logger = logger;
    }
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