using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LazyMeter.Annotations;

namespace LazyMeter
{
    public class ApplicationLog : INotifyPropertyChanged
    {
        public ApplicationLog()
        {
            this.Members = new ObservableCollection<ApplicationInstance>();
        }

        public ApplicationLog(string processName)
        {
            ProcessName = processName;
            Name = processName;
            Members = new ObservableCollection<ApplicationInstance>();
        }

        public string Name { get; set; }
        public string ProcessName { get; set; }

        public TimeSpan RunningTime { get; set; }

        public ObservableCollection<ApplicationInstance> Members { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
