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

[TestFixture]
internal class IsStringEmptyConverterTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private IsStringEmptyConverter converter;
    private CultureInfo culture;
    private Type targetType;
    private string parameter;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void SetUp()
    {
        converter = new IsStringEmptyConverter();
        targetType = AutoFaker.Generate<Type>();
        culture = CultureInfo.InvariantCulture;
        parameter = string.Empty;
    }

    [Test]
    public void Convert_WithNullValue_ReturnsUnsetValue()
    {        
        var result = converter.Convert(null!, targetType, parameter, culture);

        Assert.That(result, Is.EqualTo(System.Windows.DependencyProperty.UnsetValue));
    }

    [Test]
    public void Convert_WithNonStringValue_ReturnsUnsetValue()
    {
        var value = AutoFaker.Generate<int>();

        var result = converter.Convert(value, targetType, parameter, culture);

        Assert.That(result, Is.EqualTo(System.Windows.DependencyProperty.UnsetValue));
    }

    [Test]
    public void Convert_WithEmptyString_ReturnsTrue()
    {
        var value = string.Empty;

        var result = converter.Convert(value, targetType, parameter, culture);

        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(" ")]
    [TestCase("a")]
    public void Convert_WithNonEmptyString_ReturnsTrue(string value)
    {
        var result = converter.Convert(value, targetType, parameter, culture);

        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase("", false)]
    [TestCase(" ", true)]
    [TestCase("a", true)]
    public void Convert_WithInvertParameter_ReturnsInvertedResult(string value, bool expectedResult)
    {
        var parameter = "invert";

        var result = converter.Convert(value, targetType, parameter, culture);

        Assert.That(result, Is.EqualTo(expectedResult));
    }
}
