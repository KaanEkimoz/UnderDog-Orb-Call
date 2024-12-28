using com.game.statsystem;

namespace com.game.orbsystem
{
    [System.Serializable]
    public sealed class OrbStatModification : StatModification<OrbStatType>
    {
    }

    [System.Serializable]
    public sealed class OrbStatOverride : StatOverride<OrbStatType>
    {
    }

    [System.Serializable]
    public sealed class OrbStatCap : StatCap<OrbStatType>
    {
    }
}
