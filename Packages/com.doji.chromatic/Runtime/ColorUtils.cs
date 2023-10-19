using UnityEngine;

namespace Chromatic {
    
    public static class ColorUtils {
        public static float Sum(this Color c) {
            return c.r + c.g + c.b;
        }
    }
}