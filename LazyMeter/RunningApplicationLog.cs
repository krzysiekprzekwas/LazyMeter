using System;

namespace LazyMeter
{
    public class RunningApplicationLog
    {
        public string ApplicationName { get; }
        public TimeSpan RunningTime { get; set; }

        public RunningApplicationLog(RunningApplication runningApplication)
        {
            ApplicationName = runningApplication.Name;
            RunningTime = new TimeSpan();
        }

        public void AddRunningTime(TimeSpan time)
        {
            RunningTime = RunningTime.Add(time);
        }
    }
}
