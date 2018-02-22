using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using LazyMeter.Annotations;
using LazyMeter.Data;

namespace LazyMeter
{
    [Serializable]
    public class ApplicationLog : INotifyPropertyChanged
    {
        public ObservableCollection<ApplicationInstance> Members { get; set; }

        public string Name { get; set; }

        public string ProcessName { get; set; }
        
        public ActivityType Type { get; set; }

        [XmlIgnore]
        public TimeSpan RunningTime { get; set; }
        
        [XmlElement("RunningTime")]
        public long RunningTimeTicks
        {
            get { return RunningTime.Ticks; }
            set { RunningTime = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan FocusTime { get; set; }

        [XmlElement("FocusTime")]
        public long FocusTimeTicks
        {
            get { return FocusTime.Ticks; }
            set { FocusTime = new TimeSpan(value); }
        }

        public ApplicationLog()
        {
            Members = new ObservableCollection<ApplicationInstance>();
            Type = ActivityType.Other;
        }

        public ApplicationLog(string processName)
        {
            ProcessName = processName;
            Name = processName;
            Type = ActivityType.Other;
            Members = new ObservableCollection<ApplicationInstance>();
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
