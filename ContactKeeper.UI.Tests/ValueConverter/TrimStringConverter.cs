using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBogus;
using ContactKeeper.UI.ValueConverters;

using NUnit.Framework;

namespace ContactKeeper.UI.Tests.ValueConverter;
internal class TrimStringConverterTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private TrimStringConverter converter;
    private CultureInfo culture;
    private Type targetType;
    private string parameter;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void SetUp()
    {
        converter = new TrimStringConverter();
        targetType = AutoFaker.Generate<Type>();
        culture = CultureInfo.InvariantCulture;
        parameter = string.Empty;
    }

    [Test]
    public void Convert_WithNullValue_ReturnsNull()
    {        
        var result = converter.Convert(null!, targetType, parameter, culture);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Convert_WithNonStringValue_ReturnsValue()
    {
        var value = AutoFaker.Generate<int>();

        var result = converter.Convert(value, targetType, parameter, culture);

        Assert.That(result, Is.EqualTo(value));
    }


    [Test]
    [TestCase("", "")]
    [TestCase(" ", "")]
    [TestCase(" test", "test")]
    [TestCase("test ", "test")]
    [TestCase(" test ", "test")]
    [TestCase("test", "test")]
    public void Convert_WithStringValue_ReturnsValue(string value, string expected)
    {
        var result = converter.Convert(value, targetType, parameter, culture);

        Assert.That(result, Is.EqualTo(expected));
    }
}
