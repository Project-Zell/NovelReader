using Core.Story.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Dialogue Event Channel", menuName = "Channel/Dialogue Event Channel")]
public class DialogueEventChannel : EventChannel
{
    public TokenTracker PauseRequestTraker { get; private set; } = new();
    public TokenTracker WaitRequestTraker { get; private set; } = new();
    public ChannelEvent OnEventsParseComplete { get; private set; } = new();

    private readonly Dictionary<Type, Delegate> _events = new();

    public void Subscribe<T>(Action<T> action) where T : IDialogueEvent
    {
        _events[typeof(T)] = (Action<T>)_events.GetValueOrDefault(typeof(T)) + action;
    }

    public void Unsubscribe<T>(Action<T> action) where T : IDialogueEvent
    {
        _events[typeof(T)] = (Action<T>)_events.GetValueOrDefault(typeof(T)) - action;
    }

    public void RaiseEvent<T>(T dialogueEvent) where T : IDialogueEvent
    {
        if (_events.TryGetValue(typeof(T), out var eventDelegate))
            ((Action<T>)eventDelegate)?.Invoke(dialogueEvent);
    }

    public void RaiseEvent(IDialogueEvent dialogueEvent)
    {
        if (_events.TryGetValue(dialogueEvent.GetType(), out var eventDelegate))
        {
            eventDelegate.DynamicInvoke(dialogueEvent);
        }
    }
}