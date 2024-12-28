namespace com.game.player.statsystemextensions
{
    /// <summary>
    /// The enum that holds entries of player stat - variable object pairs. I know
    /// enumeration isn't the most convenient way of selecting among multiple entries but,
    /// for this project it is the best for both seperating public API from internal
    /// stat system AND keeping in-use syntax of stat system API simple.
    /// </summary>
    public enum PlayerStatType
    {
        Health,
        Armor,
        WalkSpeed,
        LifeSteal,
        Luck,
        Gathering,
        Damage,
        AttackSpeed,
        CriticalHits,
        Range,
        Knockback,
        Penetration,
        CrowdControl,
        LightStrength,
    }
}