using com.absence.variablesystem.builtin;
using com.absence.variablesystem.internals;
using com.absence.variablesystem.mutations.internals;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.statsystem
{
    [System.Serializable]
    public abstract class StatHolder<T> where T : Enum
    {
        protected abstract Dictionary<T, Float> GenerateDefaultEntries();

        Dictionary<T, Float> m_variableObjectEntries;

        protected StatHolder()
        {
            m_variableObjectEntries = GenerateDefaultEntries();
        }

        public Mutation<float> ModifyCustom(T targetStat, Mutation<float> mutationObject)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            desiredStatVariable.Mutate(mutationObject);
            return mutationObject;
        }

        public FloatAdditionMutation ModifyIncremental(T targetStat, float amount, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            FloatAdditionMutation mutationObject = new(amount, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return mutationObject;
        }

        public FloatMultiplicationMutation ModifyPercentage(T targetStat, float percentage, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            if (StatSystemSettings.PERCENTAGE_MODS_ON_TOP) affectionMethod = AffectionMethod.Overall;

            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            float realPercentage = 1f + (percentage / 100f);
            FloatMultiplicationMutation mutationObject = new(realPercentage, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return mutationObject;
        }

        public Float GetDesiredStatVariable(T targetStat)
        {
            if (!m_variableObjectEntries.TryGetValue(targetStat, out Float value))
            {
                Debug.LogError("An error occurred determining a stat's desired variable. " +
                    "It's entry may not exist.");

                return null;
            }

            return value;
        }

        public bool TryGetDesiredStatVariable(T targetStat, out Float desiredStatVariable)
        {
            desiredStatVariable = GetDesiredStatVariable(targetStat);
            if (desiredStatVariable != null) return true;
            else return false;
        }
    }
}
