using System;

namespace TestTaskAlt.Sortet
{
    internal static class MemoryInfo
    {
        static MemoryInfo()
        {
            Reset();
        }

        public static ulong MaximumMemoryAmount { get; private set; }

        public static void Reset()
        {
            var memoryStatus = new Hardware.Info.MemoryStatus();
            MaximumMemoryAmount = memoryStatus.AvailablePhysical;
        }

        public static float GetOccupiedMemoryPercent()
        {
            return (float) GC.GetTotalMemory(false)/MaximumMemoryAmount;
        }

        public static ulong GetFreeMemoryLeft()
        {
            return MaximumMemoryAmount - (ulong)GC.GetTotalMemory(false);
        }
    }
}