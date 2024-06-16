using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactKeeper.UI.ViewModels;
/// <summary>
/// Represents a contact in the application and exposes properties for observation in the UI.
/// </summary>
public partial class ContactVm : ObservableObject
{
    public Guid Id { get; }

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string phone = string.Empty;

    public ContactVm(Guid id, string firstName, string lastName, string phone, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
    }
}