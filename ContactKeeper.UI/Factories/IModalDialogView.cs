using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.UI.ViewModels;

namespace ContactKeeper.UI.Factories;

/// <summary>
/// Abstraction for a modal dialog view to make it easier to test.
/// </summary>
internal interface IModalDialogView
{
    /// <inheritdoc cref="System.Windows.Controls.UserControl"/>
    object? DataContext { get; set; }
}
