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

            var processes = Process.GetProcesses();
            var uniqueProcesses = processes.DistinctBy(x => x.ProcessName);

            foreach (Process p in uniqueProcesses)
            {
                    listBox1.Items.Add(p.ProcessName + " - " + p.MainWindowTitle);
            }
        }
    }
}
