using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoBogus;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Models;
using ContactKeeper.Core.Services;
using ContactKeeper.UI.Utilities;
using ContactKeeper.UI.ViewModels;

using NSubstitute;

using NUnit.Framework;

namespace ContactKeeper.UI.Tests.ViewModels;

[TestFixture]
internal class ContactsOverviewVmTests
{
    private const int NumberOfFakeContacts = 10;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private IContactService contactService;
    private ContactsOverviewVm contactsOverviewVm;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void SetUp()
    {
        contactService = Substitute.For<IContactService>();
        contactsOverviewVm = new ContactsOverviewVm(contactService);
    }

    [Test]
    public async Task InitializeContacts_WhenCalled_InitializesTheListOfContacts()
    {
        // Arrange
        var contacts = AutoFaker.Generate<Contact>(NumberOfFakeContacts);
        var contactVms = contacts.Select(c => ContactMapper.Map(c)).ToList();

        contactService.GetContactsAsync().Returns(contacts);

        // Act
        await contactsOverviewVm.InitializeContacts();

        // Assert
        Assert.That(contactsOverviewVm.Contacts, Has.Count.EqualTo(NumberOfFakeContacts));
        Assert.That(contactsOverviewVm.Contacts.Select(c => c.Id), Is.EquivalentTo(contactVms.Select(c => c.Id)));
    }

    [Test]
    public async Task InitializeContacts_WhenContactServiceThrowsException_SetsContactsToEmptyListAndInvokesErrorOccured()
    {
        // Arrange
        bool wasInvoked = false;
        contactsOverviewVm.ErrorOccured += (_, _) => wasInvoked = true;
        contactService.When(c => c.GetContactsAsync()).Throw<Exception>();

        // Act
        await contactsOverviewVm.InitializeContacts();

        // Assert
        Assert.That(contactsOverviewVm.Contacts, Is.Empty);
        Assert.That(wasInvoked, Is.True);
    }

    [Test]
    public void AddContactCommand_WhenExecuted_InvokesAddContactRequestedEvent()
    {
        // Arrange
        var wasInvoked = false;
        contactsOverviewVm.AddContactRequested += (sender, args) => wasInvoked = true;

        // Act
        contactsOverviewVm.AddContactCommand.Execute(null);

        // Assert
        Assert.That(wasInvoked, Is.True);
    }

    [Test]
    public async Task DeleteContactCommand_WhenExecuted_InvokesDeleteContactAsyncOnContactService()
    {
        // Arrange
        var contactVm = AutoFaker.Generate<ContactVm>();

        contactsOverviewVm.Contacts.Add(contactVm);

        // Act
        await contactsOverviewVm.DeleteContactCommand.ExecuteAsync(contactVm);

        // Assert
        await contactService.Received().DeleteContactAsync(contactVm.Id);
    }

    [Test]
    public async Task DeleteContactCommand_WhenContactServiceThrowsException_InvokesErrorOccuredEvent()
    {
        // Arrange
        var contactVm = AutoFaker.Generate<ContactVm>();
        contactService.When(c => c.DeleteContactAsync(contactVm.Id)).Throw<Exception>();

        var wasInvoked = false;
        contactsOverviewVm.ErrorOccured += (sender, message) => wasInvoked = true;

        // Act
        await contactsOverviewVm.DeleteContactCommand.ExecuteAsync(contactVm);

        // Assert
        Assert.That(wasInvoked, Is.True);
    }

    [Test]
    public void DeleteContactCommand_WhenContactIsNull_CannotExecute()
    {
        // Act
        var canExecute = contactsOverviewVm.DeleteContactCommand.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.False);
    }

    [Test] 
    public void EditContactCommand_WhenExecuted_InvokesEditContactRequestedEvent()
    {
        // Arrange
        var contactVm = AutoFaker.Generate<ContactVm>();
        var wasInvoked = false;
        contactsOverviewVm.EditContactRequested += (_, _) => wasInvoked = true;

        // Act
        contactsOverviewVm.EditContactCommand.Execute(contactVm);

        // Assert
        Assert.That(wasInvoked, Is.True);
    }

    [Test]
    public void EditContactCommand_WhenContactIsNull_CannotExecute()
    {
        // Act
        var canExecute = contactsOverviewVm.EditContactCommand.CanExecute(null);

        // Assert
        Assert.That(canExecute, Is.False);
    }

    [Test]
    public async Task ContactsChanged_WhenInvoked_ReInitializesContacts()
    {
        // Arrange
        var contacts = AutoFaker.Generate<Contact>(NumberOfFakeContacts);
        contactService.GetContactsAsync().Returns(contacts);

        // Act
        contactService.ContactsChanged += Raise.EventWith(new object(), EventArgs.Empty);

        // Assert
        await contactService.Received(1).GetContactsAsync();
        Assert.That(contactsOverviewVm.Contacts.Count, Is.EqualTo(NumberOfFakeContacts));
    }
}
