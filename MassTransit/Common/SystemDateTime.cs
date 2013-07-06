using System;

namespace Common
{
    public static class SystemTime
    {
        private static DateTime setTime = DateTime.MinValue;

        public static void Clear()
        {
            setTime = DateTime.MinValue;
        }

        public static void Set(DateTime toSet)
        {
            setTime = toSet;
        }
        public static DateTime GetTime()
        {
            if (setTime == DateTime.MinValue) return DateTime.Now;
            return setTime;
        }
    }
}
