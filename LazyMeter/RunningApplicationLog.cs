using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LazyMeter
{
    public class RunningApplicationLog : INotifyPropertyChanged
    {
        public string ApplicationTitle { get; }
        public string ApplicationName { get; }
        public TimeSpan RunningTime { get; set; }

        public RunningApplicationLog(RunningApplication runningApplication)
        {
            ApplicationTitle = runningApplication.Name;
            ApplicationName = runningApplication.Process.ProcessName;
            RunningTime = new TimeSpan();
        }

        public void AddRunningTime(TimeSpan time)
        {
            RunningTime = RunningTime.Add(time);
        }

        public event PropertyChangedEventHandler PropertyChanged;
  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
