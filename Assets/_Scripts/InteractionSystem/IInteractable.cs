using com.game.utilities;

namespace com.game.interactionsystem
{
    public interface IInteractable : IObject
    {
        bool Interactable { get; }

        bool Interact(IInteractor interactor);

        void OnSeenByPlayer(bool seen);
        void OnPickedByPlayer(bool picked);
    }
}
