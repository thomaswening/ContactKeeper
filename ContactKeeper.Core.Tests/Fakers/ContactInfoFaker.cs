using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.Core.Utilities;

namespace ContactKeeper.Core.Tests.Fakers;
internal class ContactInfoFaker : Faker<ContactInfo>
{
    public static Predicate<ContactInfo> PropertiesNotNull => contactInfo => {
        return contactInfo.FirstName is not null &&
                contactInfo.LastName is not null &&
                contactInfo.Email is not null &&
                contactInfo.Phone is not null;
    };
}
