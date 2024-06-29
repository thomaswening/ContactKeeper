using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContactKeeper.UI.ViewModels;

namespace ContactKeeper.UI.Validation;

/// <summary>
/// ViewModel validator for the <see cref="EditContactVm"/> class.
/// </summary>
internal class EditContactVmValidator : ViewModelValidator, IEditContactVmValidator
{
    private const string FirstName = nameof(EditContactVm.FirstName);
    private const string Email = nameof(EditContactVm.Email);
    private const string Phone = nameof(EditContactVm.Phone);

    /// <inheritdoc />
    public bool ValidateFirstName(string firstName)
    {
        ClearErrors(FirstName);
        if (string.IsNullOrWhiteSpace(firstName))
        {
            AddError(FirstName, "First name is required.");
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool ValidateEmail(string email)
    {
        ClearErrors(Email);
        if (string.IsNullOrWhiteSpace(email))
            return true;

        try
        {
            var _ = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            AddError(Email, "Invalid email address.");
            return false;
        }
    }

    /// <inheritdoc />
    public bool ValidatePhone(string phoneNumber)
    {
        ClearErrors(Phone);
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return true;

        string pattern = @"^(?:\+|00)?(?:\d{2,3})?\s?\d{2,4}\s?\d{3,9}$";
        if (!Regex.IsMatch(phoneNumber, pattern))
        {
            AddError(Phone, "Invalid phone number.");
            return false;
        }

        return true;
    }
}
