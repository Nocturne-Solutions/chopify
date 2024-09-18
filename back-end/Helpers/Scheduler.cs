namespace chopify.Helpers
{
    public class Scheduler
    {
        private readonly Dictionary<Guid, Task> _tasks = [];
        private readonly Dictionary<Guid, CancellationTokenSource> _cancellationTokens = [];
        private readonly Dictionary<Guid, Action> _cancelActions = [];

        public Guid AddTask(Action action, TimeSpan delay, Action? onCancel = null)
        {
            var tokenSource = new CancellationTokenSource();
            var taskId = Guid.NewGuid();

            var task = Task.Delay(delay, tokenSource.Token)
                            .ContinueWith(t =>
                            {
                                if (!tokenSource.Token.IsCancellationRequested)
                                {
                                    action();
                                    CleanUpTask(taskId);
                                }
                            }, TaskScheduler.Default);

            lock (_tasks)
            {
                _tasks[taskId] = task;
                _cancellationTokens[taskId] = tokenSource;

                if (onCancel != null)
                    _cancelActions[taskId] = onCancel;
            }

            return taskId;
        }

        public Guid AddPeriodicTask(Action action, TimeSpan interval, Action? onCancel = null)
        {
            var tokenSource = new CancellationTokenSource();
            var taskId = Guid.NewGuid();

            var task = Task.Run(async () =>
            {
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(interval, tokenSource.Token);
                    action();
                }
            }, tokenSource.Token);

            lock (_tasks)
            {
                _tasks[taskId] = task;
                _cancellationTokens[taskId] = tokenSource;

                if (onCancel != null)
                    _cancelActions[taskId] = onCancel;
            }

            return taskId;
        }

        public void CancelTask(Guid taskId)
        {
            lock (_tasks)
            {
                if (_cancellationTokens.TryGetValue(taskId, out CancellationTokenSource? value))
                {
                    value.Cancel();
                    
                    if (_cancelActions.TryGetValue(taskId, out Action? cancelAction))
                        cancelAction();

                    CleanUpTask(taskId);
                }
            }
        }

        public void StopAllTasks()
        {
            lock (_tasks)
            {
                foreach (var tokenSource in _cancellationTokens.Values)
                    tokenSource.Cancel();

                Task.WhenAll(_tasks.Values).Wait();
                _tasks.Clear();
                _cancellationTokens.Clear();
                _cancelActions.Clear();
            }
        }

        private void CleanUpTask(Guid taskId)
        {
            lock (_tasks)
            {
                _tasks.Remove(taskId);
                _cancellationTokens.Remove(taskId);
                _cancelActions.Remove(taskId);
            }
        }
    }
}
