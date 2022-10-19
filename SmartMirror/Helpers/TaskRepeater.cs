namespace SmartMirror.Helpers
{
    public class TaskRepeater
    {
        #region -- Public helpers --

        public static async Task<bool> Repeate(Func<Task<bool>> task, Func<bool> canExecute = null, int delayInMiliseconds = 100)
        {
            bool isSuccess = false;

            do
            {
                isSuccess = await task();

                await Task.Delay(delayInMiliseconds);

            } while (!isSuccess && (canExecute is null || canExecute()));

            return isSuccess;
        } 

        #endregion
    }
}
