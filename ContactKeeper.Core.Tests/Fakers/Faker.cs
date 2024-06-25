using AutoBogus;
using ContactKeeper.Core.Models;
using ContactKeeper.Core.Utilities;

namespace ContactKeeper.Core.Tests.Fakers;

/// <summary>
/// A class that generates fake objects for testing purposes.
/// </summary>
internal class Faker<T>
{
    /// <summary>
    /// Generates a fake <see cref="ContactInfo"/> object.
    /// </summary>
    /// <returns>A fake <see cref="ContactInfo"/> object. Optional.</returns>
    public static T GetFake(Predicate<T>? predicate = null)
    {
        T fake;
        do
        {
            fake = AutoFaker.Generate<T>();
        } while (predicate is not null && !predicate(fake));

        return fake;
    }

    /// <summary>
    /// Generates a list of fake <see cref="ContactInfo"/> objects.
    /// </summary>
    /// <param name="count">The number of fake objects to generate.</param>
    /// <returns>A list of fake <see cref="ContactInfo"/> objects.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the count is less than 0.</exception>
    public static List<T> GetFakes(int count, Predicate<T>? predicate = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0, nameof(count));

        var fakes = new List<T>();
        for (var i = 0; i < count; i++)
        {
            fakes.Add(GetFake(predicate));
        }

        return fakes;
    }
}