namespace com.game.utilities
{
    public interface ISlowable : IObject
    {
        public void SlowForSeconds(float slowPercent, float duration);
    }
}
