using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using LazyMeter.Annotations;

namespace LazyMeter
{
    [Serializable]
    public class ApplicationInstance : INotifyPropertyChanged
    {
        public ApplicationInstance()
        {
            RunningTime = new TimeSpan();
        }

        public string Title { get; set; }

        public string ProcessName { get; set; }

        [XmlIgnore]
        public TimeSpan RunningTime { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement("TimeSinceLastEvent")]
        public long TimeSinceLastEventTicks
        {
            get { return RunningTime.Ticks; }
            set { RunningTime = new TimeSpan(value); }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
