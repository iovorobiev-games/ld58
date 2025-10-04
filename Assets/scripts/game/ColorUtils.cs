using UnityEngine;

namespace DefaultNamespace.game
{
    public static class ColorUtils
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}