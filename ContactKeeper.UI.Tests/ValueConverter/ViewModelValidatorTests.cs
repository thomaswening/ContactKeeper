using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.UI.Validation;

using NUnit.Framework;

namespace ContactKeeper.UI.Tests.ValueConverter;

[TestFixture]
internal class ViewModelValidatorTests
{
    private class FakeViewModelValidator : ViewModelValidator
    {
        public void CallAddError(string propertyName, string error) => AddError(propertyName, error);
        public void CallClearErrors(string propertyName) => ClearErrors(propertyName);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private FakeViewModelValidator validator;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void SetUp()
    {
        validator = new FakeViewModelValidator();
    }

    [Test]
    public void HasErrors_WhenThereAreNoErrors_ReturnsFalse()
    {
        var result = validator.HasErrors;

        Assert.That(result, Is.False);
    }

    [Test]
    public void HasErrors_WhenThereAreErrors_ReturnsTrue()
    {
        validator.CallAddError("Property", "Error");
        var result = validator.HasErrors;

        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void IsValid_WithNullOrEmptyPropertyName_ReturnsTrue(string? propertyName)
    {
        var result = validator.IsValid(propertyName);
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_WhenThereAreNoErrorsToThatProperty_ReturnsFalse()
    {
        // Arrange
        validator.CallAddError("Property", "Error");

        // Act
        var result = validator.IsValid("SomeOtherProperty");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_WhenThereAreErrorsToThatProperty_ReturnsFalse()
    {
        // Arrange
        validator.CallAddError("Property", "Error");

        // Act
        var result = validator.IsValid("Property");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetErrors_WithNullOrEmptyPropertyName_ReturnsEmptyCollection()
    {
        var result = validator.GetErrors(null);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetErrors_WhenThereAreNoErrorsToThatProperty_ReturnsEmptyCollection()
    {
        // Arrange
        validator.CallAddError("Property", "Error");

        // Act
        var result = validator.GetErrors("SomeOtherProperty");

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetErrors_WhenThereAreErrorsToThatProperty_ReturnsCollectionWithErrors()
    {
        // Arrange
        string[] errors = { "Error1", "Error2", "Error3" };
        errors.ToList().ForEach(error => validator.CallAddError("Property", error));

        // Act
        var result = validator.GetErrors("Property");

        // Assert
        Assert.That(result, Is.EquivalentTo(errors));
    }

    [Test]
    public void Indexer_WhenThereAreNoErrorsToThatProperty_ReturnsEmptyString()
    {
        // Arrange
        validator.CallAddError("Property", "Error");

        // Act
        var result = validator["SomeOtherProperty"];

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Indexer_WhenThereAreErrorsToThatProperty_ReturnsFirstError()
    {
        // Arrange
        string[] errors = { "Error1", "Error2", "Error3" };
        errors.ToList().ForEach(error => validator.CallAddError("Property", error));

        // Act
        var result = validator["Property"];

        // Assert
        Assert.That(result, Is.EqualTo(errors.First()));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void AddError_WithNullOrEmptyPropertyName_ThrowsArgumentException(string? propertyName)
    {
        Assert.That(() => validator.CallAddError(propertyName!, "Error"), Throws.Exception);
    }

    [Test]
    public void AddError_WhenPropertyNameIsValid_AddsErrorAndInvokesErrorsChanged()
    {
        // Arrange
        string[] errors = { "Error1", "Error2", "Error3" };
        errors.ToList().ForEach(error => validator.CallAddError("Property", error));

        bool wasErrorsChangedInvoked = false;
        validator.ErrorsChanged += (_, _) => wasErrorsChangedInvoked = true;

        var newError = "NewError";
        var expectedErrors = errors.Append(newError);

        // Act
        validator.CallAddError("Property", newError);
        var result = validator.GetErrors("Property");

        // Assert
        Assert.That(result, Is.EquivalentTo(expectedErrors));
        Assert.That(wasErrorsChangedInvoked, Is.True);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ClearErrors_WhenPropertyNameNullOrWhitespace_DoesNotThrow(string? propertyName)
    {
        Assert.That(() => validator.CallClearErrors(propertyName!), Throws.Nothing);
    }

    [Test]
    [TestCase("Property")]
    public void ClearErrors_WhenThereAreNoErrorsToThatProperty_DoesNothing(string propertyName)
    {
        // Arrange       
        string[] errors = { "Error1", "Error2", "Error3" };
        errors.ToList().ForEach(error => validator.CallAddError("SomeOtherProperty", error));

        bool wasErrorsChangedInvoked = false;
        validator.ErrorsChanged += (_, _) => wasErrorsChangedInvoked = true;

        // Act
        validator.CallClearErrors(propertyName);
        var result = validator.GetErrors("SomeOtherProperty");

        // Assert
        Assert.That(result, Is.EquivalentTo(errors));
        Assert.That(wasErrorsChangedInvoked, Is.False);
    }

    [Test]
    public void ClearErrors_WhenThereAreErrorsToThatProperty_RemovesErrorsAndInvokesErrorsChanged()
    {
        // Arrange
        bool wasErrorsChangedInvoked = false;
        validator.ErrorsChanged += (_, _) => wasErrorsChangedInvoked = true;

        string[] errors = { "Error1", "Error2", "Error3" };
        errors.ToList().ForEach(error => validator.CallAddError("Property", error));

        // Act
        validator.CallClearErrors("Property");
        var result = validator.GetErrors("Property");

        // Assert
        Assert.That(result, Is.Empty);
        Assert.That(wasErrorsChangedInvoked, Is.True);
    }
}
