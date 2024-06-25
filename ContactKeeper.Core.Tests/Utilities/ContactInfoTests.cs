using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoBogus;

using ContactKeeper.Core.Models;
using ContactKeeper.Core.Tests.Fakers;
using ContactKeeper.Core.Utilities;

using NUnit.Framework;

namespace ContactKeeper.Core.Tests.Utilities;

[TestFixture]
internal class ContactInfoTests
{
    [Test]
    public void ToContact_WithNonNullProperties_ReturnsContact()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);

        // Act
        var contact = contactInfo.ToContact();

        // Assert
        Assert.That(contact, Is.Not.Null);
        Assert.That(contact.FirstName, Is.EqualTo(contactInfo.FirstName));
        Assert.That(contact.LastName, Is.EqualTo(contactInfo.LastName));
        Assert.That(contact.Email, Is.EqualTo(contactInfo.Email));
        Assert.That(contact.Phone, Is.EqualTo(contactInfo.Phone));
    }  

    [Test]
    [TestCase(nameof(ContactInfo.FirstName))]
    [TestCase(nameof(ContactInfo.LastName))]
    [TestCase(nameof(ContactInfo.Email))]
    [TestCase(nameof(ContactInfo.Phone))]
    public void ToContact_WithNullProperty_ThrowsInvalidCastException(string propertyName)
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake();
        typeof(ContactInfo).GetProperty(propertyName)?.SetValue(contactInfo, null);

        // Act
        TestDelegate act = () => contactInfo.ToContact();

        // Assert
        Assert.That(act, Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void FromContact_WithNonNullContact_ReturnsContactInfo()
    {
        // Arrange
        var contact = AutoFaker.Generate<Contact>();

        // Act
        var contactInfo = ContactInfo.FromContact(contact);

        // Assert
        Assert.That(contactInfo, Is.Not.Null);
        Assert.That(contactInfo.FirstName, Is.EqualTo(contact.FirstName));
        Assert.That(contactInfo.LastName, Is.EqualTo(contact.LastName));
        Assert.That(contactInfo.Email, Is.EqualTo(contact.Email));
        Assert.That(contactInfo.Phone, Is.EqualTo(contact.Phone));
    }

    [Test]
    public void FromContact_WithNullContact_ThrowsArgumentNullException()
    {
        // Arrange
        Contact contact = null!;

        // Act
        TestDelegate act = () => ContactInfo.FromContact(contact);

        // Assert
        Assert.That(act, Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void IsMatch_WithMatchingContact_ReturnsTrue()
    {
        // Arrange
        var contact = AutoFaker.Generate<Contact>();
        var contactInfo = ContactInfo.FromContact(contact);

        // Act
        var isMatch = contactInfo.IsMatch(contact);

        // Assert
        Assert.That(isMatch, Is.True);
    }

    [Test]
    public void IsMatch_WithNonMatchingContact_ReturnsFalse()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var nonMatchingContact = ContactFaker.GetFake(ContactFaker.DifferentFrom(contactInfo));

        // Act
        var isMatch = contactInfo.IsMatch(nonMatchingContact);

        // Assert
        Assert.That(isMatch, Is.False);
    }

    [Test]
    public void IsMatch_WithNullContact_ThrowsArgumentNullException()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake();

        // Act
        TestDelegate act = () => contactInfo.IsMatch(null!);

        // Assert
        Assert.That(act, Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void OverwriteOnto_WithNonNullProperties_UpdatesContact()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake(ContactInfoFaker.PropertiesNotNull);
        var contact = ContactFaker.GetFake(ContactFaker.DifferentFrom(contactInfo));

        // Act
        contactInfo.OverwriteOnto(contact);

        // Assert
        Assert.That(contact.FirstName, Is.EqualTo(contactInfo.FirstName));
        Assert.That(contact.LastName, Is.EqualTo(contactInfo.LastName));
        Assert.That(contact.Email, Is.EqualTo(contactInfo.Email));
        Assert.That(contact.Phone, Is.EqualTo(contactInfo.Phone));
    }

    [Test]
    [TestCase(nameof(ContactInfo.FirstName))]
    [TestCase(nameof(ContactInfo.LastName))]
    [TestCase(nameof(ContactInfo.Email))]
    [TestCase(nameof(ContactInfo.Phone))]
    public void OverwriteOnto_WithNullProperty_DoesNotUpdateContact(string propertyName)
    {
        // Arrange
        var property = typeof(ContactInfo).GetProperty(propertyName)!;
        var contactInfo = ContactInfoFaker.GetFake();
        var contact = ContactFaker.GetFake();
        var contactPropertyValue = typeof(Contact).GetProperty(propertyName)?.GetValue(contact);

        property.SetValue(contactInfo, null);

        // Act
        contactInfo.OverwriteOnto(contact);
        var updatedPropertyValue = typeof(Contact).GetProperty(propertyName)?.GetValue(contact);

        // Assert
        Assert.That(updatedPropertyValue, Is.EqualTo(contactPropertyValue));
    }

    [Test]
    public void OverwriteOnto_WithNullContact_ThrowsArgumentNullException()
    {
        // Arrange
        var contactInfo = ContactInfoFaker.GetFake();

        // Act
        TestDelegate act = () => contactInfo.OverwriteOnto(null!);

        // Assert
        Assert.That(act, Throws.TypeOf<ArgumentNullException>());
    }
}
