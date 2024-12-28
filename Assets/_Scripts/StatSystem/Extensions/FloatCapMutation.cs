using com.absence.variablesystem.internals;
using com.absence.variablesystem.mutations.internals;

namespace com.game.statsystem.extensions
{
    public class FloatCapMutation : Mutation<float>
    {
        public bool CapLow;
        public bool CapHigh;

        public float MinValue;
        public float MaxValue;

        private float m_latestOriginalValue;

        protected override int m_order => 2;

        public FloatCapMutation() : base(0f, AffectionMethod.Overall)
        {
        }

        public override void OnApply(ref float targetValue)
        {
            m_latestOriginalValue = targetValue;

            if (CapLow && targetValue < MinValue)
            {
                targetValue = MinValue;
            }

            if (CapHigh && targetValue > MaxValue) 
            { 
                targetValue = MaxValue;
            }
        }

        public override void OnRevert(ref float targetValue)
        {
            targetValue = m_latestOriginalValue;
        }

        public override void OnAdd(Variable<float> variable)
        {
            base.OnAdd(variable);

            m_latestOriginalValue = variable.Value;
        }
    }
}
