using System.Runtime.InteropServices;

namespace RR.Utility.ExternImports
{
    public static class Interpolation_External
    {
        [DllImport("interpolation", EntryPoint = "EaseInQuad")] 
        public static extern float EaseInQuad(float value, float start, float end);

        [DllImport("interpolation", EntryPoint = "EaseOutQuad")] 
        public static extern float EaseOutQuad(float value, float start, float end);

        [DllImport("interpolation", EntryPoint = "EaseInOutQuad")] 
        public static extern float EaseInOutQuad(float value, float start, float end);

        [DllImport("interpolation", EntryPoint = "EaseBounce")]
        public static extern float EaseBounce(float value, float start, float end);
    }
}