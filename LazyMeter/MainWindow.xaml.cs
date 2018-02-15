using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Automation;

namespace LazyMeter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            
            ShowApplicationNames();
        }

        private void ShowApplicationNames()
        {
            AutomationElement rootElement = AutomationElement.RootElement;
            var children = GetChildren(rootElement);


            var apps = children.Where(x => !string.IsNullOrWhiteSpace(x.Current.Name)).Select(x=>x);

            foreach (AutomationElement elemnt in apps)
            {
                
                var listboxitem = new ListBoxItem();
                listboxitem.Content = elemnt.Current.Name + " - " + elemnt.Current.ProcessId;
                listBox1.Items.Add(listboxitem);
                
            }
            
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
