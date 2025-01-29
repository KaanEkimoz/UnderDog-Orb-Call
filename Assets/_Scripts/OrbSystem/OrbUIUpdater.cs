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
        Queue<OrbDisplayGP> m_orbDisplaysWaitingForAddition;
        int m_orbCount;
        float m_stepAngle;
        float m_realScalingFactor;
        int m_selectedOrbIndex;
        Sequence m_rotatingSequence;

        private void Start()
        {
            m_orbDisplaysWaitingForAddition = new();

            SubscribeToEvents();
            FetchVariables();
            CreateOrbDisplays();

            m_selectedOrbIndex = -1;
            SelectNextOrb();

            UpdateArrangement();
        }

        void SubscribeToEvents()
        {
            m_orbController.OnOrbThrowed += OnThrow;
            m_orbController.OnOrbCalled += OnRecall;
            m_orbController.OnAllOrbsCalled += OnAllRecall;
            m_orbController.OnOrbAdded += OnAdd;
            m_orbController.OnNextOrbSelected += SelectNextOrb;
            m_orbController.OnPreviousOrbSelected += SelectPreviousOrb;
        }

        private void OnAdd()
        {
            if (m_orbDisplaysWaitingForAddition.TryDequeue(out OrbDisplayGP target))
            {
                target.SetState(OrbDisplayGP.DisplayState.Ready);
            }
        }

        private void OnAllRecall()
        {
            for (int i = 0; i < m_orbCount; ++i) 
            {
                OrbDisplayGP target = m_orbDisplays[i];
                target.SetState(OrbDisplayGP.DisplayState.Recalling);
                m_orbDisplaysWaitingForAddition.Enqueue(target);
            }
        }

        private void OnRecall()
        {
            OrbDisplayGP target = m_orbDisplays[m_selectedOrbIndex];

            if (m_orbDisplaysWaitingForAddition.Contains(target))
                return;

            m_orbDisplaysWaitingForAddition.Enqueue(target);
        }

        private void OnThrow()
        {
            OrbDisplayGP target = m_orbDisplays[m_selectedOrbIndex];

            target.SetState(OrbDisplayGP.DisplayState.Thrown);
        }

        void FetchVariables()
        {
            m_orbCount = Player.Instance.CharacterProfile.OrbCount;
            m_stepAngle = 360f / m_orbCount;
            m_realScalingFactor = m_scalingFactor / m_orbCount;
        }

        void CreateOrbDisplays()
        {
            m_orbDisplays = new();
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

                //orbDisplay.SetSelected(false);
                orbDisplay.SetState(OrbDisplayGP.DisplayState.Ready);

                m_orbDisplays.Add(orbDisplay);

                if (i == 0) m_orbSelectionBorder.localPosition = position;
            }

            m_orbSelectionBorder.SetAsFirstSibling();
        }

        void SelectNextOrb()
        {
            //if (m_selectedOrbIndex != -1) m_orbDisplays[m_selectedOrbIndex].SetSelected(false);

            m_selectedOrbIndex++;

            if (m_selectedOrbIndex >= m_orbCount) m_selectedOrbIndex -= m_orbCount;
            else if (m_selectedOrbIndex < 0) m_selectedOrbIndex += m_orbCount;

            //m_orbDisplays[m_selectedOrbIndex].SetSelected(true);

            UpdateArrangement();
        }

        void SelectPreviousOrb()
        {
            //m_orbDisplays[m_selectedOrbIndex].SetSelected(false);

            m_selectedOrbIndex--;

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
