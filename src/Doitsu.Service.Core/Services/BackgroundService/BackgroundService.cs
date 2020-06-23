using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
namespace Doitsu.Service.Core.Services.BackgroundService
{
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _stoppingCts = null;
        private Task _executingTask;

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public virtual Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken == default(CancellationToken) && _cancellationToken == null)
                throw new InvalidOperationException($"{nameof(_cancellationToken)} has not been initialized");
            _cancellationToken = cancellationToken;

            if (_stoppingCts == null)
                _stoppingCts = new CancellationTokenSource();

            _executingTask = ExecuteAsync(_stoppingCts.Token);
            if (_executingTask.IsCompleted)
                return _executingTask;
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken == default(CancellationToken) && _cancellationToken == null)
                throw new InvalidOperationException($"{nameof(_cancellationToken)} has not been initialized");

            if (_executingTask == null)
                return;
            try
            {
                Dispose();
            }
            finally
            {
                Task task = await Task.WhenAny(_executingTask, Task.Delay(-1, _cancellationToken));
            }
        }

        public virtual void Dispose()
        {
            if (_stoppingCts != null && !_stoppingCts.IsCancellationRequested)
            {
                _stoppingCts.Cancel();
                _stoppingCts.Dispose();
                _stoppingCts = null;
            }
        }
    }
}