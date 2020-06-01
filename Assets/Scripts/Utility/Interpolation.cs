using RR.Utility.ExternImports;

namespace RR.Utility
{
    public static class Interpolation
    {
        public static float EaseInQuad(float value, float start, float end) => Interpolation_External.EaseInQuad(value, start, end);
        public static float EaseOutQuad(float value, float start, float end) => Interpolation_External.EaseOutQuad(value, start, end);
        public static float EaseInOutQuad(float value, float start, float end) => Interpolation_External.EaseInOutQuad(value, start, end);
        public static float EaseBounce(float value, float start, float end) => Interpolation_External.EaseBounce(value, start, end);
    }
}