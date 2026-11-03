public readonly struct DialogueLineContainer
{
    public readonly string NameKey;
    public readonly string DialogueKey;
    public readonly string InLineEvents;

    public readonly bool IsThoughts;

    public DialogueLineContainer(string nameKey = default, string dialogueKey = default, string inLineEvents = default, bool isThoughts = default)
    {
        NameKey = nameKey;
        DialogueKey = dialogueKey;
        InLineEvents = inLineEvents;

        IsThoughts = isThoughts;
    }

    public DialogueLineContainer Merge(DialogueLineContainer fallbackLine)
    {
        return new(
            nameKey: string.IsNullOrWhiteSpace(NameKey) ? fallbackLine.NameKey : NameKey,
            dialogueKey: string.IsNullOrWhiteSpace(DialogueKey) ? fallbackLine.DialogueKey : DialogueKey,
            inLineEvents: string.IsNullOrWhiteSpace(InLineEvents) ? fallbackLine.InLineEvents : InLineEvents,
            isThoughts: fallbackLine.IsThoughts || IsThoughts);
    }
}
