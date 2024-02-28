namespace DropBear.Codex.Utilities.Helpers;

/// <summary>
///     Provides helper methods for working with tasks, including implementing timeouts on tasks.
/// </summary>
public static class TaskHelper
{
    /// <summary>
    ///     Applies a timeout to a <see cref="TaskCompletionSource{TResult}" />.
    /// </summary>
    /// <typeparam name="TResult">The result type of the task.</typeparam>
    /// <param name="taskSource">The task completion source to apply the timeout to.</param>
    /// <param name="timeout">The duration after which the task should be canceled if not completed.</param>
    /// <returns>The task's result if completed within the timeout; otherwise throws a <see cref="TaskCanceledException" />.</returns>
    /// <exception cref="TaskCanceledException">Thrown if the task does not complete within the specified timeout.</exception>
    public static async Task<TResult> TimeoutAfter<TResult>(this TaskCompletionSource<TResult> taskSource,
        TimeSpan timeout)
    {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();
        var completedTask =
            await Task.WhenAny(taskSource.Task, Task.Delay(timeout, timeoutCancellationTokenSource.Token))
                .ConfigureAwait(false);

        if (completedTask == taskSource.Task)
        {
            await timeoutCancellationTokenSource.CancelAsync()
                .ConfigureAwait(false); // Cancel the delay task to free up resources.
            return await taskSource.Task.ConfigureAwait(false); // Return the result of the completed task.
        }

        // If the task source task did not complete, attempt to transition it to a Canceled state.
        taskSource.TrySetCanceled(timeoutCancellationTokenSource.Token);

        throw new TaskCanceledException("The operation has timed out.");
    }
}

// Example Usage:
// public async Task<string> PerformOperationWithTimeoutAsync()
// {
//     var taskCompletionSource = new TaskCompletionSource<string>();
//     var longRunningTask = PerformLongRunningOperationAsync();
//
//     longRunningTask.ContinueWith(task =>
//     {
//         if (task.IsFaulted)
//         {
//             taskCompletionSource.TrySetException(task.Exception.InnerExceptions);
//         }
//         else if (task.IsCanceled)
//         {
//             taskCompletionSource.TrySetCanceled();
//         }
//         else
//         {
//             taskCompletionSource.TrySetResult(task.Result);
//         }
//     }, TaskContinuationOptions.ExecuteSynchronously);
//
//     try
//     {
//         return await taskCompletionSource.TimeoutAfter(TimeSpan.FromSeconds(5));
//     }
//     catch (TaskCanceledException)
//     {
//         return "Operation Timed Out";
//     }
// }
