using com.game.player;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.orbsystem.ui
{
    public class OrbUIUpdater : MonoBehaviour
    {
        [SerializeField] private float m_diameter;
        [SerializeField] private Ease m_rotationEase;
        [SerializeField] [Range(0.1f, 1f)] private float m_scalingFactor;
        [SerializeField] [Min(0f)] private float m_rotatingDuration;
        [SerializeField] private Transform m_orbSelectionBorder;
        [SerializeField] private Transform m_pivot;
        [SerializeField] private OrbController m_orbController;
        [SerializeField] private OrbDisplayGP m_prefab;

        List<OrbDisplayGP> m_orbDisplays;
        int m_orbCount;
        float m_stepAngle;
        float m_realScalingFactor;
        int m_selectedOrbIndex;
        bool m_firstCreation = true;
        Sequence m_rotatingSequence;

        private void Start()
        {
            SubscribeToEvents();
            FetchVariables();
        }

        private void Update()
        {
            //if (m_selectedOrbIndex != m_orbController.orb)
        }

        void SubscribeToEvents()
        {
            m_orbController.OnOrbCountChanged += OnOrbCountChanged;
            m_orbController.OnNextOrbSelected += SelectNextOrb;
            m_orbController.OnPreviousOrbSelected += SelectPreviousOrb;
        }

        private void OnOrbCountChanged(int newCount)
        {
            m_orbCount = newCount;
            CreateOrbDisplays();
        }

        void FetchVariables()
        {
            m_orbCount = Player.Instance.CharacterProfile.OrbCount;
            m_stepAngle = 360f / m_orbCount;
            m_realScalingFactor = m_scalingFactor / m_orbCount;
        }

        void CreateOrbDisplays()
        {
            if (m_orbDisplays == null) m_orbDisplays = new();
            else
            {
                for (int i = 0; i < m_orbDisplays.Count; i++)
                {
                    Destroy(m_orbDisplays[i]);
                }

                m_orbDisplays.Clear();
            }

            for (int i = 0; i < m_orbCount; i++)
            {
                float totalAngle = i * m_stepAngle;
                float cos = Mathf.Cos(totalAngle * Mathf.Deg2Rad);
                float sin = Mathf.Sin(totalAngle * Mathf.Deg2Rad);
                Vector2 direction = new Vector2(sin, cos);
                Vector2 position = direction * m_diameter;

                OrbDisplayGP orbDisplay = Instantiate(m_prefab);
                orbDisplay.transform.SetParent(m_pivot, false);
                orbDisplay.transform.localPosition = position;

                orbDisplay.Initialize(m_orbController.OrbsOnEllipse[i]);

                m_orbDisplays.Add(orbDisplay);

                if (i == 0) m_orbSelectionBorder.localPosition = position;
            }

            m_orbSelectionBorder.SetAsFirstSibling();

            if (m_firstCreation)
            {
                m_selectedOrbIndex = -1;
                SelectNextOrb();

                m_firstCreation = false;
            }

            UpdateArrangement();
        }

        void SelectNextOrb()
        {
            SelectOrb(m_selectedOrbIndex + 1);
        }

        void SelectPreviousOrb()
        {
            SelectOrb(m_selectedOrbIndex - 1);
        }

        void SelectOrb(int index)
        {
            //m_orbDisplays[m_selectedOrbIndex].SetSelected(false);

            m_selectedOrbIndex = index;

            if (m_selectedOrbIndex >= m_orbCount) m_selectedOrbIndex -= m_orbCount;
            else if (m_selectedOrbIndex < 0) m_selectedOrbIndex += m_orbCount;

            //m_orbDisplays[m_selectedOrbIndex].SetSelected(true);

            UpdateArrangement();
        }

        public void UpdateArrangement()
        {
            float totalAngle = m_stepAngle * m_selectedOrbIndex;

            Vector3 endingRotation = new Vector3(0f, 0f, totalAngle);

            if (m_rotatingSequence != null)
                m_rotatingSequence.Kill();

            m_rotatingSequence = DOTween.Sequence();

            var rotationTween = m_pivot.DORotate(endingRotation, m_rotatingDuration, RotateMode.Fast);
            m_rotatingSequence.Insert(0f, rotationTween);

            //for (int i = 0; i < m_orbCount; i++)
            //{
            //    OrbDisplayGP orbDisplay = m_orbDisplays[i];

            //    int reversedIndex = m_orbCount - i;

            //    int selectionDiff = Mathf.Abs(m_selectedOrbIndex - i);
            //    int reversedSelectionDiff = Mathf.Abs(m_selectedOrbIndex - reversedIndex);
            //    int minDiff = Mathf.Min(selectionDiff, reversedSelectionDiff);
            //    var scalingTween = orbDisplay.transform.DOScale(1f - (m_realScalingFactor * minDiff), m_rotatingDuration);
            //    m_rotatingSequence.Insert(0f, scalingTween);
            //}

            m_rotatingSequence.SetEase(m_rotationEase);
            m_rotatingSequence.OnComplete(OnRotationEnds);
            m_rotatingSequence.OnKill(OnRotationEnds);
            m_orbDisplays.ForEach(display => display.SetRotating(true));
        }

        private void OnRotationEnds()
        {
            m_orbDisplays.ForEach(display => display.SetRotating(false));
        }
    }
}
