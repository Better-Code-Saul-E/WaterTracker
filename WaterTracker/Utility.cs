namespace WaterTracker
{
    public static class Utility
    {
        public static void Print(string input) => Console.WriteLine(input);
        public static int ConvertToInteger(string input) => Int32.TryParse(input, out int num) ? num : int.MinValue;
    }
}