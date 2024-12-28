using com.absence.variablesystem.builtin;
using com.game.statsystem;
using System.Collections.Generic;

namespace com.game.orbsystem
{
    [System.Serializable]
    public class OrbStatHolder : StatHolder<OrbStatType>
    {
        protected override Dictionary<OrbStatType, Float> GenerateDefaultEntries()
        {
            return new Dictionary<OrbStatType, Float>()
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
