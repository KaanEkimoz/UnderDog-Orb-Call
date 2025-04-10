using com.absence.attributes;
using com.game.generics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemyFlash : MonoBehaviour, ISpark
    {
        [System.Serializable]
        public class MaterialEntry
        {
            [DisableIf(nameof(DefaultMaterialType))]
            public string EmissionProperty = DefaultEmissionValueProperty;

            public int MaterialIndex;
            public bool DefaultMaterialType = true;

            public bool CustomEmissionColor = false;

            [ShowIf(nameof(CustomEmissionColor))]
            public Color EmissionColor;

            public float EmissionMultiplier = 1f;

            [HideInInspector] public Material Material;

            public void Initialize(Renderer targetRenderer)
            {
                Material = targetRenderer.materials[MaterialIndex];

                if (DefaultMaterialType)
                {
                    Material.EnableKeyword(DefaultEmissionSwitchProperty);
                }
            }

            public void OnValidate()
            {
                if (DefaultMaterialType)
                    EmissionProperty = DefaultEmissionValueProperty;
            }

            public void SetEmission(Color emissionColor, float value)
            {
                Color color = GetEmissionColor(emissionColor);
                float multiplier = GetEmissionMultiplier();

                if (DefaultMaterialType) Material.SetColor(DefaultEmissionValueProperty, color * value * multiplier);
                else Material.SetFloat(EmissionProperty, value);
            }

            protected Color GetEmissionColor(Color emissionColor)
            {
                return CustomEmissionColor ?
                    EmissionColor : emissionColor;
            }

            protected float GetEmissionMultiplier()
            {
                return EmissionMultiplier;
            }
        }

        [System.Serializable]
        public class RendererEntry
        {
            public Renderer TargetRenderer;
            public Color EmissionColor;
            [Tooltip("If enabled, you can only have one material entry which is used for all materials of the renderer.")] 
            public bool AllMaterials = true;
            public List<MaterialEntry> MaterialEntries = new();

            public void Initialize()
            {
                foreach (var entry in MaterialEntries)
                {
                    entry.Initialize(TargetRenderer);
                }
            }

            public void OnValidate()
            {
                if (AllMaterials)
                {
                    if (MaterialEntries.Count > 1)
                    {
                        MaterialEntry entry = MaterialEntries[0];
                        MaterialEntries.Clear();
                        MaterialEntries.Add(entry);
                    }

                    else if (MaterialEntries.Count < 1)
                    {
                        MaterialEntries.Add(new MaterialEntry());
                    }
                }

                foreach (var entry in MaterialEntries)
                {
                    entry.OnValidate();
                }
            }

            public void SetEmission(float value)
            {
                foreach (var entry in MaterialEntries)
                {
                    entry.SetEmission(EmissionColor, value);
                }
            }
        }

        public enum Phase
        {
            [InspectorName("Idle (Not Active)")] Idle,
            Increasing,
            Decreasing,
        }

        public static readonly string CustomEmissionProperty = "_Emission";
        public static readonly string DefaultEmissionSwitchProperty = "_EMISSION";
        public static readonly string DefaultEmissionValueProperty = "_EmissionColor";

        [Header("Emission Settings")]
        [SerializeField, Readonly] private Phase m_flashPhase;
        [SerializeField, NonReorderable] private List<RendererEntry> m_rendererEntries = new();
        [SerializeField] private float m_noEmissionValue = 0f;
        [SerializeField] private float m_maxEmissionValue = 2f;
        [SerializeField] private float m_flashDuration = 0.5f;
        [SerializeField] private float m_flashSpeedMultiplier = 1f;

        [Header("Timer Settings")]
        [SerializeField] private float m_minTime = 2f;
        [SerializeField] private float m_maxTime = 5f;

        public event Action OnEnd;

        float m_timer;
        bool m_isFlashing;
        float m_flashTimer;

        private void Start()
        {
            Initialize();
            SetEmission(m_noEmissionValue);

            m_timer = GetRandomTime();
        }

        void Initialize()
        {
            foreach (var entry in m_rendererEntries)
            {
                entry.Initialize();
            }
        }

        private void Update()
        {
            if (!m_isFlashing)
            {
                if (m_timer > 0f) m_timer -= Time.deltaTime;
                else m_timer = 0f;

                return;
            }

            HandleFlash();
        }

        public void Spark()
        {
            if (m_isFlashing)
                return;

            if (m_timer > 0f)
                return;

            StartFlash();
        }

        public void ForceStop()
        {
            Stop();
        }

        private void StartFlash()
        {
            m_isFlashing = true;
            m_flashTimer = 0f;
            m_flashPhase = Phase.Increasing; // Start increasing
        }

        private void HandleFlash()
        {
            m_flashTimer += Time.deltaTime * m_flashSpeedMultiplier;
            float t = m_flashTimer / (m_flashDuration / 2f);

            if (m_flashPhase == Phase.Increasing) // Increasing
            {
                SetEmission(Mathf.Lerp(m_noEmissionValue, m_maxEmissionValue, t));
                if (m_flashTimer >= m_flashDuration / 2f)
                {
                    m_flashPhase = Phase.Decreasing;
                    m_flashTimer = 0f;
                }
            }
            else if (m_flashPhase == Phase.Decreasing) // Decreasing
            {
                SetEmission(Mathf.Lerp(m_maxEmissionValue, m_noEmissionValue, t));
                if (m_flashTimer >= m_flashDuration / 2f)
                {
                    Stop();
                }
            }
        }

        private void OnValidate()
        {
            foreach (var entry in m_rendererEntries)
            {
                entry.OnValidate();
            }
        }

        void Stop()
        {
            if (!m_isFlashing)
                return;

            m_isFlashing = false;
            SetEmission(m_noEmissionValue);

            m_timer = GetRandomTime();

            OnEnd?.Invoke();
        }

        void SetEmission(float value)
        {
            foreach(var entry in m_rendererEntries)
            {
                entry.SetEmission(value);
            }
        }

        private float GetRandomTime()
        {
            return UnityEngine.Random.Range(m_minTime, m_maxTime);
        }
    }
}
