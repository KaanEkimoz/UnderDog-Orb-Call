using UnityEngine;

namespace com.game.player
{
    public class PlayerMoneyLogic : MonoBehaviour
    {
        [SerializeField] private int m_money;
        public int Money { get { return m_money; } set { m_money = value; } }

        public bool CanAfford(int amount)
        {
            return m_money >= amount;
        }

        public void Spend(int amount)
        {
            int newMoney = m_money - amount;
            m_money = Mathf.Max(0, newMoney);
        }
    }
}
