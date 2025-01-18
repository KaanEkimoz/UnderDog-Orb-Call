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

        protected override int m_order => 2;

        public FloatCapMutation() : base(0f, AffectionMethod.Overall)
        {
        }

        public override float Apply(float initialValue)
        {
            if (CapLow && initialValue < MinValue)
            {
                return MinValue;
            }

            if (CapHigh && initialValue > MaxValue) 
            { 
                return MaxValue;
            }

            return initialValue;
        }

        public override void OnAdd(Variable<float> variable)
        {
            base.OnAdd(variable);
        }
    }
}
