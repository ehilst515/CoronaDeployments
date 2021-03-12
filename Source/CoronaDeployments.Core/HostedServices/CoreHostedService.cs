using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CoronaDeployments.Core.Runner;

namespace CoronaDeployments.Core.HostedServices
{
    public class CoreHostedService : IHostedService
    {
        private Timer _timer;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly TimeSpan _periodicInterval;
        private readonly IServiceProvider _services;

        private readonly BackgroundRunner _runner;

        public CoreHostedService(IBackgroundTaskQueue taskQueue, IServiceProvider services)
        {
            _taskQueue = taskQueue;

            _periodicInterval = TimeSpan.FromSeconds(2);

            _services = services;

            _runner = new BackgroundRunner("Project X", new BuildAction(), null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information($"{nameof(CoreHostedService)} is running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, _periodicInterval);

            _runner.Start();

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            // Disable timer.
            _timer.Change(-1, -1);

            // Inner loop.
            {
                try
                {
                    //Log.Information($"{nameof(CoreHostedService)}: Doing some work...");

                    //var workItem = await _taskQueue.DequeueAsync(CancellationToken.None);
                }
                catch (Exception exp)
                {
                    Log.Error(exp, string.Empty);
                }
            }

            // Re-enable the timer.
            _timer.Change(_periodicInterval, _periodicInterval);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"{nameof(CoreHostedService)} is stopping.");

            _runner.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}