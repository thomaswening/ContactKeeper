using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ContactKeeper.UI.Utilities;

namespace ContactKeeper.UI.ViewModels;
internal partial class MainWindowVm(ViewModelFactory viewModelFactory) : ObservableObject
{
    private readonly ViewModelFactory viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));

    [ObservableProperty]
    private ObservableObject? currentViewModel;

    public async Task InitializeAsync()
    {
        CurrentViewModel = await viewModelFactory.CreateContactsOverviewVmAsync();
    }
}
