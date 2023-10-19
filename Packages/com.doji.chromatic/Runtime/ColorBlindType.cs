namespace Chromatic {

    public enum ColorBlindType {
        /// <summary>
        /// Normal vision
        /// </summary>
        Normal,

        /// <summary>
        /// Red-green colorblindness (1% males)
        /// </summary>
        Protanopia,

        /// <summary>
        /// Red-green colorblindness (1% males, 0.01% females)
        /// </summary>
        Protanomaly,

        /// <summary>
        /// Red-green colorblindness (1% males)
        /// </summary>
        Deuteranopia,

        /// <summary>
        /// Red-green colorblindness (most common type: 6% males, 0.4% females)
        /// </summary>
        Deuteranomaly,

        /// <summary>
        /// Blue-yellow colorblindness (<1% males and females)
        /// </summary>
        Tritanopia,

        /// <summary>
        /// Blue-yellow colorblindness (0.01% males and females)
        /// </summary>
        Tritanomaly,

        /// <summary>
        /// Total colorblindness
        /// </summary>
        Achromatopsia,

        /// <summary>
        /// Total colorblindness
        /// </summary>
        Achromatomaly
    }
}