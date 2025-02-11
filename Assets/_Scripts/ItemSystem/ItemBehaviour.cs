using UnityEngine;
using com.game.itemsystem.gamedependent;

namespace com.game.itemsystem
{
    /// <summary>
    /// The abstract class to derive from if an item holds any 
    /// <see cref="ItemActionType.SpawnItemBehaviour"/> custom actions.
    /// </summary>
    public abstract class ItemBehaviour : MonoBehaviour
    {
        public static readonly string CustomDataKey = "behaviour";

        protected ItemObject m_instance = null;

        public void Initialize(ItemObject instance)
        {
            m_instance = instance;
            m_instance.CustomData.Add(ItemBehaviour.CustomDataKey, this);

            m_instance.OnDispose += Dispose;

            OnSpawn();
        }

        public abstract string GenerateActionDescription(bool richText);
        public virtual string GenerateDataDescription(bool richText)
        {
            return null;
        }

        public abstract void OnSpawn();
        public abstract void OnDespawn();

        protected void Dispose()
        {
            Destroy(gameObject);
            OnDespawn();
        }

        private void OnDestroy()
        {
            m_instance.OnDispose -= Dispose;
        }
    }
}
