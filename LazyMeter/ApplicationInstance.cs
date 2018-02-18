using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using LazyMeter.Annotations;

namespace LazyMeter
{
    [Serializable]
    public class ApplicationInstance : INotifyPropertyChanged
    {
        public string Title { get; set; }

        public string ProcessName { get; set; }

        [XmlIgnore]
        public TimeSpan RunningTime { get; set; }

        public ApplicationInstance()
        {
            RunningTime = new TimeSpan();
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
