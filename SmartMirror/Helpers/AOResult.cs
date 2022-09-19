using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ODBmobile.Helpers
{
    public class AOResult
    {
        private readonly DateTime _creationUtcTime;

        public AOResult([CallerMemberName] string callerName = null,
                        [CallerFilePath] string callerFile = null,
                        [CallerLineNumber] int callerLineNumber = 0)
        {
            _creationUtcTime = DateTime.UtcNow;
            CallerName = callerName;
            CallerFile = callerFile;
            CallerLineNumber = callerLineNumber;
        }

        #region -- Public properties --
        
        public TimeSpan OperationTime { get; private set; }
        
        public bool IsSuccess { get; private set; }
        
        public Exception Exception { get; private set; }
        
        public string ErrorId { get; private set; }
        
        public string Message { get; private set; }
        
        public string CallerName { get; private set; }
        
        public string CallerFile { get; private set; }
        
        public int CallerLineNumber { get; private set; }
        
        public bool TrackingResult { get; set; } = true;
        
        #endregion
        
        #region -- Public methods --
        
        public void MergeResult(AOResult res)
        {
            CallerName = res.CallerName;
            CallerLineNumber = res.CallerLineNumber;
            CallerFile = res.CallerFile;
            SetResult(res.IsSuccess, res.ErrorId, res.Message, res.Exception);
        }

        public void SetSuccess()
        {
            SetResult(true, null, null, null);
        }

        public void SetFailure()
        {
            SetResult(false, null, null, null);
        }

        public void SetFailure(string message)
        {
            SetResult(false, null, message, null);
        }

        public void SetFailure(Exception ex)
        {
            SetResult(false, null, null, ex);
        }

        public void ArgumentException(string argumentName, string message)
        {
            SetError("ArgumentException", $"argumentName: {argumentName}, message: {message}");
        }

        public void ArgumentNullException(string argumentName)
        {
            SetError("ArgumentNullException", $"argumentName: {argumentName}");
        }

        public void SetError(string errorId, string message, Exception ex = null)
        {
            SetResult(false, errorId, message, ex);
        }

        protected void SetResult(bool isSuccess, string errorId, string message, Exception ex)
        {
            var finishTime = DateTime.UtcNow;
            OperationTime = finishTime - _creationUtcTime;
            IsSuccess = isSuccess;
            ErrorId = errorId;
            Exception = ex;
            Message = message;
        }

        #endregion

        #region -- Static Helpers --

        public static async Task<AOResult> ExecuteTaskAsync(Func<Action<string>, Task> task, [CallerMemberName] string callerName = null, [CallerFilePath] string callerFile = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            var res = new AOResult(callerName, callerFile, callerLineNumber);

            await ExecuteTaskAsync(res, (onFailure) => task(onFailure), t =>
            {
                res.SetSuccess();
            }, callerName, callerFile, callerLineNumber);

            return res;
        }

        public static async Task<AOResult<T>> ExecuteTaskAsync<T>(Func<Action<string>, Task<T>> task, [CallerMemberName] string callerName = null, [CallerFilePath] string callerFile = null, [CallerLineNumber] int callerLineNumber = 0)
        {
            var res = new AOResult<T>(callerName, callerFile, callerLineNumber);
            await ExecuteTaskAsync(res, (onFailure) => task(onFailure), t =>
            {
                var genericT = (Task<T>)t;
                res.SetSuccess(genericT.Result);
            }, callerName, callerFile, callerLineNumber);

            return res;
        }

        protected static async Task ExecuteTaskAsync(AOResult res, Func<Action<string>, Task> task, Action<Task> onSuccess, string callerName, string callerFile, int callerLineNumber)
        {
            bool isOnFailureExecuted = false;

            Action<string> onFailure = (message) =>
            {
                isOnFailureExecuted = true;
                res.SetFailure(message);
            };

            try
            {
                var t = task(onFailure);
                await t;

                if (!isOnFailureExecuted)
                {
                    onSuccess(t);
                }
            }
            catch (Exception ex)
            {
                res.SetError($"Exception in {callerName} file: {callerFile} line:{callerLineNumber}", ex.Message, ex);
            }
        }

        #endregion
    }

    public class AOResult<T> : AOResult
    {

        public AOResult([CallerMemberName] string callerName = null, [CallerFilePath] string callerFile = null, [CallerLineNumber] int callerLineNumber = 0)
            : base(callerName, callerFile, callerLineNumber)
        {
        }

        #region -- Public properties --

        public T Result { get; private set; }

        #endregion

        #region -- Public methods --

        public void MergeResult(T result, AOResult res)
        {
            Result = result;
            MergeResult(res);
        }

        public void SetSuccess(T result)
        {
            Result = result;
            SetSuccess();
        }

        public void SetResult(T result, bool isSuccess, string errorId, string message, Exception ex = null)
        {
            Result = result;
            SetResult(isSuccess, errorId, message, ex);
        }

        public void SetFailure(T result)
        {
            Result = result;
            SetFailure();
        }

        public async Task<AOResult<T>> ExecuteCallAsync(Func<Task<T>> func)
        {
            try
            {
                var result = await func();

                if (result is not null)
                {
                    SetSuccess(result);
                }
                else
                {
                    SetFailure();
                }
            }
            catch (Exception ex)
            {
                SetError(ex.HResult.ToString(), ex.Message, ex);
            }

            return this;
        }

        #endregion
    }

    public class AOResult<TResult, TStatus> : AOResult<TResult>
    {
        #region -- Public properties --

        public TStatus Status { get; private set; }

        #endregion

        #region -- Public methods --

        public void SetFailure(Exception ex, TStatus status)
        {
            Status = status;
            SetFailure(ex);
        }
        public void SetFailure(TStatus status)
        {
            Status = status;
            SetFailure();
        }

        #endregion
    }
}
