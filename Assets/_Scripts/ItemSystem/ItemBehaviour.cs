using System;
using UnityEngine;

namespace com.game.itemsystem
{
    /// <summary>
    /// The abstract class to derive from if an item holds any 
    /// <see cref="ItemActionType.SpawnItemBehaviour"/> custom actions.
    /// </summary>
    public abstract class ItemBehaviour : MonoBehaviour
    {
        [SerializeField] private ItemObject m_instance;

        public void Initialize(ItemObject instance)
        {
            m_instance = instance;

            m_instance.OnDispose += Dispose;

            OnDispatch();
        }

        protected void Dispose()
        {
            Destroy(gameObject);
            OnDispose();
        }

        private void OnDestroy()
        {
            m_instance.OnDispose -= Dispose;
        }

        public virtual void OnDispatch()
        {
        }

        public virtual void OnDispose()
        {
        }
    }
}
