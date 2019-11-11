using JT.CommonUtils;
using JT.MusicApp;
using JT.WallpaperApp;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;

namespace JT
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly string JTApp = "JTApp";

        public MainWindow()
        {
            InitializeComponent();

            bool isExist = BootUpUtils.IsExistKey(JTApp);
            this.BootUp.IsChecked = isExist;

            this.StartMusic.IsChecked = IsStartMusic();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddTrayIcon();
        }
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        #region 任务栏小图标

        private static NotifyIcon trayIcon;
        private void RemoveTrayIcon()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }
        }
        private void AddTrayIcon()
        {
            if (trayIcon != null)
            {
                return;
            }
            trayIcon = new NotifyIcon
            {
                Icon = Properties.Resources.JT,
                Text = "个人辅助插件"
            };
            trayIcon.Visible = true;

            trayIcon.MouseClick += TrayIcon_MouseClick;

            ContextMenu menu = new ContextMenu();

            #region show app
            MenuItem showItem = new MenuItem();
            showItem.Text = "显示界面";
            showItem.Click += new EventHandler(delegate
            {
                this.Visibility = Visibility.Visible;
            });
            menu.MenuItems.Add(showItem);
            #endregion

            #region wallpaper


            MenuItem wallpaperItem = new MenuItem();
            wallpaperItem.Text = "动态壁纸";
            wallpaperItem.Click += new EventHandler(delegate
            {
                if (wallpaper == null)
                {
                    wallpaper = new Wallpaper();
                }
                wallpaper.Show();
                wallpaper.Activate();
            });
            menu.MenuItems.Add(wallpaperItem);
            #endregion


            #region close app
            MenuItem closeItem = new MenuItem();
            closeItem.Text = "退出";
            closeItem.Click += new EventHandler(delegate
            {
                RemoveTrayIcon();
                Environment.Exit(0);
            });

            menu.MenuItems.Add(closeItem);
            #endregion

            trayIcon.ContextMenu = menu;    //设置NotifyIcon的右键弹出菜单
        }


        private void TrayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        #endregion

        #region 动态壁纸
        private Wallpaper wallpaper;
        private void ShowWallpaper_Click(object sender, RoutedEventArgs e)
        {
            if (wallpaper == null)
            {
                wallpaper = new Wallpaper();
            }
            wallpaper.Show();
            wallpaper.Activate();
        }

        #endregion

        #region 悬浮框

        private FloatingBoxApp.FloatWindow floatWindow;
        private void ShowFloatApp_Click(object sender, RoutedEventArgs e)
        {
            if (floatWindow == null)
            {
                floatWindow = new FloatingBoxApp.FloatWindow();
            }
            floatWindow.Show();
            floatWindow.Activate();
        }

        #endregion

        #region 爬虫音乐

        private MusicApp.MusicWindow musicWindow;

        private void ShowMusic_Click(object sender, RoutedEventArgs e)
        {
            if (musicWindow == null)
            {
                musicWindow = new MusicApp.MusicWindow();
            }
            musicWindow.Show();
            musicWindow.Activate();
        }

        #endregion

        private void BootUp_Checked(object sender, RoutedEventArgs e)
        {
            SetBootUp();
        }

        private void BootUp_Unchecked(object sender, RoutedEventArgs e)
        {
            SetBootUp();
        }

        private void SetBootUp()
        {
            bool IsStart = this.BootUp.IsChecked == null ? false : this.BootUp.IsChecked.Value;
            string path = Process.GetCurrentProcess().MainModule.FileName;
            BootUpUtils.SelfRunning(IsStart, JTApp, path);
        }

        private void StartMusic_Checked(object sender, RoutedEventArgs e)
        {
            SetStart(true);
            StartWallpapaer();
        }

        private void StartMusic_Unchecked(object sender, RoutedEventArgs e)
        {
            SetStart(false);
        }

        private void SetStart(bool isStart)
        {
            MusicConfig musicConfig = new MusicConfig();
            musicConfig.SetStart(isStart);
        }

        private bool IsStartMusic()
        {
            MusicConfig musicConfig = new MusicConfig();
            var config = musicConfig.GetConfig();
            return config.IsStart == 1;
        }

        public void StartWallpapaer()
        {
            MusicConfig musicConfig = new MusicConfig();
            var config = musicConfig.GetConfig();
            if (wallpaper==null)
            {
                wallpaper = new Wallpaper();
            }
            wallpaper.SetWallpaper(config.Path, config.Volume);
        }

    }
}
