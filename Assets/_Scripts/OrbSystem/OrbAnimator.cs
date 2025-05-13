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
        public enum OnEllipseState
        {
            Idle_Sway,
            AllOrbCall_Rotate,
        }

        [SerializeField, Readonly] private SimpleOrb m_target;
        [SerializeField, Readonly] private OnEllipseState m_state = OnEllipseState.Idle_Sway;

        [Header("Sway Animation (Idle)")]
        [SerializeField] private bool m_swayEnabled;
        [SerializeField] private Ease m_swayEase = Ease.Linear;
        [SerializeField] private float m_swayMagnitude;
        [SerializeField, Min(0.001f)] private float m_swaySpeed;
        [SerializeField, MinMaxSlider(0f, 1f)] private Vector2 m_randomSwayDelay;

        [Header("Move Ping Pong Animation (Unstick)")]
        [SerializeField] private Ease m_unstickEase = Ease.Linear;
        [SerializeField] private float m_unstickMagnitude;
        [SerializeField, Min(0.001f)] private float m_unstickSpeed;

        [Header("Throw Animation (Throw)")]
        [SerializeField] private AnimationCurve m_throwSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private bool m_throwEnabled = true;
        [SerializeField] private float m_throwPivotFactor;
        [SerializeField, MinMaxSlider(-90f, 90f)] private Vector2 m_throwTiltRange;
        [SerializeField, Min(0.001f)] private float m_throwDuration;

        [Header("All Orbs Calling Animation (Ellipse Scale)")]
        [SerializeField] private Ease m_ellipseScaleEase = Ease.Linear;
        [SerializeField] private float m_ellipseScaleMultiplier = 1f;
        [SerializeField] private float m_ellipseScaleDuration = 1f;

        public OnEllipseState CurrentState
        {
            get
            {
                return m_state;
            }

            set
            {
                m_state = value;
            }
        }

        float m_swayCoefficient = 0f;
        float m_unstickYShift = 0f;
        float m_throwTimer;
        float m_throwTilt;
        Vector3 m_initialEulers;
        Vector3 m_initialPositionBeforeThrow;
        Vector3 m_lastSwayDirection = Vector3.up;
        PlayerOrbController m_controller;
        Tween m_ellipseScaleTween;
        Tween m_swayCoefficientTween;
        Tween m_unstickYShiftTween;
        bool m_inThrowAnimation = false;

        private void Awake()
        {
            float delay = UnityEngine.Random.Range(m_randomSwayDelay.x, m_randomSwayDelay.y);

            if (m_swayEnabled)
            {
                m_swayCoefficientTween = DOVirtual.Float(0f, 1f, 1f / m_swaySpeed, f => m_swayCoefficient = f)
                .SetDelay(delay)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(m_swayEase);
            }

            m_unstickYShiftTween = DOVirtual.Float(-0.5f, 0.5f, 1f / m_unstickSpeed, f => m_unstickYShift = f)
                .SetDelay(delay)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(m_unstickEase);
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

        private void Update()
        {
            if (m_inThrowAnimation)
                AnimateThrow();
        }

        private void AnimateThrow()
        {
            Vector3 firePoint = m_controller.FirePointGlobal;
            Vector3 startPoint = m_initialPositionBeforeThrow;
            Vector3 pivot = (startPoint + firePoint) * 0.5f;

            pivot -= new Vector3(0f, -m_throwPivotFactor);

            Vector3 relativeStart = startPoint - pivot;
            Vector3 relativeEnd = firePoint - pivot;

            Vector3 targetPosition = Vector3.Slerp(relativeStart, relativeEnd, m_throwTimer / m_throwDuration);

            transform.position = targetPosition + pivot;

            m_throwTimer += Time.deltaTime * (1f + m_throwSpeedCurve.Evaluate(m_throwTimer / m_throwDuration));

            if (m_throwTimer > m_throwDuration)
            {
                m_inThrowAnimation = false;
                transform.position = firePoint;
                m_target.CommitThrowAnimationEnd();
            }
        }

        private Vector3 OrbTargetPositionPostProcess(Vector3 input, OrbState state)
        {
            switch (state)
            {
                case OrbState.OnEllipse:
                    return GetSwayPosition(input);
                case OrbState.Sticked:
                    return GetUnstickPosition(input);
                case OrbState.Throwing:
                    return RunThrowAnimation(input);
                default:
                    return input;
            }
        }

        private Vector3 RunThrowAnimation(Vector3 firePoint)
        {
            if (!m_throwEnabled)
            {
                m_target.transform.position = firePoint;
                m_target.CommitThrowAnimationEnd();
                return firePoint;
            }
            
            m_initialPositionBeforeThrow = transform.position;
            m_throwTimer = 0f;
            m_inThrowAnimation = true;
            m_throwTilt = UnityEngine.Random.Range(m_throwTiltRange.x, m_throwTiltRange.y);
            return firePoint;
        }

        private void OnAllOrbsReturn()
        {
            if (m_state != OnEllipseState.AllOrbCall_Rotate)
                return;

            m_state = OnEllipseState.Idle_Sway;
            ScaleEllipse(1f);
        }

        private void OnAllOrbsCalled(IEnumerable<SimpleOrb> orbsCalled)
        {
            if (m_state != OnEllipseState.Idle_Sway)
                return;

            m_state = OnEllipseState.AllOrbCall_Rotate;
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
            //switch (state)
            //{
            //    case OrbState.OnEllipse:
            //        break;
            //    case OrbState.Sticked:
            //        break;
            //    case OrbState.Throwing:
            //        break;
            //    case OrbState.Returning:
            //        break;
            //    default:
            //        break;
            //}
        }

        private Vector3 GetUnstickPosition(Vector3 input)
        {
            if (m_target.StickedTransform != null)
                return input;

            input.y += m_unstickYShift * m_unstickMagnitude;
            return input;
        }

        Vector3 GetSwayPosition(Vector3 input)
        {
            if (m_state != OnEllipseState.Idle_Sway)
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
