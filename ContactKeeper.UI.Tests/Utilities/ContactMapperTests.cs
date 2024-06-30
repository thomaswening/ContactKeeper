using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoBogus;

using ContactKeeper.Core.Models;
using ContactKeeper.UI.Utilities;
using ContactKeeper.UI.ViewModels;

using NUnit.Framework;

namespace ContactKeeper.UI.Tests.ValueConverter;

[TestFixture]
internal class ContactMapperTests
{
    [Test]
    public void Map_WhenNullContactArgument_ThrowsArgumentNullException()
    {
        Contact contact = null!;

        Assert.That(() => ContactMapper.Map(contact), Throws.ArgumentNullException);
    }

    [Test]
    public void Map_WhenValidContactArgument_ReturnsContactVmObject()
    {
        // Arrange
        var contact = AutoFaker.Generate<Contact>();

        // Act
        var result = ContactMapper.Map(contact);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<ContactVm>());
            Assert.That(result.Id, Is.EqualTo(contact.Id));
            Assert.That(result.FirstName, Is.EqualTo(contact.FirstName));
            Assert.That(result.LastName, Is.EqualTo(contact.LastName));
            Assert.That(result.Phone, Is.EqualTo(contact.Phone));
            Assert.That(result.Email, Is.EqualTo(contact.Email));
        });
    }

    [Test]
    public void Map_WhenNullContactVmArgument_ThrowsArgumentNullException()
    {
        ContactVm contactVm = null!;

        Assert.That(() => ContactMapper.Map(contactVm), Throws.ArgumentNullException);
    }

    [Test]
    public void Map_WhenValidContactVmArgument_ReturnsContactObject()
    {
        // Arrange
        var contactVm = AutoFaker.Generate<ContactVm>();

        // Act
        var result = ContactMapper.Map(contactVm);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<Contact>());
            Assert.That(result.Id, Is.EqualTo(contactVm.Id));
            Assert.That(result.FirstName, Is.EqualTo(contactVm.FirstName));
            Assert.That(result.LastName, Is.EqualTo(contactVm.LastName));
            Assert.That(result.Phone, Is.EqualTo(contactVm.Phone));
            Assert.That(result.Email, Is.EqualTo(contactVm.Email));
        });
    }

    [Test]
    public void Map_WhenNullContactCollectionArgument_ThrowsArgumentNullException()
    {
        IEnumerable<Contact> contacts = null!;

        Assert.That(() => ContactMapper.Map(contacts), Throws.ArgumentNullException);
    }

    [Test]
    public void Map_WhenValidContactCollectionArgument_ReturnsContactVmCollection()
    {
        // Arrange
        var contacts = AutoFaker.Generate<Contact>(3);

        // Act
        var result = ContactMapper.Map(contacts);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<ContactVm>>());
            Assert.That(result.Count(), Is.EqualTo(contacts.Count()));
        });
    }

    [Test]
    public void Map_WhenNullContactVmCollectionArgument_ThrowsArgumentNullException()
    {
        IEnumerable<ContactVm> contactVms = null!;

        Assert.That(() => ContactMapper.Map(contactVms), Throws.ArgumentNullException);
    }

    [Test]
    public void Map_WhenValidContactVmCollectionArgument_ReturnsContactCollection()
    {
        // Arrange
        var contactVms = AutoFaker.Generate<ContactVm>(3);

        // Act
        var result = ContactMapper.Map(contactVms);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<Contact>>());
            Assert.That(result.Count(), Is.EqualTo(contactVms.Count));
        });
    }
}
