using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ContactKeeper.UI.Factories;

namespace ContactKeeper.UI.Views;
/// <summary>
/// Interaction logic for AboutSectionView.xaml
/// </summary>
public partial class AboutSectionView : UserControl, IModalDialogView
{
    public AboutSectionView()
    {
        InitializeComponent();
    }
}
