using System;

namespace WaterTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            WaterTrackerManager tracker = new WaterTrackerManager();
            tracker.InitializeDataBase();
            tracker.GetUserInput();
        }
    }
}