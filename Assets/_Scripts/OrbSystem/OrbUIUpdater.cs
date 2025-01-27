using com.game.player;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.game.orbsystem.ui
{
    public class OrbUIUpdater : MonoBehaviour
    {
        [SerializeField] private float m_diameter;
        [SerializeField] [Range(0.1f, 1f)] private float m_scalingFactor;
        [SerializeField] [Min(0f)] private float m_rotatingDuration;
        [SerializeField] private OrbDisplayGP m_prefab;

        List<OrbDisplayGP> m_orbDisplays;
        int m_orbCount;
        float m_stepAngle;
        float m_realScalingFactor;
        int m_selectedOrbIndex;
        Sequence m_rotatingSequence;

        private void Start()
        {
            m_orbCount = Player.Instance.CharacterProfile.OrbCount;
            m_stepAngle = 360f / m_orbCount;
            m_realScalingFactor = m_scalingFactor / m_orbCount;

            m_selectedOrbIndex = 0;

            m_orbDisplays = new();
            for (int i = 0; i < m_orbCount; i++)
            {
                float totalAngle = i * m_stepAngle;
                float cos = Mathf.Cos(totalAngle * Mathf.Deg2Rad);
                float sin = Mathf.Sin(totalAngle * Mathf.Deg2Rad);
                Vector2 direction = new Vector2(sin, cos);
                Vector2 position = direction * m_diameter;

                OrbDisplayGP orbDisplay = Instantiate(m_prefab);
                orbDisplay.transform.SetParent(transform, false);
                orbDisplay.transform.localPosition = position;

                orbDisplay.SetSelected(false);
                orbDisplay.SetThrown(false);

                m_orbDisplays.Add(orbDisplay);
            }

            m_orbDisplays[m_selectedOrbIndex].SetSelected(true);

            UpdateArrangement();
        }
        private void Update()
        {
            if (Game.Paused)
                return;

            if (PlayerInputHandler.Instance.PreviousChooseButtonPressed)
                SelectPreviousOrb();
            else if (PlayerInputHandler.Instance.NextChooseButtonPressed)
                SelectNextOrb();
            else if (PlayerInputHandler.Instance.AttackButtonReleased)
                Throw();
            else if (PlayerInputHandler.Instance.RecallButtonPressed)
                Recall();
        }

        private void Throw()
        {
            OrbDisplayGP orbDisplay = m_orbDisplays[m_selectedOrbIndex];

            if (orbDisplay.IsThrown)
                return;

            m_orbDisplays[m_selectedOrbIndex].SetThrown(true);
            //SelectNextOrb();
        }

        private void Recall()
        {
            m_orbDisplays.ForEach(orbDisplay => orbDisplay.SetThrown(false));
        }

        public void SelectNextOrb()
        {
            m_orbDisplays[m_selectedOrbIndex].SetSelected(false);

            m_selectedOrbIndex++;

            if (m_selectedOrbIndex >= m_orbCount) m_selectedOrbIndex -= m_orbCount;
            else if (m_selectedOrbIndex < 0) m_selectedOrbIndex += m_orbCount;

            m_orbDisplays[m_selectedOrbIndex].SetSelected(true);

            UpdateArrangement();
        }

        private void SelectPreviousOrb()
        {
            m_orbDisplays[m_selectedOrbIndex].SetSelected(false);

            m_selectedOrbIndex--;

            if (m_selectedOrbIndex >= m_orbCount) m_selectedOrbIndex -= m_orbCount;
            else if (m_selectedOrbIndex < 0) m_selectedOrbIndex += m_orbCount;

            m_orbDisplays[m_selectedOrbIndex].SetSelected(true);

            UpdateArrangement();
        }

        public void UpdateArrangement()
        {
            float totalAngle = m_stepAngle * m_selectedOrbIndex;

            Vector3 endingRotation = new Vector3(0f, 0f, totalAngle);

            if (m_rotatingSequence != null)
                m_rotatingSequence.Kill();

            m_rotatingSequence = DOTween.Sequence();

            var rotationTween = transform.DORotate(endingRotation, m_rotatingDuration, RotateMode.Fast);
            m_rotatingSequence.Insert(0f, rotationTween);

            //for (int i = 0; i < m_orbCount; i++)
            //{
            //    OrbDisplayGP orbDisplay = m_orbDisplays[i];

            //    int reversedIndex = m_orbCount - 1 - i;

            //    int selectionDiff = Mathf.Abs(m_selectedOrbIndex - i);
            //    int reversedSelectionDiff = Mathf.Abs(m_selectedOrbIndex - reversedIndex);
            //    int minDiff = Mathf.Min(selectionDiff, reversedSelectionDiff);
            //    var scalingTween = orbDisplay.transform.DOScale(1f - (m_realScalingFactor * minDiff), m_rotatingDuration);
            //    m_rotatingSequence.Insert(0f, scalingTween);
            //}

            m_rotatingSequence.SetEase(Ease.InOutSine);
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
