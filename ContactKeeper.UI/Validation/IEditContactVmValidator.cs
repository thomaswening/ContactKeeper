using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactKeeper.UI.Validation;

/// <summary>
/// ViewModel validator for the <see cref="EditContactVm"/> class.
/// </summary>
public interface IEditContactVmValidator : IViewModelValidator
{
    bool ValidateFirstName(string firstName);
    bool ValidateEmail(string email);
    bool ValidatePhone(string phone);
}
