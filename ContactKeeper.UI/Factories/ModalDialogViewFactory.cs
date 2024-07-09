using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.UI.Views;

namespace ContactKeeper.UI.Factories;

/// <inheritdoc cref="IModalDialogViewFactory"/>
internal class ModalDialogViewFactory : IModalDialogViewFactory
{
    /// <inheritdoc/>
    public IModalDialogView CreateModalDialogView() => new ModalDialogView();

    /// <inheritdoc/>

    public IModalDialogView CreateAboutSectionView() => new AboutSectionView();
}
