using EscapeFromRemoteWorkWpf.Common;
using EscapeFromRemoteWorkWpf.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

namespace EscapeFromRemoteWorkWpf.ViewModels
{
    /// <summary>
    /// MainWindowのViewModelクラス
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        #region コマンド・プロパティ
        private string _title = "EscapeFromRemoteWorkWpf";
        /// <summary>
        /// Windowタイトル
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private Visibility _taskbarVisibility = Visibility.Collapsed;
        /// <summary>
        /// 通知領域の表示状態
        /// </summary>
        public Visibility TaskbarVisibility
        {
            get
            {
                return _taskbarVisibility;
            }
            set
            {
                SetProperty(ref _taskbarVisibility, value);
            }
        }

        private bool _exitAsMinimized = false;
        /// <summary>
        /// ウィンドウを閉じる代わりに通知領域に格納する
        /// </summary>
        public bool ExitAsMinimized
        {
            get
            {
                return _exitAsMinimized;
            }
            set
            {
                SetProperty(ref _exitAsMinimized, value);
                Properties.Settings.Default.ExitAsMinimized = value;
            }
        }

        private int _mouseMinRandomSec = 10;
        /// <summary>
        /// マウス操作のウェイト時間（最小）
        /// </summary>
        public int MouseMinRandomSec
        {
            get
            {
                return _mouseMinRandomSec;
            }
            set
            {
                SetProperty(ref _mouseMinRandomSec, value);
                Properties.Settings.Default.MouseMinRandomSec = value;
            }
        }

        private int _mouseMaxRandomSec = 60;
        /// <summary>
        /// マウス操作のウェイト時間（最大）
        /// </summary>
        public int MouseMaxRandomSec
        {
            get
            {
                return _mouseMaxRandomSec;
            }
            set
            {
                SetProperty(ref _mouseMaxRandomSec, value);
                Properties.Settings.Default.MouseMaxRandomSec = value;
            }
        }

        private int _mousePrecision = 5;
        /// <summary>
        /// マウス操作の許容誤差
        /// </summary>
        public int MousePrecision
        {
            get
            {
                return _mousePrecision;
            }
            set
            {
                SetProperty(ref _mousePrecision, value);
                Properties.Settings.Default.MousePrecision = value;
            }
        }

        private int _processHandleMinRandomSec = 10;
        /// <summary>
        /// プロセスハンドル操作のウェイト時間（最小）
        /// </summary>
        public int ProcessHandleMinRandomSec
        {
            get
            {
                return _processHandleMinRandomSec;
            }
            set
            {
                SetProperty(ref _processHandleMinRandomSec, value);
                Properties.Settings.Default.ProcessHandleMinRandomSec = value;
            }
        }

        private int _processHandleMaxRandomSec = 60;
        /// <summary>
        /// プロセスハンドル操作のウェイト時間（最大）
        /// </summary>
        public int ProcessHandleMaxRandomSec
        {
            get
            {
                return _processHandleMaxRandomSec;
            }
            set
            {
                SetProperty(ref _processHandleMaxRandomSec, value);
                Properties.Settings.Default.ProcessHandleMaxRandomSec = value;
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            // アプリケーション設定をロード
            ExitAsMinimized = Properties.Settings.Default.ExitAsMinimized;
            MouseMinRandomSec = Properties.Settings.Default.MouseMinRandomSec;
            MouseMaxRandomSec = Properties.Settings.Default.MouseMaxRandomSec;
            MousePrecision = Properties.Settings.Default.MousePrecision;
            ProcessHandleMinRandomSec = Properties.Settings.Default.ProcessHandleMinRandomSec;
            ProcessHandleMaxRandomSec = Properties.Settings.Default.ProcessHandleMaxRandomSec;

            var mouseExecutor = new MouseExecutor(MouseMinRandomSec, MouseMaxRandomSec, MousePrecision);
            _ = mouseExecutor.ExecuteAsync();

            var processHandleExecutor = new ProcessHandleExecutor(ProcessHandleMinRandomSec, ProcessHandleMaxRandomSec);
            _ = processHandleExecutor.ExecuteAsync();
        }
    }
}
