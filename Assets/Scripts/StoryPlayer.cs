using Core.Story.Events;
using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Story = Ink.Runtime.Story;

public class StoryPlayer : MonoBehaviour
{
    public int CurrentLineIndex { get; private set; } = 0;

    public Action<DialogueLineContainer> OnLineProceed;
    [SerializeField] private StoryEventChannel _storyChannel;
    [SerializeField] private DialogueEventChannel _dialogueChannel;
    [SerializeField] private VariablesEventChannel _variableEventChannel;
    [SerializeField] private Button _proceedStoryButton;
    [SerializeField] private Canvas _canvas;

    private bool _waitForPlayerInput;

    Story _story;

    private void Awake() => _canvas.enabled = true;

    private void Start()
    {
        if (CurrentLineIndex == -1)
        {
            ProceedLine();
        }
    }

    private void OnEnable()
    {
        _storyChannel.OnChoiceSelected.Subscribe(OnClickChoice);
        _storyChannel.OnStoryInitialized.Subscribe(InitializeStory);

        _proceedStoryButton.onClick.AddListener(OnSpace);
    }

    private void OnDisable()
    {
        _storyChannel.OnChoiceSelected.Unsubscribe(OnClickChoice);
        _storyChannel.OnStoryInitialized.Unsubscribe(InitializeStory);

        _proceedStoryButton.onClick.RemoveListener(OnSpace);
    }

    private void InitializeStory(Story story)
    {
        _story = story;
        CurrentLineIndex = -1;
    }

    public void OnSpace()
    {
        if (_waitForPlayerInput || _dialogueChannel.PauseRequestTraker.HasTokens)
            return;

        ProceedLine();
    }

    private void ProceedLine()
    {
        if (_story.canContinue)
        {
            var raw = _story.Continue();
            var parsedDialogueLine = DialogueUtilities.ParseToDialogue(raw);

            var tagEvents = EventParser.GetTagEvents(parsedDialogueLine, _story.currentTags);
            CastEvents(tagEvents);

            _storyChannel.OnDialogueContinue.Raise(parsedDialogueLine);

            if (_story.currentChoices.Count > 0)
            {
                _proceedStoryButton.gameObject.SetActive(false);

                var parsedChoices = ChoiceUtilities.ParceToData(_story.currentChoices.ToArray());
                _storyChannel.OnChoicesAppear.Raise(parsedChoices);

                _waitForPlayerInput = true;
            }
        }
        else
        {
            _canvas.enabled = false;
            _storyChannel.OnStoryEnd.Raise();
        }
    }

    private void OnClickChoice(int index)
    {
        _story.ChooseChoiceIndex(index);
        ProceedLine();
        _waitForPlayerInput = false;
        _proceedStoryButton.gameObject.SetActive(true);
    }

    private void CastEvents(IDialogueEvent[] dialogueEvents)
    {
        if (dialogueEvents == null || dialogueEvents.Length == 0)
            return;

        foreach (var dialogueEvent in dialogueEvents)
        {
            _dialogueChannel.RaiseEvent(dialogueEvent);
        }

        _dialogueChannel.OnEventsParseComplete.Raise();
    }
}
