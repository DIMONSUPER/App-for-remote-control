namespace SmartMirror.Helpers
{
    public static class TaskRepeater
    {
        #region -- Public helpers --

        public static async Task<bool> RepeatAsync(Func<Task<bool>> task, TimeSpan executionTime, int delayInMiliseconds = 100, CancellationToken token = default)
        {
            bool isSuccess = false;
            var timeToStopExecution = DateTime.UtcNow + executionTime;

            while (!isSuccess && DateTime.UtcNow < timeToStopExecution && !token.IsCancellationRequested)
            {
                isSuccess = await task();

                await Task.Delay(delayInMiliseconds, token);
            }

            return isSuccess;
        }

        #endregion
    }
}