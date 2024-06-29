using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoBogus;

using ContactKeeper.Core.Exceptions;
using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Models;
using ContactKeeper.Core.Services;
using ContactKeeper.Core.Tests.Fakers;
using ContactKeeper.Core.Utilities;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

using Serilog;

namespace ContactKeeper.Core.Tests.Services;

[TestFixture]
public class ContactServiceTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ILogger logger;
    private IContactRepository repository;
    private ContactService contactService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void SetUp()
    {
        logger = Substitute.For<ILogger>();
        repository = Substitute.For<IContactRepository>();
        contactService = new ContactService(logger, repository);
    }

    [Test]
    public async Task AddContactAsync_WhenCalled_ReturnsAddedContact()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);

        // Act
        var result = await contactService.AddContactAsync(contactInfo);

        // Assert
        Assert.That(result!, Is.Not.Null);
        Assert.That(result!.FirstName, Is.EqualTo(contactInfo.FirstName));
        Assert.That(result!.LastName, Is.EqualTo(contactInfo.LastName));
        Assert.That(result!.Email, Is.EqualTo(contactInfo.Email));
        Assert.That(result!.Phone, Is.EqualTo(contactInfo.Phone));

        await repository.Received(1).SaveContactsAsync(Arg.Any<List<Contact>>());
    }

    [Test]
    public async Task AddContactAsync_WhenCalled_AddsContact()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);

        // Act
        await contactService.AddContactAsync(contactInfo);
        var result = contactService.Contacts;

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].FirstName, Is.EqualTo(contactInfo.FirstName));
        Assert.That(result[0].LastName, Is.EqualTo(contactInfo.LastName));
        Assert.That(result[0].Email, Is.EqualTo(contactInfo.Email));
        Assert.That(result[0].Phone, Is.EqualTo(contactInfo.Phone));

        await repository.Received(1).SaveContactsAsync(Arg.Any<List<Contact>>());
    }

    [Test]
    public void AddContactAsync_WhenRepositoryFailsToSave_ThrowsException()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        repository.SaveContactsAsync(Arg.Any<List<Contact>>()).Throws(new Exception());

        // Act
        async Task AddContactAsync() => await contactService.AddContactAsync(contactInfo);

        // Assert
        Assert.That(AddContactAsync, Throws.TypeOf<Exception>());
    }

    [Test]
    public async Task DeleteContactAsync_WhenCalled_DeletesContactButNotOthers()
    {
        // Arrange
        var numberOfRandomContacts = 10;
        var randomContactInfos = ContactInfoFaker.GetFakes(numberOfRandomContacts, ContactInfoFaker.PropertiesNotNull);
        await AddContacts(randomContactInfos);

        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var contact = await contactService.AddContactAsync(contactInfo);

        // Act
        await contactService.DeleteContactAsync(contact!.Id);
        var result = contactService.Contacts;

        // Assert
        Assert.That(result, Has.Count.EqualTo(randomContactInfos.Count()));
        Assert.That(result.Select(c => c.Id), Has.No.Member(contact.Id));

        // receives 2 extra calls because the first call is to add the contact to be deleted
        // and the second call is to save the contacts after deleting the contact
        await repository.Received(numberOfRandomContacts + 2).SaveContactsAsync(Arg.Any<List<Contact>>());
    }

    [Test]
    public async Task DeleteContactAsync_WhenCalled_ReturnsDeletedId()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var contact = await contactService.AddContactAsync(contactInfo);

        // Act
        var result = await contactService.DeleteContactAsync(contact!.Id);

        // Assert
        Assert.That(result, Is.EqualTo(contact.Id));
    }

    [Test]
    public async Task DeleteContactAsync_WhenCalledWithNonExistentId_ReturnsNull()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        await contactService.AddContactAsync(contactInfo);

        // Act
        var result = await contactService.DeleteContactAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetContactAsync_WhenCalled_ReturnsContact()
    {
        // Arrange
        var numberOfRandomContacts = 10;
        var randomContactInfos = ContactInfoFaker.GetFakes(numberOfRandomContacts, ContactInfoFaker.PropertiesNotNull);
        await AddContacts(randomContactInfos);

        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var contact = await contactService.AddContactAsync(contactInfo);

        // Act
        var result = await contactService.GetContactAsync(contact!.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(contact.Id));
        Assert.That(result!.FirstName, Is.EqualTo(contact.FirstName));
        Assert.That(result!.LastName, Is.EqualTo(contact.LastName));
        Assert.That(result!.Email, Is.EqualTo(contact.Email));
        Assert.That(result!.Phone, Is.EqualTo(contact.Phone));
    }

    [Test]
    public async Task GetContactsAsync_WhenCalled_ReturnsContacts()
    {
        // Arrange
        var numberOfRandomContacts = 10;
        var randomContactInfos = ContactInfoFaker.GetFakes(numberOfRandomContacts, ContactInfoFaker.PropertiesNotNull);
        await AddContacts(randomContactInfos);

        // Act
        var result = await contactService.GetContactsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(contactService.Contacts));
    }

    [Test]
    public async Task GetContactsAsync_WhenCacheNotYetInitialized_FetchesContactsFromRepository()
    {
        // Arrange
        var randomContacts = AutoFaker.Generate<Contact>(10);
        repository.GetContactsAsync().Returns(Task.FromResult<IEnumerable<Contact>>(randomContacts));

        // Act
        var result = await contactService.GetContactsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(randomContacts));
    }

    [Test]
    public async Task UpdateContactAsync_WhenCalled_ReturnsUpdatedContact()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var updateInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var originalContact = await contactService.AddContactAsync(contactInfo);

        // Act
        var result = await contactService.UpdateContactAsync(originalContact!.Id, updateInfo);

        // Assert
        Assert.That(result!.Id, Is.EqualTo(originalContact.Id));
        Assert.That(result!.FirstName, Is.EqualTo(updateInfo.FirstName));
        Assert.That(result!.LastName, Is.EqualTo(updateInfo.LastName));
        Assert.That(result!.Email, Is.EqualTo(updateInfo.Email));
        Assert.That(result!.Phone, Is.EqualTo(updateInfo.Phone));
    }

    [Test]
    public async Task UpdateContactAsync_WhenCalled_UpdatesContact()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var updateInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var originalContact = await contactService.AddContactAsync(contactInfo);

        // Act
        await contactService.UpdateContactAsync(originalContact!.Id, updateInfo);
        var result = contactService.Contacts;

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(originalContact.Id));
        Assert.That(result[0].FirstName, Is.EqualTo(updateInfo.FirstName));
        Assert.That(result[0].LastName, Is.EqualTo(updateInfo.LastName));
        Assert.That(result[0].Email, Is.EqualTo(updateInfo.Email));
        Assert.That(result[0].Phone, Is.EqualTo(updateInfo.Phone));

        // receives 2 calls because the first call is to add the contact to be updated
        await repository.Received(2).SaveContactsAsync(Arg.Any<List<Contact>>());
    }

    [Test]
    public async Task UpdateContactAsync_WhenCalled_DoesNotChangeTheOtherContacts()
    {
        // Arrange
        var numberOfRandomContacts = 10;
        var randomContactInfos = ContactInfoFaker.GetFakes(numberOfRandomContacts, ContactInfoFaker.PropertiesNotNull);
        await AddContacts(randomContactInfos);

        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var updateInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var originalContact = await contactService.AddContactAsync(contactInfo);

        // Act
        await contactService.UpdateContactAsync(originalContact!.Id, updateInfo);
        var result = contactService.Contacts;

        var firstNames = result.Where(c => c.Id != originalContact.Id).Select(c => c.FirstName);
        var lastNames = result.Where(c => c.Id != originalContact.Id).Select(c => c.LastName);
        var emails = result.Where(c => c.Id != originalContact.Id).Select(c => c.Email);
        var phones = result.Where(c => c.Id != originalContact.Id).Select(c => c.Phone);

        // Assert
        Assert.That(result, Has.Count.EqualTo(numberOfRandomContacts + 1));
        Assert.That(firstNames, Is.EquivalentTo(randomContactInfos.Select(c => c.FirstName)));
        Assert.That(lastNames, Is.EquivalentTo(randomContactInfos.Select(c => c.LastName)));
        Assert.That(emails, Is.EquivalentTo(randomContactInfos.Select(c => c.Email)));
        Assert.That(phones, Is.EquivalentTo(randomContactInfos.Select(c => c.Phone)));
    }

    [Test]
    public async Task UpdateContactAsync_WhenContactNotFound_ReturnsNull()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var updateInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);

        // Act
        var result = await contactService.UpdateContactAsync(Guid.NewGuid(), updateInfo);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindContact_WhenCalled_ReturnsMatchingContacts()
    {
        // Arrange
        var randomContactInfos = ContactInfoFaker.GetFakes(10, ContactInfoFaker.PropertiesNotNull);
        await AddContacts(randomContactInfos);

        var searchCriteria = randomContactInfos.First();

        // Act
        var result = await contactService.FindContactAsync(searchCriteria);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().FirstName, Is.EqualTo(searchCriteria.FirstName));
        Assert.That(result.First().LastName, Is.EqualTo(searchCriteria.LastName));
        Assert.That(result.First().Email, Is.EqualTo(searchCriteria.Email));
        Assert.That(result.First().Phone, Is.EqualTo(searchCriteria.Phone));
    }

    [Test]
    public async Task FindContact_WhenCalledWithNoMatchingCriteria_ReturnsEmptyEnumerable()
    {
        // Arrange
        var nonExistentFirstname = AutoFaker.Generate<string>();
        var randomContactInfos = ContactInfoFaker.GetFakes(10, ci =>
        {
            return ContactInfoFaker.PropertiesNotNull(ci) && ci.FirstName != nonExistentFirstname;
        });

        await AddContacts(randomContactInfos);

        var searchCriteria = new ContactInfo
        {
            FirstName = nonExistentFirstname
        };

        // Act
        var result = await contactService.FindContactAsync(searchCriteria);

        // Assert
        Assert.That(result, Is.Empty);
    }

    private async Task AddContacts(List<ContactInfo> randomContactInfos)
    {
        var tasks = randomContactInfos.Select(async c => await contactService.AddContactAsync(c));
        await Task.WhenAll(tasks);
    }
}
