using System;
using System.Collections.Generic;
using com.absence.attributes;
using com.game.player;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace com.game.orbsystem
{
    [RequireComponent(typeof(SimpleOrb))]
    public class OrbAnimator : MonoBehaviour
    {
        public enum State
        {
            Idle_Sway,
            AllOrbCall_Rotate,
        }

        [SerializeField, Readonly] private SimpleOrb m_target;
        [SerializeField, Readonly] private State m_state = State.Idle_Sway;

        [Header("Sway Animation (Idle)")]
        [SerializeField] private bool m_swayEnabled;
        [SerializeField] private Ease m_swayEase = Ease.Linear;
        [SerializeField] private float m_swayMagnitude;
        [SerializeField, Min(0.001f)] private float m_swaySpeed;
        [SerializeField, MinMaxSlider(0f, 1f)] private Vector2 m_randomSwayDelay;

        [Header("All Orbs Calling Animation (Ellipse Scale)")]
        [SerializeField] private Ease m_ellipseScaleEase = Ease.Linear;
        [SerializeField] private float m_ellipseScaleMultiplier = 1f;
        [SerializeField] private float m_ellipseScaleDuration = 1f;

        public State CurrentState
        {
            get
            {
                return m_state;
            }

            set
            {
                m_state = value;
                OnStateChanged(m_state);
            }
        }

        float m_swayCoefficient;
        Vector3 m_initialEulers;
        Vector3 m_lastSwayDirection = Vector3.up;
        PlayerOrbController m_controller;
        Tween m_ellipseScaleTween;
        Tween m_swayCoefficientTween;

        private void Awake()
        {
            float delay = UnityEngine.Random.Range(m_randomSwayDelay.x, m_randomSwayDelay.y);

            m_swayCoefficientTween = DOVirtual.Float(0f, 1f, 1f / m_swaySpeed, f => m_swayCoefficient = f)
                .SetDelay(delay)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(m_swayEase);
        }

        private void Start()
        {
            m_controller = Player.Instance.Hub.OrbContainer.Controller;
            m_initialEulers = transform.eulerAngles;

            m_target.OnStateChanged += OnOrbStateChanged;
            m_controller.OnAllOrbsCalledWithReturn += OnAllOrbsCalled;
            m_controller.OnAllOrbsReturn += OnAllOrbsReturn;

            if (m_swayEnabled)
                m_target.SubscribeToTargetPositionPostProcessing(OrbTargetPositionPostProcess);
        }

        private Vector3 OrbTargetPositionPostProcess(Vector3 input, OrbState state)
        {
            if (state != OrbState.OnEllipse)
                return input;

            if (m_state == State.Idle_Sway)
                return GetSwayPosition(input);

            return input;
        }

        private void OnStateChanged(State newState)
        {
            //switch (newState)
            //{
            //    case State.Idle_Sway:
            //        m_swayCoefficientTween.Play();
            //        break;
            //    case State.AllOrbCall_Rotate:
            //        m_swayCoefficientTween.Pause();
            //        break;
            //    default:
            //        break;
            //}
        }

        private void OnAllOrbsReturn()
        {
            if (m_state != State.AllOrbCall_Rotate)
                return;

            m_state = State.Idle_Sway;
            ScaleEllipse(1f);
        }

        private void OnAllOrbsCalled(IEnumerable<SimpleOrb> orbsCalled)
        {
            if (m_state != State.Idle_Sway)
                return;

            m_state = State.AllOrbCall_Rotate;
            ScaleEllipse(m_ellipseScaleMultiplier);
        }

        void ScaleEllipse(float targetValue)
        {
            if (m_ellipseScaleTween != null)
                m_ellipseScaleTween.Kill();

            m_ellipseScaleTween =
                DOVirtual.Float(m_controller.EllipseSizeMultiplier, targetValue, m_ellipseScaleDuration,
                f => m_controller.EllipseSizeMultiplier = f)
                .SetEase(m_ellipseScaleEase);
        }

        private void OnOrbStateChanged(OrbState state)
        {
            switch (state)
            {
                case OrbState.OnEllipse:
                    break;
                case OrbState.Sticked:
                    break;
                case OrbState.Throwing:
                    break;
                case OrbState.Returning:
                    break;
                default:
                    break;
            }
        }

        Vector3 GetSwayPosition(Vector3 input)
        {
            if (!m_swayEnabled)
                return input;

            return input + (m_swayCoefficient * m_swayMagnitude * (transform.position - m_controller.EllipseCenterGlobal));
        }

        public Vector3 GetRotatedVector(Vector3 originalEuler, float angle, Vector3 axis)
        {
            return (Quaternion.AngleAxis(angle, axis) * Quaternion.Euler(originalEuler)).eulerAngles;
        }

        private void Reset()
        {
            m_target = GetComponent<SimpleOrb>();
        }
    }
}
