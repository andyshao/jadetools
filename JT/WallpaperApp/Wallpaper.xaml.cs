using MahApps.Metro.Controls;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace JT.WallpaperApp
{
    /// <summary>
    /// Wallpaper.xaml 的交互逻辑
    /// </summary>
    public partial class Wallpaper : MetroWindow
    {
        public Wallpaper()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;

            media.UnloadedBehavior = MediaState.Manual;
            media.Volume = sliderVolume.Value / 100;
            

            fullWindow = new WallpaperFullWindow();
            fullWindow.Visibility = Visibility.Hidden;
        }

        // 壁纸窗口
        private WallpaperFullWindow fullWindow;

        private string currentAudioPath;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }
        private void MetroWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MediaState mediaState = GetMediaState(media);
            if (mediaState == MediaState.Play)
            {
                media.Pause();
            }
            else
            {
                media.Play();
            }
        }

        /// <summary>
        /// 进度拖拽完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sldProgress_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            media.Volume = sliderVolume.Value / 100;
            if (btnFull.IsChecked == true)
            {
                if (fullWindow != null)
                {
                    fullWindow.ChangeVolume(sliderVolume.Value / 100);
                }
            }

        }

        /// <summary>
        /// 进度条拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sldProgress_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            media.Volume = sliderVolume.Value / 100;
            if (btnFull.IsChecked == true)
            {
                if (fullWindow != null)
                {
                    fullWindow.ChangeVolume(sliderVolume.Value / 100);
                }
            }
        }

        //---------------------------  开关事件  --------------------------------


        private void btnFull_Checked(object sender, RoutedEventArgs e)
        {
            if (fullWindow != null)
            {
                fullWindow.Opacity = 1;
                fullWindow.ChangeVolume(sliderVolume.Value / 100);
                fullWindow.Visibility = Visibility.Visible;
            }
            if (sliderVolume != null)
            {
                media.Volume = sliderVolume.Value / 100;
            }
        }

        private void btnFull_Unchecked(object sender, RoutedEventArgs e)
        {
            fullWindow.Opacity = 0;
            media.Volume = 0;
            if (fullWindow != null)
            {
                fullWindow.ChangeVolume(0);
                fullWindow.Visibility = Visibility.Hidden;
            }
        }


        //---------------------------------------------------------------------


        //-------------------------- 事件处理 -----------------------------

        /// <summary>
        /// 设置壁纸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_SetClick(object sender, RoutedEventArgs e)
        {
            media.Stop();
            if (this.btnFull.IsChecked == true)
            {
                if (currentAudioPath != null)
                {
                    SetWallpaper(currentAudioPath, sliderVolume.Value);
                }
            }
        }

        public void SetWallpaper(string path,double sliderVolume)
        {
            fullWindow.ChangeSource(new Uri(path));
            fullWindow.ChangeVolume(sliderVolume / 100);
            fullWindow.Visibility = Visibility.Visible;
            fullWindow.Opacity = 1;
            fullWindow.Show();
        }


        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void Button_OpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "视频|*.mp4;*.wmv";
            openFileDialog.Multiselect = false;
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                currentAudioPath = openFileDialog.FileName;
                media.Stop();
                media.Source = new Uri(currentAudioPath);
                media.Play();
                //isPlay = true;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void media_MouseDown(object sender, MouseButtonEventArgs e)
        {
            media.Pause();
        }
        //---------------------------------------------------------------------
    }
}
