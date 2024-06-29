using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactKeeper.UI.ViewModels;

/// <summary>
/// Represents the options for a dialog.
/// </summary>
internal class DialogOptions
{
    /// <summary>
    /// Gets or sets the title of the dialog.
    /// Default value is "Info".
    /// </summary>
    public string Title { get; set; } = "Info";

    /// <summary>
    /// Gets or sets the Text on the OK button.
    /// Default value is "OK".
    /// </summary>
    public string OkText { get; set; } = "OK";

    /// <summary>
    /// Gets or sets the Text on the Cancel button.
    /// Default value is "Cancel".
    /// </summary>
    public string CancelText { get; set; } = "Cancel";

    /// <summary>
    /// Whether the OK button should be shown.
    /// Default value is true.
    /// </summary>
    public bool ShowOkButton { get; set; } = true;

    /// <summary>
    /// Whether the Cancel button should be shown.
    /// Default value is true.
    /// </summary>
    public bool ShowCancelButton { get; set; } = true;
}