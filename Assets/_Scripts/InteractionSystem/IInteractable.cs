using com.game.utilities;
using System;

namespace com.game.interactionsystem
{
    public interface IInteractable : IObject
    {
        string RichInteractionMessage { get; }
        string RichInteractionCallbackPopup { get; }
        bool Interactable { get; }

        event Action<IInteractable, IInteractor, bool> OnInteraction;
        event Action<bool> OnSeenByPlayer;
        event Action<bool> OnPickedByPlayer;
                           
        bool Interact(IInteractor interactor);

        void CommitSeenByPlayer(bool seen);
        void CommitPickedByPlayer(bool picked);
    }
}
