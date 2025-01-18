namespace com.game.statsystem
{
    public interface IStatProvider<T1, T2> where T1 : StatHolder<T2> where T2 : System.Enum
    {
        T1 StatHolder { get; }
    }
}
