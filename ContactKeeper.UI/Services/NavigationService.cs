using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Serilog;

namespace ContactKeeper.UI.Services;

/// <summary>
/// Service for managing navigation between view models.
/// </summary>
internal class NavigationService(ILogger logger)
{
    private readonly List<ObservableObject> viewModels = [];

    /// <summary>
    /// Invoked when the current view model changes.
    /// </summary>
    public event EventHandler? CurrentViewModelChanged;

    public ObservableObject? CurrentViewModel
    {
        get
        {
            var lastViewModel = viewModels.LastOrDefault();
            if (lastViewModel is null)
            {
                logger.Warning("No view models are registered with the navigation service.");
            }

            return lastViewModel;
        }
    }

    /// <summary>
    /// Registers a view model with the navigation service and invokes the <see cref="CurrentViewModelChanged"/> event.
    /// </summary>
    /// <param name="viewModel">The view model to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public void RegisterViewModel(ObservableObject viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel, nameof(viewModel));

        logger.Information($"Registering view model of type {viewModel.GetType().Name} with the navigation service.");
        viewModels.Add(viewModel);
        CurrentViewModelChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Unregisters a view model with the navigation service and invokes the <see cref="CurrentViewModelChanged"/> event.
    /// </summary>
    /// <param name="viewModel">The view model to unregister.</param>
    public void UnregisterViewModel(ObservableObject viewModel)
    {
        if (viewModel is null) 
        {
            logger.Warning($"Attempted to unregister a null view model. Ignoring request.");
            return;
        }

        if (!viewModels.Contains(viewModel))
        {
            logger.Warning($"Attempted to unregister a view model that is not registered with the navigation service. Ignoring request.");
            return;
        }

        viewModels.Remove(viewModel);
        CurrentViewModelChanged?.Invoke(this, EventArgs.Empty);
    }
}
