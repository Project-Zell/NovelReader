# Dialogue Parsing and Events

## DialogueLineContainer

`DialogueLineContainer` is a readonly struct with four fields:

| Field | Type | Meaning |
|---|---|---|
| `NameKey` | `string` | Speaker identifier. |
| `DialogueKey` | `string` | Parsed dialogue/narration text. |
| `InLineEvents` | `string` | Inline data following a comma in the speaker segment. |
| `IsThoughts` | `bool` | True when the dialogue was enclosed in outer parentheses. |

Its `Merge(fallbackLine)` method uses the current line's non-empty string fields, otherwise the fallback line's values. The thought flag is true if either line has it set.

## DialogueUtilities.ParseToDialogue

Parsing rules:

1. Find the first `:` in the raw line.
2. If none exists, return the trimmed line as dialogue with no speaker.
3. Treat the text before the colon as the speaker segment.
4. If the speaker segment contains a comma, split at the first comma: left side is `NameKey`, right side is `InLineEvents`.
5. Treat the text after the colon as dialogue.
6. If the dialogue is surrounded by parentheses, remove them and set `IsThoughts`.

The parser is intentionally lightweight. It does not support escaped delimiters or a richer dialogue grammar.

## Dialogue event contract

`IDialogueEvent` is a marker interface. Concrete event classes carry data describing a requested presentation change. `ChangeBackgroundDialogueEvent` is included as an example.

### ChangeBackgroundDialogueEvent

Properties:
- `BackgroundSpritePath`
- `FadeDurationInSeconds`
- `DoFadeInBlack`

The constructor accepts a comma-separated string. Missing or invalid optional values fall back to `0` and `false`.

## EventParser

`EventParser` maintains a case-insensitive registry of tag names to event factories:

- `bg` → `ChangeBackgroundDialogueEvent`
- `show` → `ShowCharacterVisualDialogueEvent`
- `hide` → `CharacterHideDialogueEvent`
- `position` → `ChangeSpeakerPositionDialogueEvent`

For each tag, it splits on the first colon. The text before the colon is the tag name; the remainder is the value string. It then invokes the matching factory.

After explicit tag events are parsed, the parser applies defaults:
- If the dialogue has a speaker and no position event exists, add an automatic position event for that speaker.
- If no character visual event exists, add a show event for the speaker (including inline event data), or add a hide event for a line without a speaker.
- If a position event exists, associate it with the current dialogue speaker using `WithCharacter`.

Some of these event types are referenced but omitted from the supplied source archive. Their exact properties and parameter formats are therefore outside this reference.

## Extending the tag registry

To support a new tag in this sample architecture, add a factory entry to `TagsRegistry` that converts the raw tag value string into a class implementing `IDialogueEvent`. Add a subscriber to `DialogueEventChannel` or another relevant event channel to handle that event.

In the original project, the event system was more extensive. This extension outline describes only the visible, simplified registry pattern.
