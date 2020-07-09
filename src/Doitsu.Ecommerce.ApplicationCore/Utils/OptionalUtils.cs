using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Optional;

namespace Doitsu.Ecommerce.ApplicationCore.Utils
{
    public static class OptionalUtils
    {
        #region Disposable
        public static void Using<T>(Func<T> factory, Action<T> map)where T : IDisposable
        {
            using(var disposable = factory())map(disposable);
        }

        public static TR Using<T, TR>(Func<T> factory, Func<T, TR> map)where T : IDisposable
        {
            using(var disposable = factory())return map(disposable);
        }

        public static async Task UsingAsync<T>(Func<T> factory, Func<T, Task> map)where T : IDisposable
        {
            using(var tDisposable = factory())await map(tDisposable);
        }

        public static async Task<TR> UsingAsync<T, TR>(Func<T> factory, Func<T, Task<TR>> map)where T : IDisposable
        {
            using(var tDisposable = factory())return await map(tDisposable);
        }
        #endregion

        public static byte[] ToUtf8Bytes(this string value) => value.Map(Encoding.UTF8.GetBytes);

        #region Functional Programming
        public static TR Map<T, TR>(this T @this, Func<T, TR> map) => map(@this);

        public static T Retry<T>(this Func<T> function, int retries = 3)
        {
            try
            {
                return function();
            }
            catch
            {
                return retries > 1 ? function.Retry(retries - 1) : default(T);
            }
        }

        public static Option<T, TError> Retry<T, TError>(this Func<Option<T, TError>> @this, int retries = 3)
        {
            if (@this == null)throw new ArgumentNullException(nameof(@this));

            return @this().Match(Option.Some<T, TError>,
                error => retries > 1 ? @this.Retry(retries - 1) : Option.None<T, TError>(error));
        }

        public static async Task<T> Retry<T>(this Func<Task<T>> task, int retries, TimeSpan delay, CancellationToken cts = default(CancellationToken)) =>
            await task().ContinueWith(async innerTask =>
            {
                cts.ThrowIfCancellationRequested();
                if (innerTask.Status != TaskStatus.Faulted)
                    return innerTask.Result;
                if (retries == 0)
                    throw innerTask.Exception ??
                        throw new Exception();
                await Task.Delay(delay, cts);
                return await task.Retry(retries - 1, delay, cts);
            }, cts).Unwrap();

        public static Task<T> Otherwise<T>(this Task<T> task, Func<Task<T>> orTask) =>
            task.ContinueWith(async innerTask => innerTask.Status == TaskStatus.Faulted ?
                await orTask() : await Task.FromResult(innerTask.Result)).Unwrap();

        public static Func<TA, Func<TB, TR>> Curry<TA, TB, TR>(this Func<TA, TB, TR> function) => a => b => function(a, b);

        public static Func<TA, TB, TR> Uncurry<TA, TB, TR>(Func<TA, Func<TB, TR>> function) => (x, y) => function(x)(y);

        public static Func<TB, TR> Partial<TA, TB, TR>(this Func<TA, TB, TR> function, TA argument) => argument2 => function(argument, argument2);

        #endregion

    }
}