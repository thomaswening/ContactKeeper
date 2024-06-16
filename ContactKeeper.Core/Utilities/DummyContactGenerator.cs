using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.Core.Models;

namespace ContactKeeper.Core.Utilities;

/// <summary>
/// Generates dummy contacts for testing purposes.
/// </summary>
internal static class DummyContactGenerator
{
    private static readonly Random random = new();
    private static readonly string[] firstNames =
    [
        "John", "Jane", "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Hank", "Ivy", "Jack", "Kate",
        "Liam", "Mia", "Noah", "Olivia", "Peter", "Quinn", "Ryan", "Sara", "Tom", "Uma", "Vince", "Wendy", "Xander",
        "Yara", "Zane"
    ];

    private static readonly string[] lastNames =
    [
        "Doe", "Smith", "Johnson", "Brown", "Lee", "White", "Black", "Green", "Jones", "King", "Adams", "Baker", "Clark",
        "Davis", "Evans", "Fisher", "Garcia", "Harris", "Irwin", "Jones", "Klein", "Lopez", "Morgan", "Nelson", "Owens",
        "Perez", "Quinn", "Reed", "Smith", "Taylor", "Upton", "Vargas", "White", "Xu", "Young", "Zhang"
    ];

    private static readonly string[] domains = ["gmail.com", "yahoo.com", "outlook.com", "hotmail.com", "protonmail.com"];
    private static readonly string[] cellPrefixes = ["0151", "0157", "0160", "0170", "0171", "0172", "0173", "0174", "0175", "0176", "0177", "0178", "0179"];

    public static List<Contact> GenerateContacts(int numberOfContacts)
    {
        List<Contact> contacts = [];

        for (int i = 0; i < numberOfContacts; i++)
        {
            string firstname = firstNames[random.Next(firstNames.Length)];
            string lastname = lastNames[random.Next(lastNames.Length)];
            string email = GenerateRandomEmail(firstname, lastname);
            string phone = GenerateRandomPhoneNumber();

            var contact = new Contact(Guid.NewGuid(), firstname, lastname, email, phone);
            if (contacts.Any(c => c.FirstName == contact.FirstName && c.LastName == contact.LastName))
            {
                i--;
                continue;
            }

            contacts.Add(contact);
        }

        return contacts;
    }

    private static string GenerateRandomPhoneNumber()
    {
        string prefix = cellPrefixes[random.Next(cellPrefixes.Length)];
        int numberLength = 7;

        var subscriberNumberSb = new StringBuilder();
        for (int i = 0; i < numberLength; i++)
        {
            subscriberNumberSb.Append(random.Next(0, 10));
        }

        return $"{prefix} {subscriberNumberSb}";
    }

    private static string GenerateRandomEmail(string firstname, string lastname)
    {
        string domain = domains[random.Next(domains.Length)];
        return $"{firstname}.{lastname}@{domain}";
    }
}
