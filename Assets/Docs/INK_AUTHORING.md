# Writing Ink Content

## Role of Ink

Ink (`.ink`) is the primary source for narrative text and branching flow. The reader runtime consumes the story through `Ink.Runtime.Story`; it does not require writers to manually create a Unity object for every dialogue line.

The reader expects dialogue output to follow a simple speaker convention when a speaker is present:

```ink
Mira: Welcome home.
```

A line without a speaker is treated as narration:

```ink
The door creaks open.
```

The parser uses the first colon to separate speaker from dialogue. If your dialogue text itself contains a colon, be aware that the first colon is treated as the speaker separator.

## Inline speaker data

The reader supports a comma in the speaker segment. The part before the comma is treated as the speaker key, and the part after it becomes `InLineEvents`.

Illustrative form:

```ink
Mira, expression=smile: Welcome home.
```

The included code does not define the full inline-event grammar; the value is passed through to the character visual event as a string. The actual supported options depend on presentation code omitted from this archive.

## Thoughts

If the dialogue text begins with `(` and ends with `)`, the parser treats it as a thought line, removes the outer parentheses, and sets `IsThoughts` to `true`.

```ink
Mira: (I hope nobody noticed.)
```

The result contains the text `I hope nobody noticed.` with `IsThoughts = true`.

The parser only checks whether the complete trimmed dialogue string starts and ends with parentheses. It does not parse nested parentheses or partial parenthetical phrases.

## Tags

Ink tags are used to attach presentation/event instructions to story content. The included parser recognizes these names, without regard to capitalization:

| Tag | Purpose in the visible implementation |
|---|---|
| `bg` | Request a background change. |
| `show` | Request a character visual change/show. |
| `hide` | Request a character hide. |
| `position` | Request a speaker-position change. |

Example background tag:

```ink
# bg: backgrounds/room, 0.5, true
```

The visible `ChangeBackgroundDialogueEvent` interprets the values as:
1. background sprite path;
2. optional fade duration in seconds (defaults to `0`);
3. optional `bool` for fading through black (defaults to `false`).

The parser accepts tags in either bare form or `tagName: values` form. Unknown tags are ignored by the included registry.

The exact argument syntax for `show`, `hide`, and `position` cannot be specified from the supplied source because their event classes are not included.

## Choices and branching

Ink owns branching and choice logic. The reader checks `Story.currentChoices`, publishes parsed choice data when choices are available, and waits for the player to select an index. It then calls `ChooseChoiceIndex(index)` on the Ink story.

The choice UI and the helper that converts Ink choices into presentation data are not included in this archive. The reader's integration point is the story channel event that publishes the parsed choices.

## Authoring advice

- Keep speaker identifiers consistent with the character keys expected by the Unity presentation layer.
- Use tags for presentation changes rather than embedding presentation-specific instructions in dialogue text.
- Treat tag parameter formats as an agreed contract between writers and the code that parses those tags.
- Test every branch in the Ink story in the Unity reader, especially branches that trigger multiple tags or choices.
- Avoid relying on unrecognized tags; unknown tag names are ignored by this sample parser.
