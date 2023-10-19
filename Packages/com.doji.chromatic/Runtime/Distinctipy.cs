using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Chromatic {
    public static class Distinctipy {
        private static readonly List<Color> CORNERS = new List<Color> {
            Color.white,
            Color.black,
            Color.red,
            Color.green,
            Color.blue,
            Color.cyan,
            Color.yellow,
            Color.magenta
        };

        private static readonly List<Color> MID_FACE = new List<Color> {
            new Color(0.0f, 0.5f, 0.0f),
            new Color(0.0f, 0.0f, 0.5f),
            new Color(0.0f, 1.0f, 0.5f),
            new Color(0.0f, 0.5f, 1.0f),
            new Color(0.0f, 0.5f, 0.5f),
            new Color(0.5f, 0.0f, 0.0f),
            new Color(0.5f, 0.5f, 0.0f),
            new Color(0.5f, 1.0f, 0.0f),
            new Color(0.5f, 0.0f, 0.5f),
            new Color(0.5f, 0.0f, 1.0f),
            new Color(0.5f, 1.0f, 0.5f),
            new Color(0.5f, 1.0f, 1.0f),
            new Color(0.5f, 0.5f, 1.0f),
            new Color(1.0f, 0.5f, 0.0f),
            new Color(1.0f, 0.0f, 0.5f),
            new Color(1.0f, 0.5f, 0.5f),
            new Color(1.0f, 1.0f, 0.5f),
            new Color(1.0f, 0.5f, 1.0f)
        };

        private static readonly List<Color> INTERIOR = new List<Color> {
            new Color(0.5f, 0.5f, 0.5f),
            new Color(0.75f, 0.5f, 0.5f),
            new Color(0.25f, 0.5f, 0.5f),
            new Color(0.5f, 0.75f, 0.5f),
            new Color(0.5f, 0.25f, 0.5f),
            new Color(0.5f, 0.5f , 0.75f),
            new Color(0.5f, 0.5f , 0.25f)
        };

        private static readonly Color[] POINTS_OF_INTEREST =
        {
            // CORNERS
            Color.white,
            Color.black,
            Color.red,
            Color.green,
            Color.blue,
            Color.cyan,
            Color.yellow,
            Color.magenta,

            // MID_FACE
            new Color(0.0f, 0.5f, 0.0f),
            new Color(0.0f, 0.0f, 0.5f),
            new Color(0.0f, 1.0f, 0.5f),
            new Color(0.0f, 0.5f, 1.0f),
            new Color(0.0f, 0.5f, 0.5f),
            new Color(0.5f, 0.0f, 0.0f),
            new Color(0.5f, 0.5f, 0.0f),
            new Color(0.5f, 1.0f, 0.0f),
            new Color(0.5f, 0.0f, 0.5f),
            new Color(0.5f, 0.0f, 1.0f),
            new Color(0.5f, 1.0f, 0.5f),
            new Color(0.5f, 1.0f, 1.0f),
            new Color(0.5f, 0.5f, 1.0f),
            new Color(1.0f, 0.5f, 0.0f),
            new Color(1.0f, 0.0f, 0.5f),
            new Color(1.0f, 0.5f, 0.5f),
            new Color(1.0f, 1.0f, 0.5f),
            new Color(1.0f, 0.5f, 1.0f),

            // INTERIOR
            new Color(0.5f , 0.5f , 0.5f),
            new Color(0.75f, 0.5f , 0.5f),
            new Color(0.25f, 0.5f , 0.5f),
            new Color(0.5f , 0.75f, 0.5f),
            new Color(0.5f , 0.25f, 0.5f),
            new Color(0.5f , 0.5f , 0.75f),
            new Color(0.5f , 0.5f , 0.25f)
        };

        private static readonly int _SEED_MAX = (int)(Math.Pow(2, 32) - 1);

        public static Random EnsureRng(Random rng) {
            return rng;
        }
        /*public static Random EnsureRng(Random rng) {
            if (rng == null) {
                rng = _inst;
            } else if (rng is int) {
                rng = new Random((int)rng % _SEED_MAX);
            } else if (rng is float) {
                float floatRng = (float)rng;
                int seed;

                if (floatRng % 1 == 0) {
                    seed = (int)floatRng;
                } else {
                    int a, b;
                    var ratio = floatRng.AsRatio();
                    a = ratio.Item1;
                    b = ratio.Item2;

                    int s = Math.Max(a.BitLength(), b.BitLength());
                    seed = (b << s) | a;
                }

                rng = new Random(seed % _SEED_MAX);
            } else if (rng is Random) {
                rng = rng as Random;
            } else {
                throw new ArgumentException("Invalid RNG type");
            }

            return rng;
        }*/

        /// <summary>
        /// Generate a random rgb color.
        /// </summary>
        /// <param name="pastelFactor">float between 0 and 1. If >0 paler colors will be generated.</param>
        /// <param name="_rng"></param>
        public static Color GetRandomColor(float pastelFactor = 0f, Random _rng = null) {
            return new Color() {
                r = ((float)_rng.NextDouble() + pastelFactor) / (1.0f + pastelFactor),
                g = ((float)_rng.NextDouble() + pastelFactor) / (1.0f + pastelFactor),
                b = ((float)_rng.NextDouble() + pastelFactor) / (1.0f + pastelFactor),
                a = 1f
            };
        }

        /// <summary>
        /// Metric to define the visual distinction between two (r,g,b) colors.
        /// Inspired by: https://www.compuphase.com/cmetric.htm
        /// </summary>
        private static float ColorDistance(Color c1, Color c2) {
            float mean_r = (c1.r + c2.r) / 2f;
            float delta_r = Mathf.Pow(c1.r - c2.r, 2f);
            float delta_g = Mathf.Pow(c1.g - c2.g, 2f);
            float delta_b = Mathf.Pow(c1.b - c2.b, 2f);

            return (2f + mean_r) * delta_r + 4f * delta_g + (3f - mean_r) * delta_b;
        }

        /// <summary>
        /// Generate a color as distinct as possible from the colors defined in <paramref name="excludeColors"/>.
        /// Inspired by: https://gist.github.com/adewes/5884820
        /// </summary>
        private static Color DistinctColor(List<Color> excludeColors, float pastelFactor = 0f,
            int attempts = 1000, ColorBlindType colorBlindType = ColorBlindType.Normal,
            Random rng = null) {
            rng = EnsureRng(rng);

            if (excludeColors == null || excludeColors.Count == 0) {
                return GetRandomColor(pastelFactor, rng);
            }

            if (colorBlindType != ColorBlindType.Normal) {
                for (int i = 0; i < excludeColors.Count; i++) {
                    excludeColors[i] = Colorblind.ColorblindFilter(excludeColors[i], colorBlindType);
                }
            }

            float maxDistance = float.NegativeInfinity;
            Color bestColor = Color.black;
            Color compareColor;

            // try pre-defined corners, edges, interior points first
            if (pastelFactor == 0) {
                foreach (Color color in POINTS_OF_INTEREST) {
                    if (!excludeColors.Contains(color)) {
                        
                        if (colorBlindType != ColorBlindType.Normal) {
                            compareColor = Colorblind.ColorblindFilter(color, colorBlindType);
                        } else {
                            compareColor = color;
                        }

                        float distanceToNearest = excludeColors.Min(c => ColorDistance(compareColor, c));

                        if (distanceToNearest > maxDistance) {
                            maxDistance = distanceToNearest;
                            bestColor = color;
                        }
                    }
                }
            }

            Color c = Color.black;
            // try attempts randomly generated colors
            for (int i = 0; i < attempts; i++) {
                c = GetRandomColor(pastelFactor, rng);
            }

            if (excludeColors == null) { //if not excludeColors:
                return c;
            } else {
                if (colorBlindType != ColorBlindType.Normal) {
                    compareColor = Colorblind.ColorblindFilter(c, colorBlindType);
                } else {
                    compareColor = c;
                }

                float distanceToNearest = excludeColors.Min(c => ColorDistance(compareColor, c));

                if (distanceToNearest > maxDistance) {
                    maxDistance = distanceToNearest;
                    bestColor = c;
                }
            }
            return bestColor;
        }

        /// <summary>
        /// Choose whether black or white text will work better on top of background_color.
        /// Inspired by: https://stackoverflow.com/a/3943023
        /// </summary>
        /// <param name="backgroundColor">The color the text will be displayed on</param>
        /// <param name="threshold">float between 0 and 1. With threshold close to 1 white
        /// text will be chosen more often.</param>
        private static Color GetTextColor(Color backgroundColor, float threshold = 0.6f) {
            if ((backgroundColor.r * 0.299f + backgroundColor.g * 0.587f + backgroundColor.b * 0.114f) > threshold) {
                return Color.black;
            } else {
                return Color.white;
            }
        }

        /// <summary>
        /// Generate a list of n visually distinct colors.
        /// </summary>
        /// <param name="n_colors">How many colors to generate</param>
        /// <param name="excludeColors"> A list of colors that new colors should be distinct
        /// from.If excludeColors = null then excludeColors will be set to avoid white
        /// and black</param>
        /// <param name="returnExcluded">If returnExcluded = true then excludeColors will be included
        /// in the returned color list. Otherwise only the newly generated colors are
        /// returned (default).</param>
        /// <param name="pastelFactor">float between 0 and 1. If pastelFactor>0 paler colors will
        /// be generated.</param>
        /// <param name="attempts">number of random colors to generated to find most distinct color.</param>
        /// <param name="colorBlindType">Generate colors that are distinct with given type of colorblindness.</param>
        /// <param name="rng"></param>
        /// <param name=""></param>
        /// <returns>colors - A list of colors that are visually distinct to each other
        /// and to the colors in excludeColors.</returns>
        private static List<Color> GetColors(
            int n_colors,
            List<Color> excludeColors = null,
            bool returnExcluded = false,
            float pastelFactor = 0f,
            int attempts = 1000,
            ColorBlindType colorBlindType = ColorBlindType.Normal,
            Random rng = null
        ) {
            rng = EnsureRng(rng);

            if (excludeColors == null || excludeColors.Count == 0) {
                excludeColors = new List<Color>() { Color.white, Color.black };
            }

            var colors = new List<Color>(excludeColors);

            for (int i = 0; i < n_colors; i++) {

                Color c = DistinctColor(
                    colors,
                    pastelFactor,
                    attempts,
                    colorBlindType,
                    rng
                );
                colors.Add(c);
            }

            if (returnExcluded) {
                return colors;
            } else {
                return colors.GetRange(excludeColors.Count, colors.Count - excludeColors.Count);
            }
        }

        /// <summary>
        /// Display the colors defined in a list of colors.
        /// </summary>
        /// <param name="colors">List of color tuples to display.</param>
        /// <param name="edgecolors">If None displayed colors have no outline.
        /// Otherwise a list of colors to use as outlines for each color.</param>
        /// <param name="showText">If True writes the background color's hex on top
        /// of it in black or white, as appropriate.</param>
        /// <param name="textThreshold">float between 0 and 1. With threshold close
        /// to 1 white text will be chosen more often.</param>
        /// <param name="ax">Matplotlib axis to plot to. If ax is None plt.show() is run in function call.</param>
        /// <param name="title">Add a title to the color swatch.</param>
        /// <param name="one_row">If True display colors on one row, if False as a grid.
        /// If one_row = null a grid is used when there are more than 8 colors.</param>
        private static void ColorSwatch(
            List<Color> colors,
            List<Color> edgecolors = null,
            bool showText = false,
            float textThreshold = 0.6f,
            object ax = null,
            string title = null,
            bool? one_row = false
        ) {
            throw new NotImplementedException();
        }

    }
}