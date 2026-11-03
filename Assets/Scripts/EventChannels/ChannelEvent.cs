using System;

public class ChannelEvent
{
    private event Action Action;

    public void Raise() => Action?.Invoke();
    public void Subscribe(Action action) => Action += action;
    public void Unsubscribe(Action action) => Action -= action;
}
