using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Automation;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LazyMeter
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> IgnoredProcessNames = new List<string> { "LogiOverlay" };
        private List<string> IgnoredNames = new List<string> { "FolderView", "Program Manager" };

        public ObservableCollection<ApplicationLog> ApplicationLogList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ApplicationLogList = new ObservableCollection<ApplicationLog>();

            trvFamilies.ItemsSource = ApplicationLogList;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        bool FilterUnwantedApp(ApplicationInstance app)
        {
            return IgnoredProcessNames.Contains(app.ProcessName) || IgnoredNames.Contains(app.Title);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            SetFocusedElementInfo(AutomationElement.FocusedElement);
            
            var instances = GetApplicationInstances();

            foreach (var instance in instances)
            {

                listBox1.Items.Add(instance.Title);

                if (FilterUnwantedApp(instance))
                    continue;

                if (ApplicationLogList.Any(x => x.ProcessName == instance.ProcessName))
                {
                    var log = ApplicationLogList.First(x => x.ProcessName == instance.ProcessName);

                    if (log.Members.Any(x=> x.Title == instance.Title))
                    {
                        var item = log.Members.First(x => x.Title == instance.Title);
                        item.RunningTime = item.RunningTime + TimeSpan.FromSeconds(1);
                        log.RunningTime = new TimeSpan(log.Members.Sum(x=>x.RunningTime.Ticks));
                    }
                    else
                    {
                        log.Members.Add(instance);
                    }
                }
                else
                {
                    var family2 = new ApplicationLog(instance.ProcessName);
                    family2.Members.Add(new ApplicationInstance() { Title = instance.Title, RunningTime = new TimeSpan() });
                    ApplicationLogList.Add(family2);
                }
            }

            lblRunningCount.Text = String.Format("Running apps: {0}", instances.Count);

            lblLogsCount.Text = String.Format("Logged apps: {0}", ApplicationLogList.Count);
        }

        private void SetFocusedElementInfo(AutomationElement focused)
        {
            if (focused != null)
            {
                string name = focused.Current.Name;
                int processId = focused.Current.ProcessId;
                using (Process process = Process.GetProcessById(processId))
                {
                    focusedAppTextBlock.Text = String.Format("  Name: {0}, Process: {1}", name, process.ProcessName);
                }
            }
        }

        private List<ApplicationInstance> GetApplicationInstances()
        {
            AutomationElement rootElement = AutomationElement.RootElement;
            var children = GetChildren(rootElement);

            var apps = children.Where(x => !string.IsNullOrWhiteSpace(x.Current.Name))
                .Select(x => new ApplicationInstance()
                {
                    Title = x.Current.Name,
                    ProcessName = Process.GetProcessById(x.Current.ProcessId).ProcessName
                });

            return apps.ToList();

        }

        public List<AutomationElement> GetChildren(AutomationElement parent)
        {
            System.Windows.Automation.Condition findCondition = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            AutomationElementCollection children = parent.FindAll(TreeScope.Children, findCondition);
            AutomationElement[] elementArray = new AutomationElement[children.Count];
            children.CopyTo(elementArray, 0);
            return elementArray.ToList();
        }
    }
}
