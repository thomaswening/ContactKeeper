using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactKeeper.UI.ViewModels;
internal partial class MainWindowVm : ObservableObject
{
    [ObservableProperty]
    private ObservableObject currentViewModel;

    public MainWindowVm()
    {
        CurrentViewModel = new ContactsOverviewVm();
    }
}
