using com.absence.utilities;
using com.game.player;
using UnityEngine;

namespace com.game.ui
{
    public class OrbContainerUI : Singleton<OrbContainerUI>
    {
        [SerializeField] private GameObject m_panel;
        [SerializeField] private RectTransform m_pivot;
        //[SerializeField] private OrbDisplay m_displayPrefab;

        PlayerOrbContainer m_container;

        private void Start()
        {
            m_container = Player.Instance.Hub.OrbContainer;
        }

        public void SetVisibility(bool visibility)
        {

        }
    }
}
