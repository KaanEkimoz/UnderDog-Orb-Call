using com.absence.utilities;
using UnityEngine;

namespace com.game.effects
{
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField] private PopupBehaviour m_damagePopupPrefab;

        public void CreateDamagePopup(float damage, Vector3 hitPoint)
        {
            PopupBehaviour popup = Create(m_damagePopupPrefab, hitPoint, Vector3.zero);
            popup.AutoStartFadeOut = true;
            popup.MoveUpDuringFadeOut = true;
            popup.DestroyAfterFadeOut = true;

            popup.SetText(damage.ToString("0"));
        }

        T Create<T>(T prefab, Vector3 position, Vector3 eulerAngles) where T : PopupBehaviour
        {
            return Instantiate(prefab, position, Quaternion.Euler(eulerAngles));
        }
    }
}
