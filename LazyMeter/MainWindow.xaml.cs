﻿using MoreLinq;
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

namespace LazyMeter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public ObservableCollection<RunningApplicationLog> RunningApplicationLogList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            RunningApplicationLogList = new ObservableCollection<RunningApplicationLog>();
            listBox2.ItemsSource = RunningApplicationLogList;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void timer_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            SetFocusedElementInfo(AutomationElement.FocusedElement);

            var applications = GetApplications();

            lblRunningCount.Text = String.Format("Running apps: {0}",applications.Count);

            foreach (RunningApplication elemnt in applications)
            {

                var listboxitem = new ListBoxItem();

                listboxitem.Content = elemnt.Name + " - "  + elemnt.Process.ProcessName;
                listBox1.Items.Add(listboxitem);

                if (RunningApplicationLogList.Any(x=>x.ApplicationName == elemnt.Process.ProcessName))
                {
                    var item = RunningApplicationLogList.Where(x => x.ApplicationName == elemnt.Process.ProcessName).FirstOrDefault();
                    item.AddRunningTime(TimeSpan.FromSeconds(1));
                }
                else
                {
                    RunningApplicationLogList.Add(new RunningApplicationLog(elemnt));
                }
            }

            lblLogsCount.Text = String.Format("Logged apps: {0}", RunningApplicationLogList.Count);
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

        private List<RunningApplication> GetApplications()
        {
            AutomationElement rootElement = AutomationElement.RootElement;
            var children = GetChildren(rootElement);

            var apps = children.Where(x => !string.IsNullOrWhiteSpace(x.Current.Name))
                               .DistinctBy(x=>x.Current.ProcessId)
                               .Select(x=> new RunningApplication(x.Current.Name,x.Current.ProcessId));

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
