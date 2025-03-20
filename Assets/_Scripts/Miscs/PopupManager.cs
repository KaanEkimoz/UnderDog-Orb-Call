using com.absence.utilities;
using UnityEngine;

namespace com.game.miscs
{
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField] private PopupBehaviour m_damagePopupPrefab;

        public PopupBehaviour CreateDamagePopup(float damage, Vector3 hitPoint, bool displayPercentages = false)
        {
            PopupBehaviour popup = Create(m_damagePopupPrefab, hitPoint, Vector3.zero);
            popup.AutoStartFadeOut = true;
            popup.MoveUpDuringFadeOut = true;
            popup.DestroyAfterFadeOut = true;

            string mask = displayPercentages ? "" : "0";
            popup.SetText(damage.ToString(mask));

            return popup;
        }

        T Create<T>(T prefab, Vector3 position, Vector3 eulerAngles) where T : PopupBehaviour
        {
            return Instantiate(prefab, position, Quaternion.Euler(eulerAngles));
        }
    }
}
