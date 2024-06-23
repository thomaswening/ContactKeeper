using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using AutoBogus;

using ContactKeeper.Core.Models;
using ContactKeeper.Infrastructure.Repositories;

using NSubstitute;

using NUnit.Framework;

using Serilog;

namespace ContactKeeper.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class JsonContactRepositoryTests
    {
        private const string TestFileName = "testContacts.json";

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private IFileSystem fileSystem;
        private ILogger logger;
        private JsonContactRepository repository;

        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SetUp]
        public void SetUp()
        {
            fileSystem = new MockFileSystem();
            logger = Substitute.For<ILogger>();
            repository = new JsonContactRepository(TestFileName, fileSystem, logger);
        }

        [Test]
        public async Task GetContactsAsync_FileExists_ReturnsContacts()
        {
            // Arrange
            var fakeContacts = new AutoFaker<Contact>().Generate(3);
            var json = JsonSerializer.Serialize(fakeContacts);

            var mockFileData = new MockFileData(json);
            ((MockFileSystem)fileSystem).AddFile(TestFileName, mockFileData);

            // Act
            var contacts = await repository.GetContactsAsync();

            // Assert
            Assert.That(contacts, Is.Not.Null);
            Assert.That(contacts, Has.Count.EqualTo(3));
        }

        [Test]
        public async Task SaveContactsAsync_ValidContacts_SavesToFile()
        {
            // Arrange
            var fakeContacts = new AutoFaker<Contact>().Generate(2);

            // Act
            await repository.SaveContactsAsync(fakeContacts);

            // Assert
            var savedData = fileSystem.File.ReadAllText(TestFileName);
            var savedContacts = JsonSerializer.Deserialize<List<Contact>>(savedData);
            Assert.That(savedContacts, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetContactsAsync_EmptyFile_ReturnsEmptyList()
        {
            // Arrange
            var mockFileData = new MockFileData(string.Empty);
            ((MockFileSystem)fileSystem).AddFile(TestFileName, mockFileData);

            // Act
            var contacts = await repository.GetContactsAsync();

            // Assert
            Assert.That(contacts, Is.Not.Null);
            Assert.That(contacts, Is.Empty);
        }

        [Test]
        public async Task SaveContactsAsync_FileDoesNotExist_CreatesFileAndSavesContacts()
        {
            // Arrange
            var fakeContacts = new AutoFaker<Contact>().Generate(2);

            // Act
            await repository.SaveContactsAsync(fakeContacts);
            var savedData = fileSystem.File.ReadAllText(TestFileName);
            var savedContacts = JsonSerializer.Deserialize<List<Contact>>(savedData);

            // Assert
            Assert.That(fileSystem.File.Exists(TestFileName), Is.True);            
            Assert.That(savedContacts, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetContactsAsync_InvalidJson_ThrowsJsonException()
        {
            // Arrange
            var invalidJson = "invalid json";
            var mockFileData = new MockFileData(invalidJson);
            ((MockFileSystem)fileSystem).AddFile(TestFileName, mockFileData);

            // Act & Assert
            Assert.ThrowsAsync<JsonException>(async () => await repository.GetContactsAsync());
        }

        [Test]
        public void GetContactsAsync_FileLocked_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.File.Open(Arg.Any<string>(), Arg.Any<FileMode>(), Arg.Any<FileAccess>(), Arg.Any<FileShare>())
                .Returns(x => { throw new UnauthorizedAccessException(); });

            var repository = new JsonContactRepository(TestFileName, fileSystem, logger);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await repository.GetContactsAsync());
        }

        [Test]
        public void SaveContactsAsync_FileLocked_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var fileSystem = Substitute.For<IFileSystem>();
            var fakeContacts = new AutoFaker<Contact>().Generate(2);
            fileSystem.File.Open(Arg.Any<string>(), Arg.Any<FileMode>(), Arg.Any<FileAccess>(), Arg.Any<FileShare>())
                .Returns(x => { throw new UnauthorizedAccessException(); });

            // Create a new repository instance using the mocked IFileSystem
            var repositoryWithMockedFileSystem = new JsonContactRepository(TestFileName, fileSystem, logger);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await repositoryWithMockedFileSystem.SaveContactsAsync(fakeContacts));
        }
    }
}
