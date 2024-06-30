using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoBogus;

using ContactKeeper.UI.Validation;

using NUnit.Framework;

namespace ContactKeeper.UI.Tests.Validation;

[TestFixture]
internal class EditContactVmValidatorTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private EditContactVmValidator validator;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void SetUp()
    {
        validator = new EditContactVmValidator();
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ValidateFirstName_WithEmptyString_ReturnsFalseAndAddsError(string value)
    {
        var result = validator.ValidateFirstName(value);

        Assert.That(result, Is.False);
        Assert.That(validator.HasErrors, Is.True);
    }

    [Test]
    public void ValidateFirstName_WithValidString_ReturnsTrue()
    {
        var value = GenerateNonEmptyString();
        var result = validator.ValidateFirstName(value);

        Assert.That(result, Is.True);
    }

    [Test]
    public void ValidateEmail_WithEmptyString_ReturnsTrue()
    {
        var result = validator.ValidateEmail(string.Empty);

        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase("invalid-email")]
    [TestCase("invalid-email@")]
    [TestCase("@domain")]
    [TestCase(".")]
    public void ValidateEmail_WithInvalidEmail_ReturnsFalse(string value)
    {
        var result = validator.ValidateEmail(value);

        Assert.That(result, Is.False);
        Assert.That(validator.HasErrors, Is.True);
    }

    [Test]
    [TestCase("test@mail.com")]
    [TestCase("test.test@mail.de")]
    [TestCase("tst@mail.test.org")]
    public void ValidateEmail_WithValidEmail_ReturnsTrue(string value)
    {
        var result = validator.ValidateEmail(value);

        Assert.That(result, Is.True);
    }

    [Test]
    public void ValidatePhone_WithEmptyString_ReturnsTrue()
    {
        var result = validator.ValidatePhone(string.Empty);

        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase("invalid-phone")] // Contains alphabetic characters and a hyphen, which are not allowed.
    [TestCase(".1234567890")] // Starts with a dot, which is not allowed.
    [TestCase("@1234567890")] // Starts with an at symbol, which is not allowed.
    [TestCase("+12345 678 90")] // The country code is too long (more than 3 digits).
    [TestCase("0012 34")] // The local number part is too short (less than 3 digits).
    [TestCase("+ 123 456 7890")] // Contains a space after the plus sign, which is not allowed.
    [TestCase("+12 34 5678901290")] // The local number part is too long (more than 9 digits).
    public void ValidatePhone_WithInvalidPhone_ReturnsFalse(string value)
    {
        var result = validator.ValidatePhone(value);

        Assert.That(result, Is.False);
        Assert.That(validator.HasErrors, Is.True);
    }

    [Test]
    [TestCase("+1234567890")]
    [TestCase("001234567890")]
    [TestCase("1234567890")]
    [TestCase("123 456 7890")]
    [TestCase("123 4567890")]
    [TestCase("123456 7890")]
    public void ValidatePhone_WithValidPhone_ReturnsTrue(string value)
    {
        var result = validator.ValidatePhone(value);

        Assert.That(result, Is.True);
    }

    private static string GenerateNonEmptyString()
    {
        string value;
        do
        {
            value = AutoFaker.Generate<string>();
        } while (string.IsNullOrWhiteSpace(value));
        return value;
    }
}
