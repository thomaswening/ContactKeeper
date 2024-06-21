using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactKeeper.UI.Validation;

/// <summary>
/// Represents a validator for a view model that offers an indexer to get the validation error message for a property
/// which can be directly bound to controls in the view. It also implements <see cref="INotifyDataErrorInfo"/> to
/// support asynchronous validation.
/// </summary>
public interface IViewModelValidator : INotifyDataErrorInfo
{
    /// <summary>
    /// Gets the first error message for the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property to get the error message for.</param>
    /// <returns>The error message for the specified property name.</returns>
    string this[string propertyName] { get; }
}