using com.absence.variablesystem.builtin;
using com.game.statsystem;
using System.Collections.Generic;

namespace com.game.orbsystem.statsystemextensions
{
    /// <summary>
    /// Another example use case of <see cref="StatHolder{T}"/>.
    /// </summary>
    [System.Serializable]
    public class OrbStatHolder : StatHolder<OrbStatType>
    {
        protected override Dictionary<OrbStatType, StatObject> GenerateDefaultEntries()
        {
            return new Dictionary<OrbStatType, StatObject>()
            {

            };
        }

        public static OrbStatHolder Create() 
        {
            OrbStatHolder holder = new OrbStatHolder();

            // any logic here.

            return holder;
        }
    }
}
