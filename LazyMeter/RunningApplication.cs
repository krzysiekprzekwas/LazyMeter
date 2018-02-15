namespace LazyMeter
{
    class RunningApplication
    {
        public string Name { get; set; }
        public int ProcessID { get; set; }

        public RunningApplication(string Name, int ProcessID)
        {
            this.Name = Name;
            this.ProcessID = ProcessID;
        }
    }
}
