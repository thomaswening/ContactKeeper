using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoBogus;

using ContactKeeper.Core.Interfaces;
using ContactKeeper.Core.Models;
using ContactKeeper.Core.Utilities;
using ContactKeeper.UI.Utilities;
using ContactKeeper.UI.ViewModels;

using NSubstitute;

using NUnit.Framework;

using Serilog;

namespace ContactKeeper.UI.Tests.Utilities;

[TestFixture]
internal class EditContactManagerTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private IContactService contactService;
    private ILogger logger;
    private EditContactManager contactManager;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void Setup()
    {
        contactService = Substitute.For<IContactService>();
        logger = Substitute.For<ILogger>();
        contactManager = new EditContactManager(contactService, logger);
    }

    [Test]
    public void HandleDuplicateContact_WhenContactInfoIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ContactInfo contactInfo = null!;
        var contactToUpdate = AutoFaker.Generate<ContactVm>();

        // Act & Assert
        Assert.That(async () => await contactManager.HandleDuplicateContact(Guid.NewGuid(), contactInfo, contactToUpdate),
                    Throws.ArgumentNullException);
    }

    [Test]
    public async Task HandleDuplicateContact_WhenContactToUpdateIsNull_DoesNotDeleteContact()
    {
        // Arrange
        var contactInfo = AutoFaker.Generate<ContactInfo>();
        ContactVm? contactToUpdate = null;

        // Act
        await contactManager.HandleDuplicateContact(Guid.NewGuid(), contactInfo, contactToUpdate);

        // Assert
        await contactService.DidNotReceive().DeleteContactAsync(Arg.Any<Guid>());
    }

    [Test]
    public async Task HandleDuplicateContact_WhenContactToUpdateIsNotNull_DeletesContact()
    {
        // Arrange
        var contactInfo = AutoFaker.Generate<ContactInfo>();
        var contactToUpdate = AutoFaker.Generate<ContactVm>();

        // Act
        await contactManager.HandleDuplicateContact(Guid.NewGuid(), contactInfo, contactToUpdate);

        // Assert
        await contactService.Received().DeleteContactAsync(contactToUpdate.Id);
    }

    [Test]
    public async Task HandleDuplicateContact_Always_UpdatesContactWithDuplicateId()
    {
        // Arrange
        var duplicateId = Guid.NewGuid();
        var contactInfo = AutoFaker.Generate<ContactInfo>();
        var contactToUpdate = AutoFaker.Generate<ContactVm>();

        // Act
        await contactManager.HandleDuplicateContact(duplicateId, contactInfo, contactToUpdate);

        // Assert
        await contactService.Received().UpdateContactAsync(duplicateId, contactInfo);
    }

    [Test]
    [TestCase(null, "Doe")]
    [TestCase("John", null)]
    [TestCase(null, null)]
    public void FindFullNameDuplicateAsync_WhenFirstOrLastNameIsNull_ThrowsArgumentNullException(string? firstName, string? lastName)
    {
        // Act & Assert
        Assert.That(async () => await contactManager.FindFullNameDuplicateAsync(firstName!, lastName!),
                    Throws.ArgumentNullException);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public async Task FindFullNameDuplicateAsync_WhenFirstNameIsNullOrWhiteSpace_ReturnsNull(string firstName)
    {
        // Act
        var lastName = AutoFaker.Generate<string>();
        var result = await contactManager.FindFullNameDuplicateAsync(string.Empty, lastName);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindFullNameDuplicateAsync_WhenNoDuplicateFound_ReturnsNull()
    {
        // Arrange
        var firstName = AutoFaker.Generate<string>();
        var lastName = AutoFaker.Generate<string>();
        var queryInfo = new ContactInfo()
        {
            FirstName = firstName,
            LastName = lastName
        };

        contactService.FindContactAsync(queryInfo).Returns([]);

        // Act
        var result = await contactManager.FindFullNameDuplicateAsync(firstName, lastName);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindFullNameDuplicateAsync_WhenOneDuplicateFound_ReturnsDuplicateId()
    {
        // Arrange
        var firstName = AutoFaker.Generate<string>();
        var lastName = AutoFaker.Generate<string>();

        var duplicateContact = new Contact(firstName, lastName, string.Empty, string.Empty);

        contactService.FindContactAsync(Arg.Any<ContactInfo>()).Returns([duplicateContact]);

        // Act
        var result = await contactManager.FindFullNameDuplicateAsync(firstName, lastName);

        // Assert
        Assert.That(result, Is.EqualTo(duplicateContact.Id));
    }

    [Test]
    public void FindFullNameDuplicateAsync_WhenMultipleDuplicatesFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var firstName = AutoFaker.Generate<string>();
        var lastName = AutoFaker.Generate<string>();

        var duplicateContacts = new List<Contact>()
        {
            new(firstName, lastName, string.Empty, string.Empty),
            new(firstName, lastName, string.Empty, string.Empty)
        };

        contactService.FindContactAsync(Arg.Any<ContactInfo>()).Returns(duplicateContacts);

        // Act & Assert
        Assert.That(async () => await contactManager.FindFullNameDuplicateAsync(firstName, lastName),
                    Throws.InvalidOperationException);
    }

    [Test]
    public async Task UpdateContactAsync_Always_UpdatesContactWithGivenId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var contactInfo = AutoFaker.Generate<ContactInfo>();

        // Act
        await contactManager.UpdateContactAsync(id, contactInfo);

        // Assert
        await contactService.Received().UpdateContactAsync(id, contactInfo);
    }

    [Test]
    public async Task AddContactAsync_Always_AddsContactWithGivenContactInfo()
    {
        // Arrange
        var contactInfo = AutoFaker.Generate<ContactInfo>();

        // Act
        await contactManager.AddContactAsync(contactInfo);

        // Assert
        await contactService.Received().AddContactAsync(contactInfo);
    }

    [Test]
    public void CheckForUnsavedChanges_WhenContactInfoIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ContactInfo contactInfo = null!;
        ContactVm contactToCompare = AutoFaker.Generate<ContactVm>();

        // Act & Assert
        Assert.That(() => contactManager.CheckForUnsavedChanges(contactInfo, contactToCompare), Throws.ArgumentNullException);
    }

    [Test]
    public void CheckForUnsavedChanges_WhenContactToCompareIsNull_ReturnsTrue()
    {
        // Arrange
        var contactInfo = AutoFaker.Generate<ContactInfo>();
        ContactVm? contactToCompare = null;

        // Act
        var result = contactManager.CheckForUnsavedChanges(contactInfo, contactToCompare);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(nameof(ContactInfo.FirstName))]
    [TestCase(nameof(ContactInfo.LastName))]
    [TestCase(nameof(ContactInfo.Phone))]
    [TestCase(nameof(ContactInfo.Email))]
    public void CheckForUnsavedChanges_WhenContactInfoDoesNotMatchContactToCompare_ReturnsTrue(string propertyName)
    {
        // Arrange
        var contactToCompare = AutoFaker.Generate<ContactVm>();
        var contactInfo = new ContactInfo()
        {
            FirstName = contactToCompare.FirstName,
            LastName = contactToCompare.LastName,
            Phone = contactToCompare.Phone,
            Email = contactToCompare.Email,
        };

        switch (propertyName)
        {
            case nameof(ContactInfo.FirstName):
                contactInfo.FirstName = GetRandomStringUnequalTo(contactToCompare.FirstName);
                break;

            case nameof(ContactInfo.LastName):
                contactInfo.LastName = GetRandomStringUnequalTo(contactToCompare.LastName);
                break;

            case nameof(ContactInfo.Phone):
                contactInfo.Phone = GetRandomStringUnequalTo(contactToCompare.Phone);
                break;

            case nameof(ContactInfo.Email):
                contactInfo.Email = GetRandomStringUnequalTo(contactToCompare.Email);
                break;
        }

        // Act
        var result = contactManager.CheckForUnsavedChanges(contactInfo, contactToCompare);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckForUnsavedChanges_WhenContactInfoMatchesContactToCompare_ReturnsFalse()
    {
        // Arrange
        var contactToCompare = AutoFaker.Generate<ContactVm>();
        var contactInfo = new ContactInfo()
        {
            FirstName = contactToCompare.FirstName,
            LastName = contactToCompare.LastName,
            Phone = contactToCompare.Phone,
            Email = contactToCompare.Email,
        };

        // Act
        var result = contactManager.CheckForUnsavedChanges(contactInfo, contactToCompare);

        // Assert
        Assert.That(result, Is.False);
    }

    private static string GetRandomStringUnequalTo(string value)
    {
        string randomString;
        do
        {
            randomString = AutoFaker.Generate<string>();
        } while (randomString == value);

        return randomString;
    }
}
