using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ContactKeeper.Core.Models;
[method: JsonConstructor]
public class Contact(Guid id, string firstName, string lastName, string phone, string email)
{
    public Guid Id { get; } = id;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public string Email { get; set; } = email;
    public string Phone { get; set; } = phone;

    public Contact(string firstName, string lastName, string phone, string email) : this(Guid.NewGuid(), firstName, lastName, phone, email)
    {
    }
}
