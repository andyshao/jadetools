using JT.CommonUtils.Music.Domain;
using JT.CommonUtils.Music.Http;
using JT.CommonUtils.Music.Provider;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JT.MusicApp
{
    /// <summary>
    /// MusicWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MusicWindow : MetroWindow
    {
        public MusicWindow()
        {
            InitializeComponent();

            txtSearch.Text = "周杰伦";

            progressState.Visibility = Visibility.Hidden;

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private static int page = 1;
        MusicProviders provider = MusicProviders.Instance;

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                return;
            }
            page = 1;
            GetList(page);
        }


        /// <summary>
        /// 开始显示进度栏动画
        /// </summary>
        private void StartProcessBar()
        {
            progressState.Visibility = Visibility.Visible;
            progressState.Value = 0;
        }

        /// <summary>
        /// 结束显示进度栏动画
        /// </summary>
        private void StopProcessBar()
        {
            progressState.Visibility = Visibility.Hidden;
            progressState.Value = 0;
        }

        /// <summary>
        /// 获取歌曲列表
        /// </summary>
        /// <param name="page"></param>
        private void GetList(int page)
        {
            StartProcessBar();
            this.labPageIndex.Content = "第 " + page + " 页";
            var songs = provider.SearchSongs(txtSearch.Text, page, 20);

            resultListView.Items.Clear();
            this.resultListView.Dispatcher.Invoke(new Action(() =>
            {
                songs.ForEach(item =>
                {
                    this.resultListView.Items.Add(item);
                });
                this.nextPage.IsEnabled = songs.Count > 0;
            }));
        }

        private void BtnSelectFilePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selectedPath = dialog.SelectedPath;
                this.txtDownloadFilePah.Text = selectedPath;
            }
        }
        SongDownloader downloader;
        private void DownLoadMusic_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var target = this.txtDownloadFilePah.Text;
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                if (downloader == null)
                {
                    downloader = new SongDownloader(provider, target);
                }

                List<MergedSong> selectedData = new List<MergedSong>();
                for (int i = 0; i < resultListView.Items.Count; i++)
                {
                    var res = (MergedSong)resultListView.Items[i];
                    if (res.IsChecked)
                    {
                        selectedData.Add(res);
                    }
                }

                foreach (var item in selectedData)
                {
                    downloader.AddDownload(item);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void PrePage_Click(object sender, RoutedEventArgs e)
        {
            if (page > 1)
            {
                page--;
                GetList(page);

                if (page == 1)
                {
                    this.prePage.IsEnabled = false;
                    this.nextPage.IsEnabled = true;
                }
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            page++;
            GetList(page);
            if (page > 1)
            {
                this.prePage.IsEnabled = true;
                //this.nextPage.IsEnabled = false;
            }
        }
    }
}
