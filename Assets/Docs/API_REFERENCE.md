# API Reference

This reference covers the public API visible in the supplied archive. Some referenced types are omitted from the NDA-sanitized project snapshot.

## `StoryPlayer`

Unity `MonoBehaviour` responsible for advancing the Ink story.

| Member | Description |
|---|---|
| `CurrentLineIndex` | Current line index property; initialized to `0`, set to `-1` when a story is initialized in this implementation. |
| `OnLineProceed` | `Action<DialogueLineContainer>` callback field. |
| `OnSpace()` | Attempts to advance the story unless waiting for player input or a pause request. |
| `InitializeStory(Story)` | Receives the Ink story from the story channel. |
| `ProceedLine()` | Continues Ink, parses dialogue and tags, dispatches events, and publishes choices or story end. |

Serialized dependencies include story, dialogue, and variable event channels, a proceed button, and a canvas.

## `DialogueLineContainer`

Readonly struct containing `NameKey`, `DialogueKey`, `InLineEvents`, and `IsThoughts`.

- Constructor accepts optional values for all four fields.
- `Merge(DialogueLineContainer fallbackLine)` fills empty string fields from the fallback and combines thought flags with OR.

## `DialogueUtilities`

Static helper.

- `ParseToDialogue(string raw)` parses a raw Ink line into a `DialogueLineContainer`.

## `EventParser`

Static helper in `Core.Story.Events`.

- `GetTagEvents(DialogueLineContainer dialogueLine, List<string> tags)` converts recognized tags to `IDialogueEvent[]` and adds default character presentation events.
- `ParseInt(string[] settings, int elementIndex, int defaultValue = 0)` parses an integer or returns the default.
- `ParseFloat(string[] settings, int elementIndex, float defaultValue = 0f)` parses a float or returns the default.
- `ParseBool(string[] settings, int elementIndex, bool defaultValue = false)` parses a boolean or returns the default.

## `IDialogueEvent`

Marker interface for typed dialogue events.

## `ChangeBackgroundDialogueEvent`

Concrete `IDialogueEvent` with:
- `BackgroundSpritePath`
- `FadeDurationInSeconds`
- `DoFadeInBlack`

Constructor: `ChangeBackgroundDialogueEvent(string values)`, where `values` is comma-separated.

## `EventChannel`

Empty `ScriptableObject` base class for event channel assets.

## `ChannelEvent`

Parameterless event wrapper:
- `Subscribe(Action action)`
- `Unsubscribe(Action action)`
- `Raise()`

## `DialogueEventChannel`

Typed dialogue-event channel:
- `Subscribe<T>(Action<T> action) where T : IDialogueEvent`
- `Unsubscribe<T>(Action<T> action) where T : IDialogueEvent`
- `RaiseEvent<T>(T dialogueEvent) where T : IDialogueEvent`
- `RaiseEvent(IDialogueEvent dialogueEvent)`

Also exposes `PauseRequestTraker`, `WaitRequestTraker`, and `OnEventsParseComplete`.

## `TokenTracker`

Request counter and asynchronous release signal:
- `HasTokens`
- `Request()`
- `Release()`
- `WaitUntilReleasedAsync()`
- `Subscribe(Action<bool> action)`
- `Unsubscribe(Action<bool> action)`

## `ReaderEventController`

Implements `IDisposable`.

- Constructor accepts an `EventChannelDistributor`.
- `Dispose()` removes subscriptions.
- `ResolveEventsAsync()` drains the queued reader actions and waits for wait requests to be released.

The implementation depends on `StoryEventChannel`, `VariablesEventChannel`, `ChangeReaderStyleDialogueEvent`, and `ItemInstance`, which are not included in this archive.

## Referenced but omitted types

The following names are referenced by the included code but their definitions are not in the supplied archive:

- `StoryEventChannel`
- `VariablesEventChannel`
- `EventChannelDistributor`
- `ChangeReaderStyleDialogueEvent`
- `ShowCharacterVisualDialogueEvent`
- `CharacterHideDialogueEvent`
- `ChangeSpeakerPositionDialogueEvent`
- `ICharacterVisualDialogueEvent`
- `CharacterPositionType`
- `ChoiceUtilities`
- `ItemInstance`

Their behavior should be documented from the corresponding full project source rather than inferred from call sites.
