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

    /// <summary>
    /// Checks if the first name is not empty.
    /// </summary>
    /// <param name="firstName">The first name to validate.</param>
    /// <returns><see langword="true"/> if the first name is valid; otherwise, <see langword="false"/>.</returns>
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

    /// <summary>
    /// Checks if the email address has a valid format.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns><see langword="true"/> if the email address is valid; otherwise, <see langword="false"/>.</returns>
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

    /// <summary>
    /// Checks if the phone number has a valid format.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <returns><see langword="true"/> if the phone number is valid; otherwise, <see langword="false"/>.</returns>
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
