namespace Core.Story.Events
{
    public sealed class ChangeBackgroundDialogueEvent : IDialogueEvent
    {
        public string BackgroundSpritePath { get; }
        public float FadeDurationInSeconds { get; }
        public bool DoFadeInBlack { get; }

        public ChangeBackgroundDialogueEvent(string values)
        {
            var settings = values.Split(',');

            BackgroundSpritePath = settings[0].Trim();

            FadeDurationInSeconds = settings.Length > 1 && float.TryParse(settings[1].Trim(), out var fade) ? fade : 0f;
            DoFadeInBlack = settings.Length > 2 && bool.TryParse(settings[2].Trim(), out var fadeInBlack) && fadeInBlack;
        }
    }
}