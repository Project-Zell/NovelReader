# Event Channels and Asynchronous Coordination

## EventChannel

`EventChannel` is a minimal `ScriptableObject` base class. It provides a Unity asset type from which specialized channels can derive.

## ChannelEvent

`ChannelEvent` wraps a parameterless C# event and exposes:
- `Subscribe(Action)`
- `Unsubscribe(Action)`
- `Raise()`

It is used where listeners need notification without a payload.

## DialogueEventChannel

`DialogueEventChannel` is a ScriptableObject-based channel for dialogue-specific events.

It exposes:
- `Subscribe<T>(Action<T>)` and `Unsubscribe<T>(Action<T>)` for event types implementing `IDialogueEvent`;
- `RaiseEvent<T>(T)` to dispatch a typed event;
- `RaiseEvent(IDialogueEvent)` to dispatch using the runtime event type;
- `OnEventsParseComplete`, a parameterless event raised after the parsed tag-event batch has been dispatched;
- `PauseRequestTraker`, a `TokenTracker` for pause requests;
- `WaitRequestTraker`, a `TokenTracker` for wait requests.

Subscribers are stored by concrete event `Type`. The channel dispatches to listeners registered for the exact runtime type; it does not walk base classes or interfaces to find compatible subscriptions.

## TokenTracker

`TokenTracker` coordinates overlapping requests, such as multiple systems independently requesting that reading pause or wait.

- `Request()` increments the outstanding request count.
- `Release()` decrements it if the count is greater than zero.
- `HasTokens` indicates whether any requests remain.
- `WaitUntilReleasedAsync()` completes immediately when no requests are outstanding, or returns a task that completes when the count returns to zero.
- `Subscribe(Action<bool>)` and `Unsubscribe(Action<bool>)` observe transitions between active and inactive.

A request-state notification is raised when the count changes from zero to one, and a release-state notification is raised when it returns to zero.

## ReaderEventController

The controller subscribes to:
- `ChangeReaderStyleDialogueEvent` on `DialogueEventChannel`;
- item-request notifications on `VariablesEventChannel`.

It queues corresponding actions. `ResolveEventsAsync()` executes queued actions in order and waits for the dialogue channel's wait tracker to be released after each action.

This is a small example of how reader-side actions can be mediated through channels rather than being called directly from the Ink player.

## Integration cautions

- Subscribers should unsubscribe when their lifetime ends; `ReaderEventController` implements `IDisposable` for this purpose.
- Every successful `TokenTracker.Request()` should have a corresponding `Release()`. A missing release leaves the reader waiting indefinitely.
- The supplied `TokenTracker` is not synchronized for multithreaded access. Treat it as a Unity-main-thread coordination utility unless additional synchronization is added.
- The channel and tracker implementation shown here is intentionally minimal and should not be mistaken for the full event/telemetry infrastructure of the original project.
