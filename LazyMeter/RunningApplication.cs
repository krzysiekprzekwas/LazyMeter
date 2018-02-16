using System.Diagnostics;

namespace LazyMeter
{
    public class RunningApplication
    {
        public string Name { get; }
        public Process Process { get; }
        public int ProcessID {
            get{ return Process.Id; }
        }

        public RunningApplication(string Name, int ProcessID)
        {
            this.Name = Name;
            this.Process = Process.GetProcessById(ProcessID);
        }

        public RunningApplication(string Name, Process Process)
        {
            this.Name = Name;
            this.Process = Process;
        }
    }
}
