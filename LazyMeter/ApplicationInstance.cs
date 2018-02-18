using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LazyMeter.Annotations;

namespace LazyMeter
{
    public class ApplicationInstance : INotifyPropertyChanged
    {
        public ApplicationInstance()
        {
            RunningTime = new TimeSpan();
        }

        public string Title { get; set; }

        public string ProcessName { get; set; }

        public TimeSpan RunningTime { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
