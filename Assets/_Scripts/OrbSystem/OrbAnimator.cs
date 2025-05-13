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

        public enum AxisType
        {
            Global,
            Transform,
            Vector
        }

        [SerializeField, Readonly] private SimpleOrb m_target;
        [SerializeField, Readonly] private State m_state = State.Idle_Sway;

        [Header("Sway Animation (Idle)")]
        [SerializeField] private bool m_swayEnabled;
        [SerializeField] private float m_swayMagnitude;
        [SerializeField, Min(0.001f)] private float m_swayMaxAngleChange = 45f;
        [SerializeField, Min(0.001f)] private float m_swaySmoothing = 1f;
        [SerializeField] private bool m_disabledOnLocalZ = true;

        [Header("All Orbs Calling Animation (Rotate)")]
        [SerializeField] private AxisType m_overrideUpAxis = AxisType.Global;
        [SerializeField, Min(0.001f)] private float m_rotationSmoothing = 1f;
        [SerializeField] private float m_rotationSpeed;
        [SerializeField] private float m_rotationRecoverDuration;
        [SerializeField, ShowIf(nameof(m_overrideUpAxis), AxisType.Transform)] private Transform m_axisReference;
        [SerializeField, ShowIf(nameof(m_overrideUpAxis), AxisType.Vector)] private Vector3 m_axisValue;

        Vector3 m_initialEulers;
        Vector3 m_lastSwayDirection = Vector3.up;
        PlayerOrbController m_controller;

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

        private void Update()
        {
            Animate();
        }

        void Animate()
        {
            if (m_target.currentState != OrbState.OnEllipse)
                return;

            switch (m_state)
            {
                case State.AllOrbCall_Rotate:
                    Animate_Rotate();
                    break;
                default:
                    break;
            }
        }

        private void Animate_Rotate()
        {
            Vector3 currentEulers = transform.eulerAngles;
            Vector3 targetEulers = GetRotatedVector(currentEulers, m_rotationSpeed);

            Vector3 result = Vector3.Lerp(currentEulers, targetEulers, Time.deltaTime / m_rotationSmoothing);

            transform.eulerAngles = result;
        }

        private void OnAllOrbsReturn()
        {
            if (m_state == State.AllOrbCall_Rotate) 
                m_state = State.Idle_Sway;

            ResetRotation();
        }

        private void OnAllOrbsCalled(IEnumerable<SimpleOrb> orbsCalled)
        {
            if (m_state == State.Idle_Sway) 
                m_state = State.AllOrbCall_Rotate;
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
            Vector3 randomDirection = m_disabledOnLocalZ ?
                UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitSphere;

            Vector3 direction = Vector3.RotateTowards(m_lastSwayDirection, randomDirection, m_swayMaxAngleChange * Mathf.Deg2Rad, 
                Time.deltaTime * m_swaySmoothing);

            Vector3 targetPosition = input + (direction * m_swayMagnitude);

            //return Vector3.Lerp(input, targetPosition, Time.deltaTime / m_swaySmoothing);

            m_lastSwayDirection = direction;
            return targetPosition;
        }

        Vector3 GetUpAxis()
        {
            return m_overrideUpAxis switch
            {
                AxisType.Global => Vector3.up,
                AxisType.Transform => m_axisReference.up,
                AxisType.Vector => m_axisValue,
                _ => throw new NotImplementedException(),
            };
        }

        void ResetRotation()
        {
            transform.DORotate(m_initialEulers, m_rotationRecoverDuration);
        }

        public Vector3 GetRotatedVector(Vector3 target, float angle)
        {
            return GetRotatedVector(target, angle, GetUpAxis());
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
