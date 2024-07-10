using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.UI.Events;
using ContactKeeper.UI.Services;
using ContactKeeper.UI.Validation;
using ContactKeeper.UI.ViewModels;

using Serilog;

namespace ContactKeeper.UI.Utilities;

/// <summary>
/// Factory for creating view models.
/// </summary>
internal class ViewModelFactory
{
    private readonly INavigationService navigationService;
    private readonly IContactService contactService;
    private readonly IDialogService dialogService;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
    /// </summary>
    /// <param name="navigationService">The navigation service to use.</param>
    /// <param name="contactService">The contact service to use.</param>
    /// <param name="dialogService">The dialog service to use.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="navigationService"/>, <paramref name="contactService"/>, 
    /// <paramref name="dialogService"/>, or <paramref name="logger"/> is null.</exception>
    public ViewModelFactory(INavigationService navigationService, IContactService contactService, IDialogService dialogService, ILogger logger)
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

    /// <summary>
    /// Creates a new <see cref="MainWindowVm"/> view model with an initialized <see cref="ContactsOverviewVm"/> as the current view model
    /// and registers the <see cref="ContactsOverviewVm"/> view model with the <see cref="NavigationService"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the created view model.</returns>
    public async Task<MainWindowVm> CreateMainWindowVmAsync()
    {        
        var contactsOverviewVm = await CreateContactsOverviewVmAsync();
        navigationService.RegisterViewModel(contactsOverviewVm);

        var mainWindowVm = new MainWindowVm(contactsOverviewVm);
        mainWindowVm.DefaultViewModelRequested += async (s, e) => mainWindowVm.CurrentViewModel = await CreateContactsOverviewVmAsync();
        mainWindowVm.AboutSectionRequested += async (s, e) => await dialogService.ShowAboutSectionAsync(CreateAboutSectionVm());
        navigationService.CurrentViewModelChanged += (s, e) => mainWindowVm.CurrentViewModel = navigationService.CurrentViewModel;

        return mainWindowVm;
    }

    /// <summary>
    /// Creates a new <see cref="ContactsOverviewVm"/> view model with the provided <see cref="IContactService"/>, 
    /// initializes the contacts, and registers the view model with the <see cref="NavigationService"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the created view model.</returns>
    public async Task<ContactsOverviewVm> CreateContactsOverviewVmAsync()
    {
        var viewModel = new ContactsOverviewVm(contactService);
        viewModel.AddContactRequested += (s, e) => navigationService.RegisterViewModel(CreateEditContactVm());
        viewModel.EditContactRequested += (s, e) => navigationService.RegisterViewModel(CreateEditContactVm(e.Contact));
        AttachErrorHandler(viewModel);

        await viewModel.InitializeContacts();
        return viewModel;
    }

    /// <summary>
    /// Creates a new <see cref="EditContactVm"/> view model with the provided <see cref="ContactVm"/> 
    /// and registers the view model with the <see cref="NavigationService"/> and subscribes to the dialog service events.
    /// </summary>
    /// <param name="contact">The contact to edit. If null, a new contact will be created.</param>
    /// <returns>The created view model.</returns>
    public EditContactVm CreateEditContactVm(ContactVm? contact = null)
    {
        var contactManager = new EditContactManager(contactService, logger);
        var validator = new EditContactVmValidator();
        var viewModel = new EditContactVm(contactManager, validator, contact);

        viewModel.ConfirmContactOverwriteRequested += async (s, args) =>
        {
            await ShowModalDialogAsync(ModalDialogViewModelFactory.CreateConfirmOverwriteDialogVm, args);
        };

        viewModel.ConfirmCloseWithUnsavedChangesRequested += async (s, args) =>
        {
            await ShowModalDialogAsync(ModalDialogViewModelFactory.CreateConfirmCloseWithUnsavedChangesDialogVm, args).ConfigureAwait(false);
        };

        viewModel.CloseRequested += (s, e) => navigationService.UnregisterViewModel(viewModel);
        AttachErrorHandler(viewModel);

        return viewModel;
    }

    /// <summary>
    /// Creates a new <see cref="AboutSectionVm"/> view model.
    /// </summary>
    public AboutSectionVm CreateAboutSectionVm() => new();


    private async Task ShowModalDialogAsync(Func<ModalDialogVm> creationMethod, AwaitableEventArgs<bool> args)
    {
        var vm = creationMethod();
        await dialogService.ShowModalDialogWithReturnValueAsync(vm, args).ConfigureAwait(false);
    }

    private void AttachErrorHandler(IErrorPublisher viewModel)
    {
        viewModel.ErrorOccured += async (s, e) =>
        {
            var vm = ModalDialogViewModelFactory.CreateErrorDialogVm(e);
            await dialogService.ShowModalDialogAsync(vm).ConfigureAwait(false);
        };
    }
}