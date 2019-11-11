using JT.CommonUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace JT.FloatingBoxApp
{
    /// <summary>
    /// FloatWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FloatWindow : Window
    {
        private static bool isFixed = false;
        public FloatWindow()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.Topmost = true;

            Task.Factory.StartNew(() => 
            {
                MonitorNetStream();
                MonitorSystem();
            });
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!isFixed)
            {
                this.DragMove();
            }
        }

        private void Window_MouseLeftClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ShowRightMenus(sender);
            }
        }
        private void Window_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            ShowRightMenus(sender);
        }

        private void ShowRightMenus(object sender)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }

        private void MenuItem_CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_FixedClick(object sender, RoutedEventArgs e)
        {
            isFixed = true;
        }

        private void MenuItem_RemoveFixedClick(object sender, RoutedEventArgs e)
        {
            isFixed = false;
        }


        #region CPU


        private void MonitorSystem()
        {
            ParameterizedThreadStart ts = new ParameterizedThreadStart(MonitorSystemGet);
            Thread monitor = new Thread(ts);
            SystemInfo systemInfo = new SystemInfo();
            monitor.Start(systemInfo);
        }

        private void MonitorSystemGet(object obj)
        {
            SystemInfo systemInfo = (SystemInfo)obj;
            while (true)
            {
                Thread.Sleep(1000);
                showLabelSystemMsg(systemInfo.CpuLoad.ToString("F2") + "%", systemInfo.MemoryLoad().ToString("F2") + "%");
            }
        }

        private void showLabelSystemMsg(string cpuText,string memoryText)
        {
            this.LabCPU.Dispatcher.Invoke(new Action(() =>
            {
                this.LabCPU.Content = cpuText;
            }));
            this.LabMemory.Dispatcher.Invoke(new Action(() =>
            {
                this.LabMemory.Content = memoryText;
            }));
        }
        #endregion

        #region Net


        /// <summary>
        /// 网络监视
        /// </summary>
        private void MonitorNetStream()
        {
            List<PerformanceCounter> pcs = new List<PerformanceCounter>();
            List<PerformanceCounter> pcs2 = new List<PerformanceCounter>();
            string[] names = getAdapter();
            foreach (string name in names)
            {
                try
                {
                    PerformanceCounter pc = new PerformanceCounter("Network Interface", "Bytes Received/sec", name.Replace('(', '[').Replace(')', ']'), ".");
                    PerformanceCounter pc2 = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name.Replace('(', '[').Replace(')', ']'), ".");
                    pc.NextValue();
                    pcs.Add(pc);
                    pcs2.Add(pc2);
                }
                catch
                {
                    continue;
                }

            }
            ParameterizedThreadStart ts = new ParameterizedThreadStart(run);
            Thread monitor = new Thread(ts);
            List<PerformanceCounter>[] pcss = new List<PerformanceCounter>[2];
            pcss[0] = pcs;
            pcss[1] = pcs2;
            monitor.Start(pcss);
        }
        private void run(object obj)
        {
            List<PerformanceCounter>[] pcss = (List<PerformanceCounter>[])obj;
            List<PerformanceCounter> pcs = pcss[0];
            List<PerformanceCounter> pcs2 = pcss[1];
            while (true)
            {
                Thread.Sleep(500);
                long recv = 0;
                long sent = 0;
                foreach (PerformanceCounter pc in pcs)
                {
                    recv += Convert.ToInt32(pc.NextValue()) / 1000;
                }
                foreach (PerformanceCounter pc in pcs2)
                {
                    sent += Convert.ToInt32(pc.NextValue()) / 1000;
                }
                float up = (float)recv / (float)8 / (float)1024;
                float down = (float)sent / (float)8 / (float)1024;
                Console.WriteLine("up:" + recv + ",down:" + sent);
                showLabelNetMsg(up.ToString("F2") + "kb/s", down.ToString("F2") + "kb/s");
            }
        }

        private void showLabelNetMsg(string downText,string upText)
        {
            this.LabNetDown.Dispatcher.Invoke(new Action(() =>
            {
                this.LabNetDown.Content = downText;
            }));
            this.LabNetUp.Dispatcher.Invoke(new Action(() =>
            {
                this.LabNetUp.Content = upText;
            }));
        }
        private string[] getAdapter()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string[] name = new string[adapters.Length];
            int index = 0;
            foreach (NetworkInterface ni in adapters)
            {
                name[index] = ni.Description;
                index++;
            }
            return name;
        }

        #endregion


    }
}
