    using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactKeeper.UI.Validation;

/// <summary>
/// Represents a validator for a view model that offers an indexer to get the validation error message for a property
/// which can be directly bound to controls in the view. It also implements <see cref="INotifyDataErrorInfo"/> to
/// support asynchronous validation.
/// </summary>
public class ViewModelValidator : ObservableObject, IViewModelValidator
{
    private readonly Dictionary<string, List<string>> errorsToProperty = [];

    /// <inheritdoc/>
    public bool HasErrors => errorsToProperty.Count > 0;

    /// <inheritdoc/>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Determines whether the specified property has any validation errors.
    /// </summary>
    /// <param name="propertyName">The name of the property to check for validation errors.</param>
    /// <returns><see langword="true"/> if the property has validation errors; otherwise, <see langword="false"/>.</returns>
    public bool IsValid(string? propertyName)
    {
        return string.IsNullOrWhiteSpace(propertyName)
            || !errorsToProperty.TryGetValue(propertyName, out var _);
    }

    /// <inheritdoc/>
    public IEnumerable GetErrors(string? propertyName)
    {
        errorsToProperty.TryGetValue(propertyName ?? string.Empty, out var errors);
        return errors ?? [];
    }

    /// <inheritdoc/>
    public string this[string propertyName] => GetErrors(propertyName).Cast<string>().FirstOrDefault() ?? string.Empty;

    /// <summary>
    /// Raises the <see cref="ErrorsChanged"/> event for the specified property name.
    /// </summary>
    protected virtual void OnErrorsChanged([CallerMemberName] string propertyName = "")
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Adds a validation error to the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property to add the error to.</param>
    /// <param name="error">The error message to add.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="propertyName"/> is <see langword="null"/> or empty.</exception>
    protected void AddError(string propertyName, string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        if (!errorsToProperty.TryGetValue(propertyName, out List<string>? value))
        {
            value = [];
            errorsToProperty[propertyName] = value;
        }

        value.Add(error);
        OnErrorsChanged(propertyName);
    }

    /// <summary>
    /// Clears all validation errors for the specified property.
    /// </summary>
    protected void ClearErrors(string? propertyName)
    {
        if (propertyName is null) return;

        if (errorsToProperty.Remove(propertyName))
        {
            OnErrorsChanged(propertyName);
        }
    }
}