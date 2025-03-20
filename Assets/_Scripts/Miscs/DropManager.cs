using com.absence.utilities;
using UnityEngine;

namespace com.game.miscs
{
    public class DropManager : Singleton<DropManager>
    {
        [SerializeField] private DropBehaviour m_moneyDropBehaviour;
        [SerializeField] private DropBehaviour m_experienceDropBehaviour;

        public DropBehaviour SpawnMoneyDrop(int amount, Vector3 position)
        {
            DropBehaviour drop = Create(m_moneyDropBehaviour, position, Vector3.zero);
            drop.Amount = amount;
            return drop;
        }

        public DropBehaviour SpawnExperienceDrop(int amount, Vector3 position)
        {
            DropBehaviour drop = Create(m_experienceDropBehaviour, position, Vector3.zero);
            drop.Amount = amount;
            return drop;
        }

        T Create<T>(T prefab, Vector3 position, Vector3 eulerAngles) where T : DropBehaviour
        {
            return Instantiate(prefab, position, Quaternion.Euler(eulerAngles));
        }
    }
}
