using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ContactKeeper.UI.Utilities;

namespace ContactKeeper.UI.ViewModels;
internal partial class MainWindowVm(ObservableObject initialViewModel) : ObservableObject
{
    [ObservableProperty]
    private ObservableObject currentViewModel = initialViewModel ?? throw new ArgumentNullException(nameof(initialViewModel));
}
