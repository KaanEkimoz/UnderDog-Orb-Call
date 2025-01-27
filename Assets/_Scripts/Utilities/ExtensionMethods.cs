using UnityEngine;

namespace com.game.utilities
{
    public static class ExtensionMethods
    {
        public static Vector2 RotateByAngle(this Vector2 target, float angleInDegrees)
        {
            float deg2rad = Mathf.Deg2Rad;
            float sin = Mathf.Sin(angleInDegrees * deg2rad);
            float cos = Mathf.Cos(angleInDegrees * deg2rad);
            float x = target.x;
            float y = target.y;
            return new Vector2(cos * x - sin * y, sin * x + cos * y);
        }

        public static Vector3 ProjectXZ(this Vector2 target)
        {
            return new Vector3(target.x, 0f, target.y);
        }

        public static Vector3 ProjectZX(this Vector2 target)
        {
            return new Vector3(target.y, 0f, target.x);
        }
    }
}
