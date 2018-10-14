using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRYB {
    public readonly float r, y, b;

    public ColorRYB(float r, float y, float b) {
        this.r = r;
        this.y = y;
        this.b = b;
    }

    public static Color Complementary(Color c) {
        ColorHSV hsv = RYBtoHSV(RGBtoRYB(c));

        if (hsv.h - .5 < 0) {
            hsv.h = ( 1f + ( hsv.h - .5f ));
            return RYBtoRGB(HSVtoRYB(hsv));
        } else {
            hsv.h = ( hsv.h - .5f );
            return RYBtoRGB(HSVtoRYB(hsv));
        }
    }

    public static ColorRYB RGBtoRYB(Color rgb) {
        float yellow, red = rgb.r, green = rgb.g, blue = rgb.b;

        float white = Mathf.Min(red, green, blue);
        red -= white;
        green -= white;
        blue -= white;

        float maxGreen = Mathf.Max(red, green, blue);

        yellow = Mathf.Min(red, green);

        red -= yellow;
        green -= yellow;

        if (blue > 0 && green > 0) {
            blue /= 2;
            green /= 2;
        }

        yellow += green;
        blue += green;

        float maxYellow = Mathf.Max(red, yellow, blue);

        if (maxYellow > 0) {
            float n = maxGreen / maxYellow;
            red *= n;
            yellow *= n;
            blue *= n;
        }

        blue += white;
        red += white;
        yellow += white;

        return new ColorRYB(red, yellow, blue);
    }

    public static Color RYBtoRGB(ColorRYB ryb) {
        float red = ryb.r, yellow = ryb.y, blue = ryb.b, green;

        float white = Mathf.Min(red, yellow, blue);
        red -= white;
        yellow -= white;
        blue -= white;

        float maxYellow = Mathf.Max(red, yellow, blue);

        green = Mathf.Min(yellow, blue);

        yellow -= green;
        blue -= green;

        if (blue > 0 && green > 0) {
            blue *= 2f;
            green *= 2f;
        }

        red += yellow;
        green += yellow;

        float maxGreen = Mathf.Max(red, green, blue);

        if (maxGreen > 0) {
            float n = maxYellow / maxGreen;

            red *= n;
            green *= n;
            blue *= n;

        }

        red += white;
        green += white;
        blue += white;

        return new Color(red, green, blue);
    }

    public static ColorHSV RYBtoHSV(ColorRYB ryb) {
        float h, s, v, red = ryb.r, yellow = ryb.y, blue = ryb.b;
        float max = Mathf.Max(red, yellow, blue);
        float min = Mathf.Min(red, yellow, blue);
        float delta = max - min;

        if (delta == 0) {
            h = 0;
        } else if (max == red) {
            h = 60 * ( ( ( yellow - blue ) / delta ) % 6 );
        } else if (max == yellow) {
            h = 60 * ( ( ( blue - red ) / delta ) + 2 );
        } else {
            h = 60 * ( ( ( red - yellow ) / delta ) + 4 );
        }
        h = h / 360f;

        if (max == 0) {
            s = 0;
        } else {
            s = delta / max;
        }

        v = max;

        return new ColorHSV(h, s, v);
    }

    public static ColorRYB HSVtoRYB(ColorHSV hsv) {
        float r = 0, y = 0, b = 0, h = hsv.h * 360, s = hsv.s, v = hsv.v;
        float c = v * s;
        float x = c * ( 1 - Mathf.Abs(( ( ( h ) / ( 60 ) ) % 2 ) - 1));
        float m = v - c;

        if (h >= 0 && h < 60) {
            r = c;
            y = x;
        } else if (h >= 60 && h < 120) {
            r = x;
            y = c;
        } else if (h >= 120 && h < 180) {
            y = c;
            b = x;
        } else if (h >= 180 && h < 240) {
            y = x;
            b = c;
        } else if (h >= 240 && h < 300) {
            r = x;
            b = c;
        } else if (h >= 300 && h < 360) {
            r = c;
            b = x;
        }

        return new ColorRYB(r + m, y + m, b + m);
    }
}

public class ColorHSV {
    public float h, s, v;

    public ColorHSV(float h, float s, float v) {
        this.h = h;
        this.s = s;
        this.v = v;
    }
}
