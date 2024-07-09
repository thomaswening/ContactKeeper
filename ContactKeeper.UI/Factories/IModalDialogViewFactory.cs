namespace ContactKeeper.UI.Factories;

/// <summary>
/// Represents a factory for creating modal dialog views, used to make it easier to test.
/// </summary>
internal interface IModalDialogViewFactory
{
    /// <summary>
    /// Creates a new instance of an about section view.
    /// </summary>s
    IModalDialogView CreateAboutSectionView();

    /// <summary>
    /// Creates a new instance of a modal dialog view.
    /// </summary>
    IModalDialogView CreateModalDialogView();
}