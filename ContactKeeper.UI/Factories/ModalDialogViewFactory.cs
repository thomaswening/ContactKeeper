using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.UI.Views;

namespace ContactKeeper.UI.Factories;

/// <summary>
/// Represents a factory for creating modal dialog views, actual implementation.
/// </summary>
internal class ModalDialogViewFactory : IModalDialogViewFactory
{
    /// <summary>
    /// Creates a new instance of a modal dialog view.
    /// </summary>
    /// <returns>A new instance of a modal dialog view.</returns>
    public IModalDialogView CreateModalDialogView()
    {
        return new ModalDialogView();
    }
}
