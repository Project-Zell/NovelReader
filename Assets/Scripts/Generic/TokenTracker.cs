using System;
using System.Threading.Tasks;

public class TokenTracker
{
    public bool HasTokens => _tokenCount > 0;

    private event Action<bool> OnStateChange;
    private int _tokenCount;

    private TaskCompletionSource<bool> _releaseTask = new();
    public void Request()
    {
        _tokenCount++;
        if (_tokenCount == 1)
        {
            OnStateChange?.Invoke(true);
            _releaseTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }

    public void Release()
    {
        if (_tokenCount == 0)
            return;

        _tokenCount--;

        if (_tokenCount == 0)
        {
            _releaseTask.TrySetResult(true);
            OnStateChange?.Invoke(false);
        }
    }

    public Task WaitUntilReleasedAsync()
    {
        if (_tokenCount == 0)
            return Task.CompletedTask;

        return _releaseTask.Task;
    }

    public void Subscribe(Action<bool> action) => OnStateChange += action;
    public void Unsubscribe(Action<bool> action) => OnStateChange -= action;
}