using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactKeeper.UI.Events;

/// <summary>
/// Event data that is used to request a return value from the subscriber.
/// </summary>
/// <typeparam name="T">The type of the return value.</typeparam>
internal class AwaitableEventArgs<T> : EventArgs
{
    private readonly TaskCompletionSource<T> tcs = new();

    /// <summary>
    /// Gets the task that represents the operation.
    /// </summary>
    public Task<T> Task => tcs.Task;

    /// <summary>
    /// Sets the result of the operation.
    /// </summary>
    public void SetResult(T result) => tcs.SetResult(result);
}
