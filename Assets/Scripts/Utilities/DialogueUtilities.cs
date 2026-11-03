public static class DialogueUtilities
{
    public static DialogueLineContainer ParseToDialogue(string raw)
    {
        int colonIndex = raw.IndexOf(':');

        if (colonIndex == -1)
        {
            return new(string.Empty, raw.Trim(), string.Empty, false);
        }

        var speakerName = raw[..colonIndex].Trim();
        int commaIndex = speakerName.IndexOf(',');

        var inLineEvents = string.Empty;

        if (commaIndex is not -1)
        {
            inLineEvents = speakerName[(commaIndex + 1)..].Trim();
            speakerName = speakerName[..commaIndex].Trim();
        }

        var dialogueText = raw[(colonIndex + 1)..].Trim();

        var isThougts = Check(ref dialogueText);
        return new(speakerName, dialogueText, inLineEvents, isThougts);
    }

    private static bool Check(ref string dialogueText)
    {
        if (dialogueText.StartsWith('(') && dialogueText.EndsWith(')'))
        {
            dialogueText = dialogueText[1..^1].Trim();
            return true;
        }

        return false;
    }
}