using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

using AutoBogus;

using ContactKeeper.Core.Utilities;
using ContactKeeper.UI.Utilities;
using ContactKeeper.UI.Validation;
using ContactKeeper.UI.ViewModels;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

namespace ContactKeeper.UI.Tests.ViewModels;

[TestFixture]
internal class EditContactVmTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private IEditContactVmValidator validator;
    private IEditContactManager contactManager;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void SetUp()
    {
        validator = Substitute.For<IEditContactVmValidator>();
        contactManager = Substitute.For<IEditContactManager>();
    }

    [Test]
    public void Constructor_WhenContactToEditIsNull_InitializesPropertiesAsEmpty()
    {
        // Act
        var viewModel = new EditContactVm(contactManager, validator);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel.FirstName, Is.Empty);
            Assert.That(viewModel.LastName, Is.Empty);
            Assert.That(viewModel.Email, Is.Empty);
            Assert.That(viewModel.Phone, Is.Empty);
        });
    }

    [Test]
    public void Constructor_WhenContactToEditIsNotNull_InitializesPropertiesFromContact()
    {
        // Arrange
        var contactToEdit = AutoFaker.Generate<ContactVm>();
        var viewModel = new EditContactVm(contactManager, validator, contactToEdit);

        // Act
        viewModel = new EditContactVm(contactManager, validator, contactToEdit);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel.FirstName, Is.EqualTo(contactToEdit.FirstName));
            Assert.That(viewModel.LastName, Is.EqualTo(contactToEdit.LastName));
            Assert.That(viewModel.Email, Is.EqualTo(contactToEdit.Email));
            Assert.That(viewModel.Phone, Is.EqualTo(contactToEdit.Phone));
        });
    }

    [TestCase(nameof(EditContactVm.FirstName))]
    [TestCase(nameof(EditContactVm.LastName))]
    [TestCase(nameof(EditContactVm.Email))]
    [TestCase(nameof(EditContactVm.Phone))]
    public void PropertyChanged_WhenPropertyChanges_ValidatesCorrespondingProperty(string propertyName)
    {
        // Arrange
        var viewModel = new EditContactVm(contactManager, validator);
        var newValue = AutoFaker.Generate<string>();

        // Act
        viewModel.GetType().GetProperty(propertyName)?.SetValue(viewModel, newValue);

        // Assert
        switch (propertyName)
        {
            case nameof(EditContactVm.FirstName):
                validator.Received().ValidateFirstName(newValue);
                break;

            case nameof(EditContactVm.LastName):
                validator.Received().ValidateFirstName(viewModel.FirstName);
                break;

            case nameof(EditContactVm.Email):
                validator.Received().ValidateEmail(newValue);
                validator.Received().ValidateFirstName(viewModel.FirstName);
                break;

            case nameof(EditContactVm.Phone):
                validator.Received().ValidatePhone(newValue);
                validator.Received().ValidateFirstName(viewModel.FirstName);
                break;
        }
    }

    [Test]
    public void SaveCommand_WhenHasErrors_DoesNotCallContactManager()
    {
        // Arrange
        validator.HasErrors.Returns(true);

        var viewModel = new EditContactVm(contactManager, validator);

        bool wasCloseRequested = false;
        bool wasConfirmContactOverwriteRequested = false;
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ConfirmContactOverwriteRequested += (_, _) => wasConfirmContactOverwriteRequested = true;

        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(contactManager.ReceivedCalls(), Is.Empty);
            Assert.That(wasCloseRequested, Is.False);
            Assert.That(wasConfirmContactOverwriteRequested, Is.False);
        });
    }

    [Test]
    public void SaveCommand_WhenHasUnsavedChangesIsFalse_ClosesDialogAndDoesNotCallContactManager()
    {
        // Arrange
        var viewModel = new EditContactVm(contactManager, validator);

        bool wasCloseRequested = false;
        bool wasConfirmContactOverwriteRequested = false;
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ConfirmContactOverwriteRequested += (_, _) => wasConfirmContactOverwriteRequested = true;

        validator.HasErrors.Returns(false);
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(false);

        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(contactManager.ReceivedCalls().Count, Is.EqualTo(1));
            Assert.That(wasCloseRequested, Is.True);
            Assert.That(wasConfirmContactOverwriteRequested, Is.False);
        });
    }

    [Test]
    public void SaveCommand_WhenContactManagerThrowsError_InvokesErrorOccuredAndDoesNotCloseDialog()
    {
        // Arrange
        var viewModel = new EditContactVm(contactManager, validator);
        
        bool wasCloseRequested = false;
        bool wasErrorOccuredInvoked = false;
        bool wasConfirmContactOverwriteRequested = false;

        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ErrorOccured += (_, _) => wasErrorOccuredInvoked = true;
        viewModel.ConfirmContactOverwriteRequested += (_, _) => wasConfirmContactOverwriteRequested = true;

        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>())
                      .Returns(true);

        contactManager.HandleDuplicateContact(Arg.Any<Guid>(), Arg.Any<ContactInfo>(), Arg.Any<ContactVm>())
                      .Throws(new Exception());

        contactManager.UpdateContactAsync(Arg.Any<Guid>(), Arg.Any<ContactInfo>()).Throws(new Exception());
        contactManager.AddContactAsync(Arg.Any<ContactInfo>()).Throws(new Exception());


        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(wasCloseRequested, Is.False);
            Assert.That(wasErrorOccuredInvoked, Is.True);
            Assert.That(wasConfirmContactOverwriteRequested, Is.False);
        });
    }

    [Test]
    public void SaveCommand_WhenContactToEditIsNullAndNoDuplicateFound_CallsAddContactAsyncAndRequestsClose()
    {
        // Arrange
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns((Guid?)null);

        var viewModel = new EditContactVm(contactManager, validator);

        bool wasCloseRequested = false;
        bool wasConfirmContactOverwriteRequested = false;
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ConfirmContactOverwriteRequested += (_, _) => wasConfirmContactOverwriteRequested = true;

        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        contactManager.Received().AddContactAsync(Arg.Any<ContactInfo>());

        Assert.Multiple(() =>
        {
            Assert.That(wasCloseRequested, Is.True);
            Assert.That(wasConfirmContactOverwriteRequested, Is.False);
        });
    }

    [Test]
    public void SaveCommand_WhenContactToEditIsNotNullAndNoDuplicateFound_CallsUpdateContactAsyncAndRequestsClose()
    {
        // Arrange
        var contactToEdit = AutoFaker.Generate<ContactVm>();
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns((Guid?)null);

        var viewModel = new EditContactVm(contactManager, validator, contactToEdit);

        bool wasCloseRequested = false;
        bool wasConfirmContactOverwriteRequested = false;
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ConfirmContactOverwriteRequested += (_, _) => wasConfirmContactOverwriteRequested = true;

        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        contactManager.Received().UpdateContactAsync(Arg.Any<Guid>(), Arg.Any<ContactInfo>());
        contactManager.DidNotReceive().HandleDuplicateContact(Arg.Any<Guid>(), Arg.Any<ContactInfo>(), Arg.Any<ContactVm>());
        contactManager.DidNotReceive().AddContactAsync(Arg.Any<ContactInfo>());

        Assert.Multiple(() =>
        {
            Assert.That(wasCloseRequested, Is.True);
            Assert.That(wasConfirmContactOverwriteRequested, Is.False);
        });
    }

    [Test]
    public void SaveCommand_WhenDuplicateFoundAndContactToEditIsNull_RequestsConfirmContactOverwrite()
    {
        // Arrange
        var duplicateId = Guid.NewGuid();
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(duplicateId);

        var viewModel = new EditContactVm(contactManager, validator);

        bool wasConfirmContactOverwriteRequested = false;
        viewModel.ConfirmContactOverwriteRequested += (_, _) => wasConfirmContactOverwriteRequested = true;

        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        contactManager.DidNotReceive().UpdateContactAsync(Arg.Any<Guid>(), Arg.Any<ContactInfo>());
        contactManager.DidNotReceive().AddContactAsync(Arg.Any<ContactInfo>());

        Assert.That(wasConfirmContactOverwriteRequested, Is.True);
    }

    [Test]
    public void SaveCommand_WhenDuplicateFoundWhichIsNotContactToEdit_RequestsConfirmContactOverwrite()
    {
        // Arrange
        var duplicateId = Guid.NewGuid();
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(duplicateId);

        var viewModel = new EditContactVm(contactManager, validator);

        bool wasConfirmContactOverwriteRequested = false;
        viewModel.ConfirmContactOverwriteRequested += (_, _) => wasConfirmContactOverwriteRequested = true;

        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        contactManager.DidNotReceive().UpdateContactAsync(Arg.Any<Guid>(), Arg.Any<ContactInfo>());
        contactManager.DidNotReceive().AddContactAsync(Arg.Any<ContactInfo>());

        Assert.That(wasConfirmContactOverwriteRequested, Is.True);
    }

    [Test]
    public void SaveCommand_WhenDuplicateFoundWhichIsContactToEdit_DoesNotRequestConfirmContactOverwriteAndUpdatesContact()
    {
        // Arrange
        var contactToEdit = AutoFaker.Generate<ContactVm>();
        var viewModel = new EditContactVm(contactManager, validator, contactToEdit);

        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(contactToEdit.Id);

        bool wasConfirmContactOverwriteRequested = false;
        viewModel.ConfirmContactOverwriteRequested += (_, _) => wasConfirmContactOverwriteRequested = true;

        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        contactManager.Received().UpdateContactAsync(Arg.Any<Guid>(), Arg.Any<ContactInfo>());
        contactManager.DidNotReceive().AddContactAsync(Arg.Any<ContactInfo>());

        Assert.That(wasConfirmContactOverwriteRequested, Is.False);
    }

    [Test]
    public void SaveCommand_WhenDuplicateFoundAndContactToEditIsNullAndUserConfirmsOverwrite_CallsHandleDuplicateContactAndRequestsClose()
    {
        // Arrange
        var duplicateId = Guid.NewGuid();
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(duplicateId);

        var viewModel = new EditContactVm(contactManager, validator);

        bool wasCloseRequested = false;
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ConfirmContactOverwriteRequested += (_, e) => e.SetResult(true);

        // Act
        viewModel.SaveCommand.Execute(true);

        // Assert
        contactManager.Received().HandleDuplicateContact(duplicateId, Arg.Any<ContactInfo>(), Arg.Any<ContactVm>());
        Assert.That(wasCloseRequested, Is.True);
    }

    [Test]
    public void SaveCommand_WhenDuplicateFoundAndContactToEditIsNotNullAndUserConfirmsOverwrite_CallsHandleDuplicateContactAndRequestsClose()
    {
        // Arrange
        var duplicateId = Guid.NewGuid();
        var contactToEdit = AutoFaker.Generate<ContactVm>();
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(duplicateId);

        var viewModel = new EditContactVm(contactManager, validator, contactToEdit);

        bool wasCloseRequested = false;
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ConfirmContactOverwriteRequested += (_, e) => e.SetResult(true);

        // Act
        viewModel.SaveCommand.Execute(true);

        // Assert
        contactManager.Received().HandleDuplicateContact(duplicateId, Arg.Any<ContactInfo>(), Arg.Any<ContactVm>());
        Assert.That(wasCloseRequested, Is.True);
    }

    [Test]
    public void SaveCommand_WhenDuplicateFoundAndContactToEditIsNullAndUserDoesNotConfirmOverwrite_DoesNotCallHandleDuplicateContactAndDoesNotClose()
    {
        // Arrange
        var duplicateId = Guid.NewGuid();
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(duplicateId);

        var viewModel = new EditContactVm(contactManager, validator);

        bool wasCloseRequested = false;
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ConfirmContactOverwriteRequested += (_, e) => e.SetResult(false);

        // Act
        viewModel.SaveCommand.Execute(true);

        // Assert
        contactManager.DidNotReceive().HandleDuplicateContact(Arg.Any<Guid>(), Arg.Any<ContactInfo>(), Arg.Any<ContactVm>());
        Assert.That(wasCloseRequested, Is.False);
    }

    [Test]
    public void SaveCommand_WhenDuplicateFoundAndContactToEditIsNotNullAndUserDoesNotConfirmOverwrite_DoesNotCallHandleDuplicateContactAndDoesNotClose()
    {
        // Arrange
        var duplicateId = Guid.NewGuid();
        var contactToEdit = AutoFaker.Generate<ContactVm>();

        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);
        contactManager.FindFullNameDuplicateAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(duplicateId);

        var viewModel = new EditContactVm(contactManager, validator, contactToEdit);

        bool wasCloseRequested = false;
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;
        viewModel.ConfirmContactOverwriteRequested += (_, e) => e.SetResult(false);

        // Act
        viewModel.SaveCommand.Execute(true);

        // Assert
        contactManager.DidNotReceive().HandleDuplicateContact(Arg.Any<Guid>(), Arg.Any<ContactInfo>(), Arg.Any<ContactVm>());
        Assert.That(wasCloseRequested, Is.False);
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public void SaveCommand_CanExecuteReturnsExpectedValueBasedOnErrors(bool hasErrors, bool expectedResult)
    {
        // Arrange
        validator.HasErrors.Returns(hasErrors);

        var viewModel = new EditContactVm(contactManager, validator);

        // Act
        var canSave = viewModel.SaveCommand.CanExecute(null);

        // Assert
        Assert.That(canSave, Is.EqualTo(expectedResult));
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void CancelCommand_WhenHasUnsavedChanges_RequestsConfirmCloseWithUnsavedChanges(bool hasUnsavedChanges, bool expectedResult)
    {
        // Arrange
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(hasUnsavedChanges);

        var viewModel = new EditContactVm(contactManager, validator);

        bool wasConfirmCloseWithUnsavedChangesRequested = false;
        viewModel.ConfirmCloseWithUnsavedChangesRequested += (_, _) => wasConfirmCloseWithUnsavedChangesRequested = true;

        // Act
        viewModel.CancelCommand.Execute(null);

        // Assert
        Assert.That(wasConfirmCloseWithUnsavedChangesRequested, Is.EqualTo(expectedResult));
    }


    [TestCase(true, true)]
    [TestCase(false, false)]
    public void CancelCommand_WhenHasUnsavedChanges_RequestsCloseOnlyIfUserConfirmsCloseWithUnsavedChanges(bool isUserConfirmedClose, bool expectedResult)
    {
        // Arrange
        contactManager.CheckForUnsavedChanges(Arg.Any<ContactInfo>(), Arg.Any<ContactVm>()).Returns(true);

        var viewModel = new EditContactVm(contactManager, validator);

        bool wasCloseRequested = false;
        viewModel.ConfirmCloseWithUnsavedChangesRequested += (_, e) => e.SetResult(isUserConfirmedClose);
        viewModel.CloseRequested += (_, _) => wasCloseRequested = true;

        // Act
        viewModel.CancelCommand.Execute(null);

        // Assert
        Assert.That(wasCloseRequested, Is.EqualTo(expectedResult));
    }
}
