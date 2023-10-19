using UnityEngine;

namespace Chromatic {

    /// <summary>
    /// Adapted from "The Color Blind Simulation function" by Matthew Wickline
    /// and the Human - Computer Interaction Resource Network(http://hcirn.com/), 2000 - 2001.
    /// </summary>
    internal class Colorblind {

        private struct rBlind {
            public float cpu;
            public float cpv;
            public float am;
            public float ayi;

            public rBlind(float cpu, float cpv, float am, float ayi) {
                this.cpu = cpu;
                this.cpv = cpv;
                this.am  = am;
                this.ayi = ayi;
            }

            private static readonly rBlind _protan = new rBlind(0.735f,  0.265f, 1.273463f, -0.073894f);
            private static readonly rBlind _deutan = new rBlind(1.14f,  -0.14f,  0.968437f,  0.003331f);
            private static readonly rBlind _tritan = new rBlind(0.171f, -0.003f, 0.062921f,  0.292119f);

            public static rBlind protan { get { return _protan; } }
            public static rBlind deutan { get { return _deutan; } }
            public static rBlind tritan { get { return _tritan; } }
        }


        private delegate Color FilterFunction(Color c);
        private static FilterFunction fBlind(ColorBlindType colorblindType) {
            switch (colorblindType) {
                case ColorBlindType.Normal:
                    return FilterFunction_Normal;
                case ColorBlindType.Protanopia:
                    return FilterFunction_Protanopia;
                case ColorBlindType.Protanomaly:
                    return FilterFunction_Protanomaly;
                case ColorBlindType.Deuteranopia:
                    return FilterFunction_Deuteranopia;
                case ColorBlindType.Deuteranomaly:
                    return FilterFunction_Deuteranomaly;
                case ColorBlindType.Tritanopia:
                    return FilterFunction_Tritanopia;
                case ColorBlindType.Tritanomaly:
                    return FilterFunction_Tritanomaly;
                case ColorBlindType.Achromatopsia:
                    return FilterFunction_Achromatopsia;
                case ColorBlindType.Achromatomaly:
                    return FilterFunction_Achromatomaly;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private static Color FilterFunction_Normal(Color c) {
            return c;
        }
        private static Color FilterFunction_Protanopia(Color c) {
            return blindMK(c, rBlind.protan);
        }
        private static Color FilterFunction_Protanomaly(Color c) {
            return anomylize(c, blindMK(c, rBlind.protan));
        }
        private static Color FilterFunction_Deuteranopia(Color c) {
            return blindMK(c, rBlind.deutan);
        }
        private static Color FilterFunction_Deuteranomaly(Color c) {
            return anomylize(c, blindMK(c, rBlind.deutan));
        }
        private static Color FilterFunction_Tritanopia(Color c) {
            return blindMK(c, rBlind.tritan);
        }
        private static Color FilterFunction_Tritanomaly(Color c) {
            return anomylize(c, blindMK(c, rBlind.tritan));
        }
        private static Color FilterFunction_Achromatopsia(Color c) {
            return monochrome(c);
        }
        private static Color FilterFunction_Achromatomaly(Color c) {
            return anomylize(c, monochrome(c));
        }

        private static Color rgb2xyz(Color rgb) {
            return new Color() {
                r = 0.430574f * rgb.r + 0.341550f * rgb.g + 0.178325f * rgb.b,
                g = 0.222015f * rgb.r + 0.706655f * rgb.g + 0.071330f * rgb.b,
                b = 0.020183f * rgb.r + 0.129553f * rgb.g + 0.939180f * rgb.b
            };
        }

        private static Color rgb2xyz(float r, float g, float b) {
            return new Color() {
                r = 0.430574f * r + 0.341550f * g + 0.178325f * b,
                g = 0.222015f * r + 0.706655f * g + 0.071330f * b,
                b = 0.020183f * r + 0.129553f * g + 0.939180f * b
            };
        }

        private static Color xyz2rgb(Color xyz) {
            return new Color() {
                r =  3.063218f * xyz.r - 1.393325f * xyz.g - 0.475802f * xyz.b,
                g = -0.969243f * xyz.r + 1.875966f * xyz.g + 0.041555f * xyz.b,
                b =  0.067871f * xyz.r - 0.228834f * xyz.g + 1.069251f * xyz.b
            };
        }

        private static Color xyz2rgb(float x, float y, float z) {
            return new Color() {
                r =  3.063218f * x - 1.393325f * y - 0.475802f * z,
                g = -0.969243f * x + 1.875966f * y + 0.041555f * z,
                b =  0.067871f * x - 0.228834f * y + 1.069251f * z
            };
        }

        private static Color anomylize(Color c1, Color c2) {
            float v = 1.75f;
            float d = v * 1f + 1f;

            return new Color(
                (v * c2.r + c1.r * 1) / d,
                (v * c2.g + c1.g * 1) / d,
                (v * c2.b + c1.b * 1) / d
            );
        }

        private static Color monochrome(Color c) {
            float z = c.r * 0.299f + c.g * 0.587f + c.b * 0.114f;
            return new Color(z, z, z);
        }

        private static Color blindMK(Color rgb, rBlind t) {
            float gamma = 2.2f;
            float wx = 0.312713f;
            float wy = 0.329016f;
            float wz = 0.358271f;

            float r = rgb.r;
            float g = rgb.g;
            float b = rgb.b;

            Color c_rgb = new Color(Mathf.Pow(r, gamma), Mathf.Pow(g, gamma), Mathf.Pow(b, gamma));
            Color c_xyz = rgb2xyz(c_rgb);

            float sum_xyz = c_xyz.Sum();

            float c_u = 0f;
            float c_v = 0f;

            if (sum_xyz != 0) {
                c_u = c_xyz[0] / sum_xyz;
                c_v = c_xyz[1] / sum_xyz;
            }

            float nx = wx * c_xyz[1] / wy;
            float nz = wz * c_xyz[1] / wy;

            float d_y = 0f;

            float clm;
            if (c_u < t.cpu) {
                clm = (t.cpv - c_v) / (t.cpu - c_u);
            } else {
                clm = (c_v - t.cpv) / (c_u - t.cpu);
            }
            float clyi = c_v - c_u * clm;
            float d_u = (t.ayi - clyi) / (clm - t.am);
            float d_v = (clm * d_u) + clyi;

            float s_x = d_u * c_xyz[1] / d_v;
            float s_y = c_xyz[1];
            float s_z = (1f - (d_u + d_v)) * c_xyz[1] / d_v;

            Color s_rgb = xyz2rgb(s_x, s_y, s_z);

            float d_x = nx - s_x;
            float d_z = nz - s_z;

            Color d_rgb = xyz2rgb(d_x, d_y, d_z);

            float adjr;
            float adjg;
            float adjb;
            if (d_rgb.r != 0) {
                int constant = s_rgb[0] < 0 ? 0 : 1;
                adjr = (constant - s_rgb[0]) / d_rgb[0];
            } else {
                adjr = 0;
            }
            if (d_rgb[1] != 0) {
                int constant = s_rgb[1] < 0 ? 0 : 1;
                adjg = (constant - s_rgb[1]) / d_rgb[1];
            } else {
                adjg = 0;
            }
            if (d_rgb[2] != 0) {
                int constant = s_rgb[2] < 0 ? 0 : 1;
                adjb = (constant - s_rgb[2]) / d_rgb[2];
            } else {
                adjb = 0;
            }
            float adjust = Mathf.Max(
                (adjr > 1 || adjr < 0) ? 0 : adjr,
                (adjg > 1 || adjg < 0) ? 0 : adjg,
                (adjb > 1 || adjb < 0) ? 0 : adjb
            );

            float s_r = s_rgb[0] + (adjust * d_rgb[0]);
            float s_g = s_rgb[1] + (adjust * d_rgb[1]);
            float s_b = s_rgb[2] + (adjust * d_rgb[2]);

            return new Color() {
                r = (s_r <= 0) ? 0.0f : (s_r >= 1) ? 1.0f : Mathf.Pow(s_r, 1.0f / gamma),
                g = (s_g <= 0) ? 0.0f : (s_g >= 1) ? 1.0f : Mathf.Pow(s_g, 1.0f / gamma),
                b = (s_b <= 0) ? 0.0f : (s_b >= 1) ? 1.0f : Mathf.Pow(s_b, 1.0f / gamma)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DimulateImage(Texture2D img, ColorBlindType colorBlindType) {
                    /*
            filter_function = fBlind[colorblind_type]

            img = mpimg.imread(img_path)
            n_rows = img.shape[0]
            n_columns = img.shape[1]

            filtered_img = np.zeros((n_rows, n_columns, 3))

            for r in range(n_rows):
                for c in range(n_columns):
                    filtered_img[r, c] = filter_function(img[r, c, 0:3])

            fig, axes = plt.subplots(1, 2, figsize = (12, 6))

            axes[0].imshow(img)
            axes[1].imshow(filtered_img)

            axes[0].axis("off")
            axes[1].axis("off")

            axes[0].set_title("Normal Vision")
            axes[1].set_title("With " + colorblind_type)

            plt.show()
            */
        }

        /// <summary>
        /// Transforms an (r,g,b) colour into a simulation of how
        /// a person with colourblindnes would see that colour.
        /// </summary>
        internal static Color ColorblindFilter(Color c, ColorBlindType colorblindType) {
            return fBlind(colorblindType)(c);
        }
    }
}