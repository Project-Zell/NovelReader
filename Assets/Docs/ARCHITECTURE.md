# Architecture and Reading Flow

## Design intent

The reader is built around Ink as the narrative source. Writers create story content in `.ink` files; the Unity runtime loads or receives a compiled Ink story, advances it, parses its output, and turns tags into events that presentation systems can handle.

The intention is to separate **story authoring** from **game presentation**:
- Ink controls narrative progression, dialogue, and choices.
- The reader core interprets the current line and its tags.
- Event channels communicate presentation requests without tightly coupling the Ink player to individual UI components.
- UI and presentation components react to those requests.

This makes it possible for writers to create playable narrative content primarily through Ink rather than implementing each line or branch in Unity.

## Main components

### `StoryPlayer`

The central playback coordinator and a Unity `MonoBehaviour`.

It:
- receives an initialized `Ink.Runtime.Story` through `StoryEventChannel`;
- advances the story with `Story.Continue()`;
- parses each returned line with `DialogueUtilities`;
- parses the story's current tags with `EventParser`;
- raises typed dialogue events through `DialogueEventChannel`;
- publishes dialogue continuation and choice data through the story channel;
- waits for player input when choices are available;
- ends playback when Ink can no longer continue.

The player also prevents advancing while a pause request is active.

### `DialogueLineContainer`

A readonly value type representing a parsed line. It contains:
- `NameKey`: speaker identifier;
- `DialogueKey`: dialogue text;
- `InLineEvents`: inline event data attached to the speaker segment;
- `IsThoughts`: whether the text was wrapped in parentheses.

`Merge` combines a line with a fallback line. Empty string values are taken from the fallback; `IsThoughts` is combined with logical OR.

### `DialogueUtilities`

Converts raw text returned by Ink into a `DialogueLineContainer`. It recognizes the first colon as the separator between speaker and dialogue. If there is no colon, the entire line is treated as dialogue without a speaker.

Within the speaker segment, the first comma separates the speaker name from inline event data. If the dialogue text begins and ends with parentheses, those parentheses are removed and `IsThoughts` is set.

### `EventParser`

Converts Ink tags into `IDialogueEvent` instances using a tag registry. The included registry recognizes `bg`, `show`, `hide`, and `position`.

It also inserts default character presentation events based on whether the parsed line has a speaker and whether an explicit character visual or position event already exists.

### Event channels

`DialogueEventChannel` provides typed event dispatch for classes implementing `IDialogueEvent`. Other event channels are referenced by the reader and controller but are not included in this archive.

`ChannelEvent` is a small parameterless C# event wrapper with `Subscribe`, `Unsubscribe`, and `Raise` methods.

### `ReaderEventController`

Bridges certain dialogue and variable events to story-level requests. It queues actions and resolves them sequentially. After each queued action it waits until the dialogue channel's wait-request tracker has been released.

### `TokenTracker`

Tracks overlapping pause or wait requests. Each `Request()` increments a counter; each `Release()` decrements it. Consumers can await `WaitUntilReleasedAsync()` until the count returns to zero.

## Reading sequence

1. A story initialization event supplies an Ink `Story` to `StoryPlayer`.
2. The player calls `Continue()` when it is allowed to advance.
3. The returned text is parsed into a `DialogueLineContainer`.
4. Current Ink tags are parsed into typed dialogue events.
5. The player raises those events and publishes the parsed dialogue line.
6. If the story exposes choices, the player publishes the choice data and waits for a choice selection.
7. Once the player selects a choice, the Ink story is told which choice index was selected and playback continues.
8. When Ink can no longer continue, the player hides the reader canvas and raises the story-end event.

## StreamingAssets and asset loading

The project description indicates that the reader loads assets from StreamingAssets, but the corresponding complete loader and asset-presentation components are not present in the supplied archive. The included background event carries a sprite path and fade parameters, demonstrating the kind of asset reference passed from narrative tags to presentation code. The exact path resolution, file loading, caching, and platform-specific behavior cannot be documented from this source snapshot.

## Deliberately omitted functionality

The original project supports more advanced systems, including custom code execution, complex events, and telemetry. The supplied source is a stripped-down reader core and does not expose those systems. Some event types, channels, and presentation classes referenced by the included code are also absent due to the NDA-oriented reduction.
