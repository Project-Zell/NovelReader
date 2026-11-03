using Core.Story.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ReaderEventController : IDisposable
{
    private readonly StoryEventChannel _storyEventChannel;
    private readonly DialogueEventChannel _dialogueEventChannel;
    private readonly VariablesEventChannel _variablesEventChannel;

    private readonly Queue<Action> _actionQueue = new();

    public ReaderEventController(EventChannelDistributor distributor)
    {
        _storyEventChannel = distributor.Get<StoryEventChannel>();
        _dialogueEventChannel = distributor.Get<DialogueEventChannel>();
        _variablesEventChannel = distributor.Get<VariablesEventChannel>();

        _dialogueEventChannel.Subscribe<ChangeReaderStyleDialogueEvent>(HandleEvent);

        _variablesEventChannel.OnItemRequest.Subscribe(HandleItemRequest);
    }

    public void Dispose()
    {
        _dialogueEventChannel.Unsubscribe<ChangeReaderStyleDialogueEvent>(HandleEvent);

        _variablesEventChannel.OnItemRequest.Unsubscribe(HandleItemRequest);
    }

    private void HandleEvent(ChangeReaderStyleDialogueEvent evt)
    {
        _actionQueue.Enqueue(() => _storyEventChannel.OnReaderStyleChangeRequest.Raise(evt.ReaderStyleName));
    }

    private void HandleItemRequest(ItemInstance item)
    {
        _actionQueue.Enqueue(() => _variablesEventChannel.OnItemObtained.Raise(item));
    }

    public async Task ResolveEventsAsync()
    {
        while (_actionQueue.TryDequeue(out var action))
        {
            action?.Invoke();
            await _dialogueEventChannel.WaitRequestTraker.WaitUntilReleasedAsync();
        }
    }
}