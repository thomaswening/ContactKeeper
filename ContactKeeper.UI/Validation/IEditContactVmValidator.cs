using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactKeeper.UI.Validation;

/// <summary>
/// ViewModel validator for the <see cref="EditContactVm"/> class.
/// </summary>
internal interface IEditContactVmValidator : IViewModelValidator
{
    /// <summary>
    /// Checks if the first name is not empty.
    /// </summary>
    /// <param name="firstName">The first name to validate.</param>
    /// <returns><see langword="true"/> if the first name is valid; otherwise, <see langword="false"/>.</returns>
    bool ValidateFirstName(string firstName);

    /// <summary>
    /// Checks if the email address has a valid format.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns><see langword="true"/> if the email address is valid; otherwise, <see langword="false"/>.</returns>
    bool ValidateEmail(string email);

    /// <summary>
    /// Checks if the phone number has a valid format.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <returns><see langword="true"/> if the phone number is valid; otherwise, <see langword="false"/>.</returns>
    bool ValidatePhone(string phone);
}
