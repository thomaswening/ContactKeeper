using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactKeeper.UI.Services;

internal class NavigationService()
{
    private readonly List<ObservableObject> viewModels = [];

    public event EventHandler? CurrentViewModelChanged;
    public ObservableObject CurrentViewModel => viewModels.LastOrDefault() 
        ?? throw new InvalidOperationException("No view models are currently registered.");

    public void RegisterViewModel(ObservableObject viewModel)
    {
        viewModels.Add(viewModel);
        CurrentViewModelChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UnregisterViewModel(ObservableObject viewModel)
    {
        if (!viewModels.Contains(viewModel))
        {
            return;
        }

        viewModels.Remove(viewModel);
        CurrentViewModelChanged?.Invoke(this, EventArgs.Empty);
    }
}
