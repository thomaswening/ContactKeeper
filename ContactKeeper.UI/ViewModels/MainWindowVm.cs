using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ContactKeeper.UI.Utilities;

namespace ContactKeeper.UI.ViewModels;
internal partial class MainWindowVm : ObservableObject
{
    private readonly ViewModelFactory viewModelFactory;

    [ObservableProperty]
    private ObservableObject currentViewModel;

    public MainWindowVm(ViewModelFactory viewModelFactory)
    {
        this.viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
        CurrentViewModel = viewModelFactory.CreateContactsOverviewVm();
    }
}
