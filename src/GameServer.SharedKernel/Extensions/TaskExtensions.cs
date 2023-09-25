namespace GameServer.SharedKernel.Extensions;

public static class TaskExtensions
{
  public static void RunAndForget(this Task task, Action<Exception>? errorHandler = null)
  {
    task.ContinueWith(t => errorHandler?.Invoke(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);
  }
}
