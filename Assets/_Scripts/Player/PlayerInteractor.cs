using com.game.interactionsystem;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    public class PlayerInteractor : MonoBehaviour, IInteractor
    {
        [SerializeField] private PlayerInputHandler m_inputHandler;

        List<IInteractable> m_seenInteractables = new();

        int m_pickedIndex = 0;

        public bool IsPlayer => true;

        private void Awake()
        {
            RefreshPickedIndex();
        }

        private void Update()
        {
            if (m_inputHandler.InteractButtonPressed)
                Interact();
        }

        public void Interact()
        {
            if (m_pickedIndex == -1)
                return;

            RefreshPickedIndex();

            GetAtOrNull(m_pickedIndex)?.Interact(this);
        }

        public void PickNext()
        {
            Pick(m_pickedIndex + 1);
        }

        public void PickPrevious()
        {
            Pick(m_pickedIndex - 1);
        }

        public void Pick(int index)
        {
            if (m_pickedIndex == -1)
                return;

            GetAtOrNull(m_pickedIndex)?.CommitPickedByPlayer(false);

            m_pickedIndex = index;

            RefreshPickedIndex();

            GetAtOrNull(m_pickedIndex)?.CommitPickedByPlayer(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable))
                return;

            if (!m_seenInteractables.Contains(interactable))
                m_seenInteractables.Add(interactable);

            RefreshPickedIndex(false);

            interactable.CommitSeenByPlayer(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable))
                return;

            if (m_seenInteractables.Contains(interactable))
                m_seenInteractables.Remove(interactable);

            RefreshPickedIndex(false);

            interactable.CommitSeenByPlayer(false);
        }

        void RefreshPickedIndex(bool wrap = true)
        {
            int count = m_seenInteractables.Count;

            if (count == 0)
            {
                m_pickedIndex = -1;
                return;
            }

            if (wrap)
            {
                if (m_pickedIndex < 0f) m_pickedIndex += count;
                else if (m_pickedIndex >= count) m_pickedIndex -= count;

                return;
            }

            if (m_pickedIndex < 0f) m_pickedIndex = 0;
            else if (m_pickedIndex >= count) m_pickedIndex = count - 1;
        }

        IInteractable GetAtOrNull(int index)
        {
            if (index < 0) return null;
            if (index >= m_seenInteractables.Count) return null;

            return m_seenInteractables[index];
        }
    }
}
