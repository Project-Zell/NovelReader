# Ink-Based Novel Reader

An isolated Unity implementation of the core text-reading pipeline from a larger text-based novel project. Ink (`.ink`) is the primary narrative source: writers author story content in Ink, and the reader runtime advances the story, interprets dialogue and tags, dispatches presentation events, and coordinates player input.

The goal is to let writers turn an Ink story into a playable reading experience without having to build the narrative flow directly in Unity. Narrative content is authored in Ink; Unity components handle playback and presentation.

This repository is a **simplified and partially stripped-down sample**. Some components have been reduced or omitted to comply with NDA requirements. The original project supports a broader feature set, including custom code execution, more complex events, and telemetry. Those capabilities should not be assumed to exist in this sample.

## Core responsibilities

- **Ink playback:** use `Ink.Runtime.Story` to continue the narrative and present choices.
- **Dialogue parsing:** convert raw Ink lines into a structured `DialogueLineContainer`.
- **Tag parsing:** interpret recognized Ink tags as typed dialogue events.
- **Presentation dispatch:** publish story and dialogue events through event channels so UI and presentation systems can respond.
- **Player input:** advance the story, select choices, and respect pause requests.
- **Event coordination:** queue certain reader-side actions and wait for asynchronous dialogue requests to be released.

## How the reading flow works

```text
Ink story (.ink)
      │
      ▼
Ink.Runtime.Story
      │ Continue()
      ▼
Raw dialogue line + current tags
      │
      ├── DialogueUtilities.ParseToDialogue()
      │       └── DialogueLineContainer
      │
      └── EventParser.GetTagEvents()
              └── IDialogueEvent[]
                       │
                       ▼
              DialogueEventChannel
                       │
             ┌─────────┴──────────┐
             ▼                    ▼
      Presentation systems   Reader systems
      (background,            (reader style,
       character, etc.)        item requests, etc.)
                       │
                       ▼
               Story/UI updates
                       │
                       ▼
               Player advances
```

At a high level, `StoryPlayer` is the playback coordinator. It calls `Continue()` on the Ink story, parses the returned line, parses the story's current tags, raises the resulting events, and notifies the rest of the application that a dialogue line or choices are ready.

## Project structure

```text
Assets/
└── Scripts/
    ├── DialogueEvents/
    │   ├── ChangeBackgroundDialogueEvent.cs
    │   └── IDialogueEvent.cs
    ├── EventChannels/
    │   ├── ChannelEvent.cs
    │   ├── DialogueEventChannel.cs
    │   └── EventChannel.cs
    ├── Generic/
    │   └── TokenTracker.cs
    ├── Utilities/
    │   ├── DialogueUtilities.cs
    │   └── EventParser.cs
    ├── DialogueLineContainer.cs
    ├── ReaderEventController.cs
    └── StoryPlayer.cs
```

The archive also includes a sample Unity scene, Input System action asset, and URP settings. The project archive is not a complete standalone application: several referenced event, channel, choice, and presentation types are omitted from this NDA-sanitized snapshot.

## Ink tags and dialogue conventions

The included `EventParser` recognizes these tag names (case-insensitive):

| Tag | Parsed event |
|---|---|
| `bg` | `ChangeBackgroundDialogueEvent` |
| `show` | `ShowCharacterVisualDialogueEvent` |
| `hide` | `CharacterHideDialogueEvent` |
| `position` | `ChangeSpeakerPositionDialogueEvent` |

Tags may be written as a bare tag or as a name followed by colon-separated values, for example:

```ink
=== opening ===
# bg: backgrounds/room, 0.5, true
Mira: Welcome home.
```

The exact parameter format for `show`, `hide`, and `position` depends on their event classes, which are not included in the supplied archive. The background event accepts a sprite path, an optional fade duration, and an optional boolean controlling fade through black.

The parser also applies default character presentation behavior:
- If a line has a speaker but no explicit position event, it creates an automatic speaker-position event.
- If no character visual event is present, it creates a show event for a named speaker, or a hide event for a line without a speaker.

Dialogue lines are parsed using a simple `Speaker: dialogue` convention. A comma in the speaker segment separates the speaker key from inline event text. Text enclosed in parentheses is marked as thoughts and has the surrounding parentheses removed.

## Runtime dependencies

The visible source uses:
- Unity and Unity UI (`MonoBehaviour`, `Button`, `Canvas`);
- Ink runtime (`Ink.Runtime.Story`);
- C# tasks and collections;
- additional project-specific event channels and event types that are not all present in this archive.

The source archive does not include an Ink story file, the complete StreamingAssets loading implementation, or the full set of presentation/UI scripts. Therefore, the documentation describes the reader core and its integration contracts rather than a ready-to-run project setup.

## NDA and scope note

This repository is intentionally incomplete. Types and systems may be missing or simplified to avoid exposing proprietary implementation details. In particular, the original project's custom code execution, complex event processing, telemetry, and some asset-loading/presentation systems are outside the visible scope of this sample.

Do not interpret an omitted class or feature as evidence that it did not exist in the original project.

## Documentation

- [Architecture and reading flow](docs/ARCHITECTURE.md)
- [Writing Ink content and tags](docs/INK_AUTHORING.md)
- [Dialogue parsing and events](docs/DIALOGUE_AND_EVENTS.md)
- [Event channels and asynchronous coordination](docs/EVENT_CHANNELS.md)
- [API reference](docs/API_REFERENCE.md)

## License

No license information was present in the supplied archive. Add a `LICENSE` file if this project is intended for distribution.
