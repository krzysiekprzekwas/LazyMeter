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

        public string ClassName { get; set; }

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

        public ApplicationInstance()
        {
            RunningTime = new TimeSpan();
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
