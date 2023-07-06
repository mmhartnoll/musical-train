namespace Tools.Extensions
{
    public static class TaskExtensions
    {
        public static async void WithExceptionHandler(this Task task, Action<Exception> exceptionHandler)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                exceptionHandler.Invoke(ex);
            }
        }
    }
}