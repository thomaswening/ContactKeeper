using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Navigation;

using Microsoft.Xaml.Behaviors;

namespace ContactKeeper.UI.Behaviors;

internal class HyperlinkNavigationBehavior : Behavior<Hyperlink>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.RequestNavigate += OnRequestNavigate;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.RequestNavigate -= OnRequestNavigate;
        base.OnDetaching();
    }

    private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}
