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
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using LazyMeter.Data;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.VisualBasic;

namespace LazyMeter
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> IgnoredProcessNames = new List<string> {"LogiOverlay"};
        private List<string> IgnoredNames = new List<string> {"FolderView", "Przełącznik zadań"};
        private List<string> IgnoredClasess = new List<string> {"Progman"};
        private DispatcherTimer timer;

        private static string LOG_PATH = "log.xml";

        public ObservableCollection<ApplicationLog> ApplicationLogList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                if (!File.Exists(LOG_PATH))
                    throw new FileNotFoundException();

                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ApplicationLog>));
                using (TextReader writer = new StreamReader(LOG_PATH))
                {
                    ApplicationLogList = (ObservableCollection<ApplicationLog>) serializer.Deserialize(writer);
                }
            }
            catch
            {

                ApplicationLogList = new ObservableCollection<ApplicationLog>();
            }

            ApplicationsTreeView.ItemsSource = ApplicationLogList;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            DispatcherTimer timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(10);
            timer2.Tick += timer_Tick2;
            timer2.Start();

            StopTimerMenu.IsEnabled = true;
            StartTimerMenu.IsEnabled = false;

            #region NotifyIcon

            var ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("icon.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
            
            #endregion
        }
        
        public SeriesCollection ChartValues { get; set; }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        private void timer_Tick2(object sender, EventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ApplicationLog>));
            using (TextWriter writer = new StreamWriter(LOG_PATH))
            {
                serializer.Serialize(writer, ApplicationLogList);
            }
        }

        bool FilterUnwantedApp(ApplicationInstance app)
        {
            return IgnoredProcessNames.Contains(app.ProcessName) || IgnoredNames.Contains(app.Title) ||
                   IgnoredClasess.Contains(app.ClassName);
        }

        void timer_Tick(object sender, EventArgs e)
        {

            var instances = GetApplicationInstances();

            listBox1.ItemsSource = instances.Select(x => x.Title);

            try
            {
                SetFocusedElementInfo(instances[0]);
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine(exception);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Console.WriteLine(exception);
            }

            var filteredInstances = instances.Where(x => !FilterUnwantedApp(x));

            foreach (var instance in filteredInstances)
            {

                if (ApplicationLogList.Any(x => x.ProcessName == instance.ProcessName))
                {
                    var log = ApplicationLogList.First(x => x.ProcessName == instance.ProcessName);

                    if (log.Members.Any(x => x.Title == instance.Title))
                    {
                        var item = log.Members.First(x => x.Title == instance.Title);
                        item.RunningTime = item.RunningTime + TimeSpan.FromSeconds(1);
                        log.RunningTime = new TimeSpan(log.Members.Sum(x => x.RunningTime.Ticks));
                    }
                    else
                    {
                        instance.Type = log.Type;
                        log.Members.Add(instance);
                    }
                }
                else
                {
                    var log = new ApplicationLog(instance.ProcessName);
                    log.Members.Add(instance);
                    ApplicationLogList.Add(log);
                }
            }

            ApplicationLogList = new ObservableCollection<ApplicationLog>(ApplicationLogList.OrderByDescending(x => x.FocusTime).ToList());

            lblRunningCount.Text = String.Format("Running apps: {0}", instances.Count);

            lblLogsCount.Text = String.Format("Logged apps: {0}", ApplicationLogList.Count);
        }

        private void SetFocusedElementInfo(ApplicationInstance focused)
        {
            var app = ApplicationLogList.First(x => x.Members.Any(y => y.Title == focused.Title));

            var item = app.Members.First(y => y.Title == focused.Title);

            item.FocusTime = item.FocusTime + TimeSpan.FromSeconds(1);

            app.FocusTime = new TimeSpan(app.Members.Sum(x => x.FocusTime.Ticks));

        }

        private List<ApplicationInstance> GetApplicationInstances()
        {
            AutomationElement rootElement = AutomationElement.RootElement;
            var children = GetChildren(rootElement);

            var apps = children.Where(x =>
                {
                    // Possible NullException when one off apps will close in meantime
                    try
                    {
                        return !string.IsNullOrWhiteSpace(x.Current.Name);
                    }
                    catch
                    {
                        return false;
                    }
                })
                .Select(x => new ApplicationInstance()
                {
                    Title = x.Current.Name,
                    ClassName = x.Current.ClassName,
                    ProcessName = Process.GetProcessById(x.Current.ProcessId).ProcessName,
                    Type = ActivityType.Other
                });

            return apps.ToList();

        }

        public List<AutomationElement> GetChildren(AutomationElement parent)
        {
            System.Windows.Automation.Condition findCondition =
                new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            AutomationElementCollection children = parent.FindAll(TreeScope.Children, findCondition);
            AutomationElement[] elementArray = new AutomationElement[children.Count];
            children.CopyTo(elementArray, 0);
            return elementArray.ToList();
        }

        private void DeleteApplicationLog_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mnu)
            {
                var sp = ((ContextMenu) mnu.Parent).PlacementTarget as StackPanel;

                var log = (ApplicationLog) sp.DataContext;

                ApplicationLogList.Remove(log);
            }
        }

        private void DeleteApplicationInstance_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mnu)
            {
                var sp = ((ContextMenu) mnu.Parent).PlacementTarget as StackPanel;

                var instance = (ApplicationInstance) sp.DataContext;

                var app = ApplicationLogList.First(x => x.Members.Contains(instance));

                app.Members.Remove(instance);

                app.RunningTime = new TimeSpan(app.Members.Sum(x => x.RunningTime.Ticks));
            }
        }

        private void RenameApplicationLog_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mnu)
            {
                var sp = ((ContextMenu) mnu.Parent).PlacementTarget as StackPanel;

                var log = (ApplicationLog) sp.DataContext;

                var newName = Interaction.InputBox("Provide new application name", "Rename " + log.ProcessName, log.Name);

                if (!string.IsNullOrWhiteSpace(newName))
                    log.Name = newName;
            }
        }

        private void SetTypeApplicationInstance_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mnu)
            {
                var sp = ((ContextMenu)((MenuItem)mnu.Parent).Parent).PlacementTarget as StackPanel;

                var instance = (ApplicationInstance)sp.DataContext;

                var type = (string)mnu.Header;

                switch (type)
                {
                    case "Fun":
                        instance.Type = ActivityType.Fun;
                        break;
                    case "Work":
                        instance.Type = ActivityType.Work;
                        break;
                    case "Univeristy":
                        instance.Type = ActivityType.Univeristy;
                        break;
                    case "Learning":
                        instance.Type = ActivityType.Learning;
                        break;
                    default:
                        instance.Type = ActivityType.Other;
                        break;
                }
            }
        }

        private void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ApplicationsTab.IsSelected)
               RefreshApplicationChart();
            else if (ActivityTab.IsSelected)
                RefreshActivityChart();
            
        }

        private void RefreshActivityChart()
        {
            var seriesColection = new SeriesCollection();
            
            var instances = ApplicationLogList.SelectMany(x => x.Members).GroupBy(x => x.Type).Select(g => new {
                Type = g.Key,
                Total = g.Sum(x => x.FocusTime.Ticks)
            });

            foreach (var instance in instances)
            {
                var series = new PieSeries
                {
                    Title = instance.Type.ToString(),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(instance.Total) },
                    DataLabels = true,
                    LabelPoint = a => TimeSpan.FromTicks((long)a.Y).ToString()
                };

                seriesColection.Add(series);
            }
            ActivityChart.Series = seriesColection;
        }

        private void RefreshApplicationChart()
        {
            var seriesColection = new SeriesCollection();

            foreach (var log in ApplicationLogList)
            {
                var series = new PieSeries
                {
                    Title = log.Name,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(log.FocusTime.Ticks) },
                    DataLabels = true,
                    LabelPoint = a => TimeSpan.FromTicks((long)a.Y).ToString()
                };

                seriesColection.Add(series);
            }
            Chart.Series = seriesColection;
        }

        private void SetTypeApplicationLog_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mnu)
            {
                var sp = ((ContextMenu)((MenuItem)mnu.Parent).Parent).PlacementTarget as StackPanel;

                var log = (ApplicationLog)sp.DataContext;

                var type = (string)mnu.Header;

                ActivityType typeValue;

                switch (type)
                {
                    case "Fun":
                        typeValue = ActivityType.Fun;
                        break;
                    case "Work":
                        typeValue = ActivityType.Work;
                        break;
                    case "Univeristy":
                        typeValue = ActivityType.Univeristy;
                        break;
                    case "Learning":
                        typeValue = ActivityType.Learning;
                        break;
                    default:
                        typeValue = ActivityType.Other;
                        break;
                }

                log.Type = typeValue;

                foreach (var instance in log.Members)
                {
                    if (instance.Type == ActivityType.Other)
                    {
                        instance.Type = typeValue;
                    }
                }
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                var tab = sender as TabItem;

                if ((string)tab.Header == "Applications")
                {
                    RefreshApplicationChart();
                }
                else if((string)tab.Header == "Activity")
                {
                    RefreshActivityChart();
                }
            }
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            StopTimerMenu.IsEnabled = false;
            StartTimerMenu.IsEnabled = true;
        }

        private void StartTimerMenu_OnClick(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            StopTimerMenu.IsEnabled = true;
            StartTimerMenu.IsEnabled = false;
        }
    }
}
