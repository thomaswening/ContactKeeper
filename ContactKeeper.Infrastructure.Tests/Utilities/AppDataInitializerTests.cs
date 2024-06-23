using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactKeeper.Infrastructure.Utilities;

using NSubstitute;

using NUnit.Framework;

using Serilog;

namespace ContactKeeper.Infrastructure.Tests.Utilities;

[TestFixture]
public class AppDataInitializerTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ILogger logger;
    private IFileSystem fileSystem;
    private AppDataInitializer appDataInitializer;
    private string baseDirectory;
    private string defaultFileName;

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void SetUp()
    {
        logger = Substitute.For<ILogger>();
        fileSystem = Substitute.For<IFileSystem>();
        appDataInitializer = new AppDataInitializer(fileSystem, logger);

        baseDirectory = @"C:\TestData";
        defaultFileName = "contacts.json";
        fileSystem.Path.Combine(baseDirectory, defaultFileName).Returns($"{baseDirectory}\\{defaultFileName}");
    }

    [Test]
    public void Initialize_WhenCalled_ReturnsExpectedPath()
    {
        // Arrange
        var expectedPath = $"{baseDirectory}\\{defaultFileName}";
        fileSystem.Directory.Exists(baseDirectory).Returns(true);

        // Act
        var result = appDataInitializer.Initialize(baseDirectory, defaultFileName);

        // Assert
        Assert.That(expectedPath, Is.EqualTo(result));
    }

    [Test]
    public void Initialize_BaseDirectoryIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => appDataInitializer.Initialize(null!, defaultFileName));
    }

    [Test]
    public void Initialize_BaseDirectoryIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => appDataInitializer.Initialize(string.Empty, defaultFileName));
    }

    [Test]
    public void Initialize_BaseDirectoryIsWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => appDataInitializer.Initialize(" ", defaultFileName));
    }

    [Test]
    public void Initialize_DirectoryDoesNotExist_CreatesDirectory()
    {
        // Arrange
        fileSystem.Directory.Exists(baseDirectory).Returns(false);

        // Act
        appDataInitializer.Initialize(baseDirectory, defaultFileName);

        // Assert
        fileSystem.Directory.Received(1).CreateDirectory(baseDirectory);
    }
}
