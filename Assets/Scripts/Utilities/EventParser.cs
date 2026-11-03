using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Story.Events
{
    public static class EventParser
    {
        private static readonly Dictionary<string, Func<string, IDialogueEvent>> TagsRegistry = new(StringComparer.OrdinalIgnoreCase)
        {
            ["bg"] = values => new ChangeBackgroundDialogueEvent(values),
            ["show"] = values => new ShowCharacterVisualDialogueEvent(values),
            ["hide"] = values => new CharacterHideDialogueEvent(values),
            ["position"] = values => new ChangeSpeakerPositionDialogueEvent(values)
        };

        public static IDialogueEvent[] GetTagEvents(DialogueLineContainer dialogueLine, List<string> tags)
        {
            var events = new List<IDialogueEvent>();

            foreach (var tag in tags)
            {
                var colonIndex = tag.IndexOf(':');

                if (colonIndex == -1)
                {
                    TryAddEvent(events, tag);
                    continue;
                }

                var tagName = tag[..colonIndex].Trim();
                var values = tag[(colonIndex + 1)..].Trim();

                TryAddEvent(events, tagName, values);
            }

            var hasSpeaker = string.IsNullOrEmpty(dialogueLine.NameKey) is false;
            var positionEvent = events.OfType<ChangeSpeakerPositionDialogueEvent>().FirstOrDefault();

            if (positionEvent is null)
            {
                if (hasSpeaker)
                {
                    events.Add(new ChangeSpeakerPositionDialogueEvent(dialogueLine.NameKey, CharacterPositionType.Auto));
                }
            }
            else
            {
                positionEvent.WithCharacter(dialogueLine.NameKey);
            }

            if (events.Any(x => x is ICharacterVisualDialogueEvent) is false)
            {
                if (hasSpeaker)
                {
                    events.Add(new ShowCharacterVisualDialogueEvent(dialogueLine.NameKey, dialogueLine.InLineEvents));
                }
                else
                {
                    events.Add(new CharacterHideDialogueEvent(string.Empty));
                }
            }

            return events.ToArray();
        }

        private static void TryAddEvent(List<IDialogueEvent> events, string tagName, string values = "")
        {
            if (TagsRegistry.TryGetValue(tagName, out var registry))
                events.Add(registry(values));
        }

        public static int ParseInt(string[] settings, int elementIndex, int defaultValue = 0)
        {
            return settings.Length > elementIndex && int.TryParse(settings[elementIndex].Trim(), out var value) ? value : defaultValue;
        }

        public static float ParseFloat(string[] settings, int elementIndex, float defaultValue = 0f)
        {
            return settings.Length > elementIndex && float.TryParse(settings[elementIndex].Trim(), out var value) ? value : defaultValue;
        }

        public static bool ParseBool(string[] settings, int elementIndex, bool defaultValue = false)
        {
            return settings.Length > elementIndex && bool.TryParse(settings[elementIndex].Trim(), out var value) ? value : defaultValue;
        }
    }
}