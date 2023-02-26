namespace GameHook.Domain
{
    public static class NCalcFunctions
    {
        public static int BitRange(int x, int upperRange, int lowerRange)
        {
            return (x & ((1 << (upperRange + 1)) - 1)) >> lowerRange;
        }
    }
}
