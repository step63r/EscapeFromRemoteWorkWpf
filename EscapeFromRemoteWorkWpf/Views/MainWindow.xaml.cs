using System;
using System.ComponentModel;
using System.Windows;

namespace EscapeFromRemoteWorkWpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            LoadWindowBounds();
        }

        /// <summary>
        /// Window.Closingのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveWindowBounds();
            if (Properties.Settings.Default.ExitAsMinimized)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;
                taskbarIcon.Visibility = Visibility.Visible;
            }
            else
            {
                e.Cancel = false;
            }
        }

        /// <summary>
        /// Window.StateChangedのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
            {
                ShowInTaskbar = true;
            }
        }

        /// <summary>
        /// ShowMainWindowMenuItem.Clickのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMainWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            taskbarIcon.Visibility = Visibility.Collapsed;
            ShowInTaskbar = true;
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// ExitMenuItem.Clickのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// ウィンドウの情報を保存する
        /// </summary>
        private void SaveWindowBounds()
        {
            var settings = Properties.Settings.Default;
            settings.WindowState = WindowState;
            // 最大化を解除する
            WindowState = WindowState.Normal;
            settings.Left = Left;
            settings.Top = Top;
            settings.Width = Width;
            settings.Height = Height;
            settings.Save();
        }

        /// <summary>
        /// ウィンドウの位置・サイズを復元する
        /// </summary>
        private void LoadWindowBounds()
        {
            var settings = Properties.Settings.Default;
            if (settings.Left >= 0 && settings.Left + settings.Width < SystemParameters.VirtualScreenWidth)
            {
                Left = settings.Left;
            }
            if (settings.Top >= 0 && settings.Top + settings.Height < SystemParameters.VirtualScreenHeight)
            {
                Top = settings.Top;
            }
            if (settings.Width > 0 && settings.Width <= SystemParameters.WorkArea.Width)
            {
                Width = settings.Width;
            }
            if (settings.Height > 0 && settings.Height <= SystemParameters.WorkArea.Height)
            {
                Height = settings.Height;
            }
            if (settings.WindowState == WindowState.Maximized)
            {
                Loaded += (sender, e) => WindowState = WindowState.Maximized;
            }
        }
    }
}
