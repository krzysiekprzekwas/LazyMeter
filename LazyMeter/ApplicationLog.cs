using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using LazyMeter.Annotations;

namespace LazyMeter
{
    [Serializable]
    public class ApplicationLog : INotifyPropertyChanged
    {
        public ObservableCollection<ApplicationInstance> Members { get; set; }

        public string Name { get; set; }

        public string ProcessName { get; set; }

        [XmlIgnore]
        public TimeSpan RunningTime { get; set; }

        public ApplicationLog()
        {
            Members = new ObservableCollection<ApplicationInstance>();
        }

        public ApplicationLog(string processName)
        {
            ProcessName = processName;
            Name = processName;
            Members = new ObservableCollection<ApplicationInstance>();
        }

        [XmlElement("TimeSinceLastEvent")]
        public long TimeSinceLastEventTicks
        {
            get { return RunningTime.Ticks; }
            set { RunningTime = new TimeSpan(value); }
        }


        #region PropertyChanging

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
    }
}
