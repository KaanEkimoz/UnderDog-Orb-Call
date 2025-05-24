using com.absence.attributes;
using com.game.interactionsystem;
using com.game.player;
using System;
using System.Text;
using UnityEngine;

namespace com.game.generics
{
    public class DropOnInteract : MonoBehaviour, IInteractable
    {
        [SerializeField] private Cost m_cost;
        [Space, SerializeField] private bool m_precalculateDropAmounts = true;
        [SerializeField] private Drop m_drop;

        public string RichInteractionMessage
        {
            get
            {
                StringBuilder sb = new();

                sb.Append(m_cost.GenerateDescription(true));
                sb.Append("\n");
                sb.Append(m_drop.GenerateDescription(true));

                return sb.ToString();
            }
        }
        public string RichInteractionCallbackPopup
        {
            get
            {
                return "Interacted.";
            }
        }

        public bool Interactable => m_cost.CanAfford(Player.Instance);

        public event Action<IInteractable, IInteractor, bool> OnInteraction;
        public event Action<bool> OnSeenByPlayer;
        public event Action<bool> OnPickedByPlayer;

        private void Awake()
        {
            m_drop.DiscardCalculations();

            if (m_precalculateDropAmounts)
                m_drop.CalculateAmounts();
        }

        public bool Interact(IInteractor interactor)
        {
            if (!Interactable)
            {
                OnInteraction?.Invoke(this, interactor, false);
                return false;
            }

            m_cost.Perform(Player.Instance);
            m_drop.Perform(transform);
            Destroy(gameObject);

            OnInteraction?.Invoke(this, interactor, true);
            return true;
        }

        public void CommitPickedByPlayer(bool picked)
        {
            OnPickedByPlayer?.Invoke(picked);
        }

        public void CommitSeenByPlayer(bool seen)
        {
            Debug.Log("fj");
            OnSeenByPlayer?.Invoke(seen);
        }

        [Button("Generate Description")]
        void PrintDescription()
        {
            Debug.Log(RichInteractionMessage);
        }
    }
}
