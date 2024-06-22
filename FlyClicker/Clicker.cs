using WindowsInput;

public class Clicker
{
    private CancellationTokenSource _cancellationTokenSource;
    private Task _clickingTask;
    private readonly InputSimulator _inputSimulator = new();

    public int Interval { get; set; }
    public int Jitter { get; set; }

    public void Start()
    {
        if (_clickingTask != null && !_clickingTask.IsCompleted)
        {
            return; // Already running
        }

        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        _clickingTask = Task.Run(async () =>
        {
            var random = new Random();
            while (!token.IsCancellationRequested)
            {
                _inputSimulator.Mouse.LeftButtonClick();
                int jitter = random.Next(-Jitter, Jitter);
                await Task.Delay(Interval + jitter, token);
            }
        }, token);
    }

    public async Task StopAsync()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            try
            {
                if (_clickingTask != null)
                {
                    await _clickingTask; // Await the task to handle task completion and exceptions correctly
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }

    public void Stop()
    {
        StopAsync().Wait();
    }

    public bool IsRunning()
    {
        return _clickingTask != null && !_clickingTask.IsCompleted;
    }
}