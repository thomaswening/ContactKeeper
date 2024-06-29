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

    public async Task<MainWindowVm> CreateMainWindowVmAsync()
    {        
        var contactsOverviewVm = await CreateContactsOverviewVmAsync();
        navigationService.RegisterViewModel(contactsOverviewVm);

        var mainWindowVm = new MainWindowVm(contactsOverviewVm);
        navigationService.CurrentViewModelChanged += (s, e) => mainWindowVm.CurrentViewModel = navigationService.CurrentViewModel;

        return mainWindowVm;
    }

    public async Task<ContactsOverviewVm> CreateContactsOverviewVmAsync()
    {
        var viewModel = new ContactsOverviewVm(contactService);
        viewModel.AddContactRequested += (s, e) => navigationService.RegisterViewModel(CreateEditContactVm());
        viewModel.EditContactRequested += (s, e) => navigationService.RegisterViewModel(CreateEditContactVm(e.Contact));

        await viewModel.InitializeContacts();
        return viewModel;
    }

    public EditContactVm CreateEditContactVm(ContactVm? contact = null)
    {
        var contactManager = new EditContactManager(contactService, logger);
        var validator = new EditContactVmValidator();
        var viewModel = new EditContactVm(contactManager, validator, contact);

        viewModel.ConfirmContactOverwriteRequested += dialogService.OnConfirmContactOverwrite;
        viewModel.ConfirmCloseWithUnsavedChangesRequested += dialogService.OnConfirmCloseWithUnsavedChanges;
        viewModel.CloseRequested += (s, e) => navigationService.UnregisterViewModel(viewModel);

        return viewModel;
    }
}