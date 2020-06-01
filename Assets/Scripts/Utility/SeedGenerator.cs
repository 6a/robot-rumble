namespace RR.Utility
{
    public static class SeedGenerator
    {
        public static int StringToSeed(string input)
        {
            var output = 0;

            foreach (var c in input)
            {
                output += (int)c;
            }

            return output;
        }
    }
}
