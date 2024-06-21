using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ContactKeeper.UI.Validation;

namespace ContactKeeper.UI.ViewModels;

/// <summary>
/// Represents a view model that supports validation using a <see cref="IViewModelValidator"/> instance.
/// </summary>
public partial class ValidatedViewModel : ObservableObject, INotifyDataErrorInfo
{
    /// <inheritdoc/>
    public bool HasErrors => Validator.HasErrors;

    /// <inheritdoc/>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Gets the validator that provides validation for this view model.
    /// </summary>
    public IViewModelValidator Validator { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatedViewModel"/> class.
    /// </summary>
    /// <param name="validator">The validator to use for this view model.</param>
    /// <exception cref="ArgumentNullException"><paramref name="validator"/> is <see langword="null"/>.</exception>
    public ValidatedViewModel(IViewModelValidator validator)
    {
        Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        Validator.ErrorsChanged += (s, e) =>
        {
            ErrorsChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(Validator));
        };
    }

    /// <inheritdoc/>
    public IEnumerable GetErrors(string? propertyName) => Validator.GetErrors(propertyName);
}