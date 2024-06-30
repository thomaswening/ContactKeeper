using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactKeeper.UI.Services;

/// <summary>
/// Service for managing navigation between view models.
/// </summary>
internal interface INavigationService
{
    /// <summary>
    /// Gets the current view model.
    /// </summary>
    ObservableObject? CurrentViewModel { get; }

    /// <summary>
    /// Invoked when the current view model changes.
    /// </summary>
    event EventHandler? CurrentViewModelChanged;

    /// <summary>
    /// Registers a view model with the navigation service.
    /// </summary>
    void RegisterViewModel(ObservableObject viewModel);

    /// <summary>
    /// Unregisters a view model with the navigation service.
    /// </summary>
    void UnregisterViewModel(ObservableObject viewModel);
}