using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.Core.Models;
using ContactKeeper.Core.Utilities;

namespace ContactKeeper.Core.Tests.Fakers;
internal class ContactFaker : Faker<Contact>
{
    public static Predicate<Contact> DifferentFrom(ContactInfo contactInfo) => c => {
        return c.FirstName != contactInfo.FirstName ||
                c.LastName != contactInfo.LastName ||
                c.Email != contactInfo.Email ||
                c.Phone != contactInfo.Phone;
    };
}
