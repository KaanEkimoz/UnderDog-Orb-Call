using com.absence.variablesystem.builtin;
using com.absence.variablesystem.internals;
using com.absence.variablesystem.mutations.internals;
using com.game.statsystem.extensions;
using com.game.statsystem.presetobjects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game.statsystem
{
    [System.Serializable]
    public abstract class StatHolder<T> where T : Enum
    {
        protected Dictionary<T, Float> m_variableObjectEntries;

        protected StatHolder()
        {
            m_variableObjectEntries = GenerateDefaultEntries();
        }

        protected abstract Dictionary<T, Float> GenerateDefaultEntries();

        /*
         Public API
         */

        #region Helpers

        /// <summary>
        /// Use to perform an action for every stat variable paired with an enum value.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        public virtual void ForAllStats(Action<Float> action)
        {
            m_variableObjectEntries.Values.ToList().ForEach(action);
        }

        /// <summary>
        /// Use to perform an action for every key-value pair of enums and stat variables.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        public virtual void ForAllStatEntries(Action<T, Float> action)
        {
            m_variableObjectEntries.ToList().ForEach(kvp => action?.Invoke(kvp.Key, kvp.Value));
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Use to refresh the value of every stat via removing and re-applying every
        /// modificataion on them.
        /// </summary>
        public virtual void RefreshAll()
        {
            ForAllStats(stat =>
            {
                stat.Refresh();
            });
        }

        /// <summary>
        /// Use to remove all of the modifiers from all of the stats.
        /// </summary>
        public virtual void ClearAllModifiers()
        {
            ForAllStats(stat =>
            {
                stat.ClearMutations();
            });

            RefreshAll();
        }

        /// <summary>
        /// Use to refresh a single stat variable.
        /// </summary>
        /// <param name="targetStat">The stat variable to refresh.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        public virtual bool Refresh(T targetStat)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return false;

            desiredStatVariable.Refresh();
            return true;
        }

        /// <summary>
        /// Use to clear modifiers applied to a single stat variable.
        /// </summary>
        /// <param name="targetStat">The stat to clear modifiers of.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        public virtual bool ClearModifiersOf(T targetStat)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return false;

            desiredStatVariable.ClearMutations();
            return true;
        }

        #endregion

        #region Basic Operations

        /// <summary>
        /// Use to increment a stat variable WITHOUT creating a modifier.
        /// </summary>
        /// <param name="targetStat">The stat to increment.</param>
        /// <param name="amount">Amount of incrementation.</param>
        /// <param name="setType">Selection of the 'set' logic.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        public virtual bool BasicIncrement(T targetStat, float amount, SetType setType = SetType.Baked)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return false;

            desiredStatVariable.Add(amount, setType);
            return true;
        }

        /// <summary>
        /// Use to multiply a stat variable WITHOUT creating a modifier.
        /// </summary>
        /// <param name="targetStat">The stat to multiply.</param>
        /// <param name="multiplier">Amount of multiplication.</param>
        /// <param name="setType">Selection of the 'set' logic.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        public virtual bool BasicMultiply(T targetStat, float multiplier, SetType setType = SetType.Baked)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return false;

            desiredStatVariable.Multiply(multiplier, setType);
            return true;
        }

        /// <summary>
        /// Use to override a stat variable WITHOUT creating a modifier.
        /// </summary>
        /// <param name="targetStat">The stat to multiply.</param>
        /// <param name="newValue">Intended new value of the stat variable.</param>
        /// <param name="setType">Selection of the 'set' logic.</param>
        /// <param name="clearMutations">If true, the modifiers applied to the
        /// target variable of this process will all get cleared before overriding
        /// the variable value itself.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        public virtual bool BasicOverride(T targetStat, float newValue, SetType setType = SetType.Baked, bool clearMutations = true)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return false;

            if (clearMutations) ClearModifiersOf(targetStat);
            desiredStatVariable.Set(newValue, setType);
            return true;
        }

        #endregion

        #region Modifiers with Editor Presets

        /// <summary>
        /// Use to modify a stat variable with a <see cref="StatModification"/>.
        /// </summary>
        /// <param name="mod">The modification object.</param>
        /// <returns>Returns the mutation object created from this operation.</returns>
        public virtual ModifierObject ModifyWith(StatModification<T> mod)
        {
            ModifierObject result = null;

            if (mod.ModificationType == StatModificationType.Incremental)
            {
                result = ModifyIncremental(mod.TargetStatType, mod.Value, AffectionMethod.InOrder);
            }

            else if (mod.ModificationType == StatModificationType.Percentage)
            {
                bool onTop = StatSystemSettings.PERCENTAGE_MODS_ON_TOP;
                AffectionMethod affectionMethod = onTop ? AffectionMethod.Overall : AffectionMethod.InOrder;

                result = ModifyPercentage(mod.TargetStatType, mod.Value, affectionMethod);
            }

            return result;
        }

        /// <summary>
        /// Use to cap a stat variable with a <see cref="StatCap"/>.
        /// </summary>
        /// <param name="cap">The modification object.</param>
        /// <returns>Returns the mutation object created from this operation.</returns>
        public virtual ModifierObject CapWith(StatCap<T> cap)
        {
            FloatCapMutation result = new FloatCapMutation()
            {
                CapLow = cap.CapLow,
                CapHigh = cap.CapHigh,
                MinValue = cap.MinValue,
                MaxValue = cap.MaxValue,
            };

            ModifyCustom(cap.TargetStatType, result);

            return new ModifierObject(result);
        }

        /// <summary>
        /// Use to override a stat variable with a <see cref="StatOverride"/>.
        /// </summary>
        /// <param name="ovr">The modification object.</param>
        /// <returns>Returns the new value of the stat variable.</returns>
        public virtual float OverrideWith(StatOverride<T> ovr)
        {
            Float targetVariable = GetDesiredStatVariable(ovr.TargetStatType);
            bool clear = StatSystemSettings.OVERRIDES_CLEAR_MUTATIONS;
            float newValue = ovr.NewValue;

            if (clear)
            {
                targetVariable.ClearMutations();
                targetVariable.Value = newValue;
                return newValue;
            }

            bool root = StatSystemSettings.OVERRIDES_AFFECT_FROM_ROOT;
            SetType setType = root ? SetType.Raw : SetType.Baked;

            targetVariable.Set(newValue, setType);
            return targetVariable.Value;
        }

        #endregion

        #region Modifiers with Values

        /// <summary>
        /// Use to add a custom-logic modification to a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="mutationObject">Mutation object created</param>
        /// <returns>Returns the mutation object passed as an argument.</returns>
        public virtual ModifierObject ModifyCustom(T targetStat, Mutation<float> mutationObject)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            desiredStatVariable.Mutate(mutationObject);
            return new ModifierObject(mutationObject);
        }

        /// <summary>
        /// Use to add an incremental modification (eg. +5, -2) to a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="amount">Amount of modification.</param>
        /// <returns>
        /// Returns the mutation applied to desired stat's variable object. You can
        /// remove this modification from the desired stat via the 
        /// <see cref="Demodify(PlayerStatType, Mutation{float})"/> function.
        /// </returns>
        public virtual ModifierObject ModifyIncremental(T targetStat, float amount, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            FloatAdditionMutation mutationObject = new(amount, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return new ModifierObject(mutationObject);
        }

        /// <summary>
        /// Use to add a percentage based modification (eg. +25%, -100%) to a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="percentage">Amount of modification. (in the following form: <i>percentage</i>%).</param>
        /// <returns>
        /// Returns the mutation applied to desired stat's variable object. You can
        /// remove this modification from the desired stat via the 
        /// <see cref="Demodify(PlayerStatType, Mutation{float})"/> function.
        /// </returns>
        public virtual ModifierObject ModifyPercentage(T targetStat, float percentage, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            if (StatSystemSettings.PERCENTAGE_MODS_ON_TOP) affectionMethod = AffectionMethod.Overall;

            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            float realPercentage = 1f + (percentage / 100f);
            FloatMultiplicationMutation mutationObject = new(realPercentage, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return new ModifierObject(mutationObject);
        }

        /// <summary>
        /// Use to de-modify a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to de-modify.</param>
        /// <param name="mutationObject">The mutation object created when
        /// the modification took place.</param>
        public void Demodify(T targetStat, Mutation<float> mutationObject)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return;

            try
            {
                desiredStatVariable.Immutate(mutationObject);
            }

            catch
            {
                Debug.LogError("An error occurred while trying to demodify a stat. An " +
                    "Mutation object used may be invalid.");
            }
        }

        #endregion

        #region Raw

        /// <summary>
        /// Use to get a stat's object reference.
        /// </summary>
        /// <param name="targetStat">Target stat.</param>
        /// <returns>Returns the StatObject wrapper instance of the targeted stat.</returns>
        public virtual StatObject GetStatObject(T targetStat)
        {
            Float variable = GetDesiredStatVariable(targetStat);
            if (variable == null) return null;

            return new StatObject(variable);
        }

        /// <summary>
        /// Use to try getting a stat's object reference.
        /// </summary>
        /// <param name="targetStat">Target stat.</param>
        /// <param name="statObject">Found StatObject instance. Null if the function itself returns false.</param>
        /// <returns>Returns true if the targeted stat exists, false otherwise.</returns>
        public virtual bool TryGetStatObject(T targetStat, out StatObject statObject)
        {
            statObject = null;

            Float variable = GetDesiredStatVariable(targetStat);
            if (variable == null) return false;

            statObject = new StatObject(variable);
            return true;
        }

        #endregion

        #region Vulnerable API

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to get the corresponding variable object of a stat.
        /// </summary>
        /// <param name="targetStat">The target enumeration value corresponds to a variable object.</param>
        /// <returns>Returns the desired variable if the enumeration entry is valid. Returns null otherwise.</returns>
        public virtual Float GetDesiredStatVariable(T targetStat)
        {
            if (!m_variableObjectEntries.TryGetValue(targetStat, out Float value))
            {
                Debug.LogError("An error occurred determining a stat's desired variable. " +
                    "It's entry may not exist.");

                return null;
            }

            return value;
        }

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to check-n-get a corresponding variable object of a stat.
        /// </summary>
        /// <param name="targetStat">The target enumeration value corresponds to a variable object.</param>
        /// <param name="desiredStatVariable">The variable object received from the getting operation. 
        /// Null if the checking operation fails.</param>
        /// <returns>Returns true if the checking operation succeeds. False otherwise.</returns>
        public virtual bool TryGetDesiredStatVariable(T targetStat, out Float desiredStatVariable)
        {
            desiredStatVariable = GetDesiredStatVariable(targetStat);
            if (desiredStatVariable != null) return true;
            else return false;
        }

        #endregion
    }
}
