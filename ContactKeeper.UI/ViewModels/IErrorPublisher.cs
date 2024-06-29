using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactKeeper.UI.ViewModels;

/// <summary>
/// Represents a publisher of error messages.
/// </summary>
internal interface IErrorPublisher
{
    /// <summary>
    /// Invoked when an error occurs.
    /// </summary>
    event EventHandler<string>? ErrorOccured;
}
