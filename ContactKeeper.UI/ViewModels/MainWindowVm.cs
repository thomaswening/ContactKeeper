using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ContactKeeper.UI.Utilities;

namespace ContactKeeper.UI.ViewModels;
internal partial class MainWindowVm : ObservableObject
{
    public event EventHandler? DefaultViewModelRequested;
    public event EventHandler? AboutSectionRequested;

    [ObservableProperty]
    private ObservableObject? currentViewModel;

    public MainWindowVm(ObservableObject initialViewModel)
    {
        PropertyChanged += OnPropertyChanged;
        CurrentViewModel = initialViewModel ?? throw new ArgumentNullException(nameof(initialViewModel));
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // If the current view model is set to null, request the default view model
        if (e.PropertyName == nameof(CurrentViewModel) && CurrentViewModel is null)
        {
            DefaultViewModelRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private void ShowAboutSection()
    {
        AboutSectionRequested?.Invoke(this, EventArgs.Empty);
    }
}
