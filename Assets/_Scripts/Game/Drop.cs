using com.absence.attributes;
using com.game.miscs;
using System.Text;
using UnityEngine;

namespace com.game
{
    [System.Serializable]
    public class Drop
    {
        [MinMaxSlider(0f, Constants.Drops.MAX_DROP_AMOUNT)] public Vector2Int MoneyAmount = Vector2Int.zero;
        [MinMaxSlider(0f, Constants.Drops.MAX_DROP_AMOUNT)] public Vector2Int ExperienceAmount = Vector2Int.zero;

        int m_calculatedMoneyAmount;
        int m_calculatedExperienceAmount;
        bool m_calculated = false;

        public void DiscardCalculations()
        {
            m_calculated = false;
        }

        public void CalculateAmounts()
        {
            m_calculated = true;

            m_calculatedMoneyAmount = GetRandomAmount(MoneyAmount);
            m_calculatedExperienceAmount = GetRandomAmount(ExperienceAmount);
        }

        public bool IsEmpty()
        {
            if (MoneyAmount == Vector2Int.zero && ExperienceAmount == Vector2Int.zero)
                return true;

            return false;
        }

        public void Perform(Transform sender)
        {
            if (DropManager.Instance == null)
            {
                Debug.LogError("There is no DropManager in the scene!");
                return;
            }

            int money = m_calculated ? m_calculatedMoneyAmount : GetRandomAmount(MoneyAmount);
            int exp = m_calculated ? m_calculatedExperienceAmount : GetRandomAmount(ExperienceAmount);

            DropManager.Instance.SpawnIndividualMoneyDrops(money, sender.position);
            DropManager.Instance.SpawnIndividualExperienceDrops(exp, sender.position);
        }

        public string GenerateDescription(bool richText = false)
        {
            bool noMoneyDrop = MoneyAmount == Vector2Int.zero;
            bool noExperienceDrop = ExperienceAmount == Vector2Int.zero;

            if (noMoneyDrop && noExperienceDrop)
                return "Drops nothing.";

            StringBuilder sb = new StringBuilder("Drops:\n");

            if (!noMoneyDrop)
            {
                string moneyAmountText =
                    m_calculated ? m_calculatedMoneyAmount.ToString() : GetAmountText(MoneyAmount);

                if (noExperienceDrop)
                    return $"Drops {moneyAmountText}$.";

                sb.Append($"\t{moneyAmountText}$");
                sb.Append("\n");
            }

            if (!noExperienceDrop)
            {
                string expAmountText =
                    m_calculated ? m_calculatedExperienceAmount.ToString() : GetAmountText(ExperienceAmount);

                if (noMoneyDrop)
                    return $"Drops {expAmountText} Exp.";

                sb.Append($"\t{expAmountText} Exp");
            }

            return sb.ToString();
        }

        string GetAmountText(Vector2Int targetMinMax)
        {
            return targetMinMax.x == targetMinMax.y ? targetMinMax.x.ToString() : "??";
        }

        int GetRandomAmount(Vector2Int targetMinMax)
        {
            if (targetMinMax == Vector2Int.zero)
                return 0;

            return UnityEngine.Random.Range(targetMinMax.x, targetMinMax.y);
        }
    }
}
