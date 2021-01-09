using EscapeFromRemoteWorkWpf.Common;
using EscapeFromRemoteWorkWpf.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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

        private DateTime _startTime = new DateTime(2000, 1, 1, 8, 50, 0);
        /// <summary>
        /// 開始時刻
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                SetProperty(ref _startTime, value);
                Properties.Settings.Default.StartTime = value;
            }
        }

        private bool _isStartManually = true;
        /// <summary>
        /// 開始時刻を手動で制御
        /// </summary>
        public bool IsStartManually
        {
            get
            {
                return _isStartManually;
            }
            set
            {
                SetProperty(ref _isStartManually, value);
                Properties.Settings.Default.IsStartManually = value;
            }
        }

        private DateTime _endTime = new DateTime(2000, 1, 1, 17, 35, 0);
        /// <summary>
        /// 終了時刻
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                SetProperty(ref _endTime, value);
                Properties.Settings.Default.EndTime = value;
            }
        }

        private bool _isEndManually = false;
        /// <summary>
        /// 終了時刻を手動で制御
        /// </summary>
        public bool IsEndManually
        {
            get
            {
                return _isEndManually;
            }
            set
            {
                SetProperty(ref _isEndManually, value);
                Properties.Settings.Default.IsEndManually = value;
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

        private ObservableCollection<string> _targetProcesses = new ObservableCollection<string>();
        /// <summary>
        /// フォーカスを当てる対象プロセス名
        /// </summary>
        /// <remarks>空の場合は実行中の全てのプロセスが対象となる</remarks>
        public ObservableCollection<string> TargetProcesses
        {
            get
            {
                return _targetProcesses;
            }
            set
            {
                SetProperty(ref _targetProcesses, value);
                Properties.Settings.Default.TargetProcesses = value;
            }
        }

        private string _selectedProcess = "";
        /// <summary>
        /// 選択されたプロセス名
        /// </summary>
        public string SelectedProcess
        {
            get
            {
                return _selectedProcess;
            }
            set
            {
                SetProperty(ref _selectedProcess, value);
                InputProcess = value;
            }
        }

        private string _inputProcess = "";
        /// <summary>
        /// 入力中のプロセス名
        /// </summary>
        public string InputProcess
        {
            get
            {
                return _inputProcess;
            }
            set
            {
                SetProperty(ref _inputProcess, value);
            }
        }

        private bool _isRunning = false;
        /// <summary>
        /// 制御が実行中かどうか
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                SetProperty(ref _isRunning, value);
                RunCommand.RaiseCanExecuteChanged();
                SuspendCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 実行コマンド
        /// </summary>
        public DelegateCommand RunCommand { get; private set; }
        /// <summary>
        /// 停止コマンド
        /// </summary>
        public DelegateCommand SuspendCommand { get; private set; }
        /// <summary>
        /// 対象プロセス一覧に追加コマンド
        /// </summary>
        public DelegateCommand AddToTargetProcessesCommand { get; private set; }
        /// <summary>
        /// 対象プロセス一覧から削除コマンド
        /// </summary>
        public DelegateCommand RemoveFromTargetProcessesCommand { get; private set; }
        #endregion

        #region メンバ変数
        /// <summary>
        /// ディスパッチャタイマー
        /// </summary>
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;
        /// <summary>
        /// CancellationToken
        /// </summary>
        private CancellationToken _cancellationToken;
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
            StartTime = Properties.Settings.Default.StartTime;
            IsStartManually = Properties.Settings.Default.IsStartManually;
            EndTime = Properties.Settings.Default.EndTime;
            IsEndManually = Properties.Settings.Default.IsEndManually;
            TargetProcesses = Properties.Settings.Default.TargetProcesses ?? new ObservableCollection<string>();

            // コマンド登録
            RunCommand = new DelegateCommand(ExecuteRunCommand, CanExecuteRunCommand);
            SuspendCommand = new DelegateCommand(ExecuteSuspendCommand, CanExecuteSuspendCommand);
            AddToTargetProcessesCommand = new DelegateCommand(ExecuteAddToTargetProcessesCommand, CanExecuteAddToTargetProcessesCommand);
            RemoveFromTargetProcessesCommand = new DelegateCommand(ExecuteRemoveFromTargetProcessesCommand, CanExecuteRemoveFromTargetProcessesCommand);

            // タイマー開始
            _dispatcherTimer.Tick += OnDispatcherTimerTicked;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Start();
        }

        /// <summary>
        /// ファイナライザ
        /// </summary>
        ~MainWindowViewModel()
        {
            _dispatcherTimer.Tick -= OnDispatcherTimerTicked;
        }

        /// <summary>
        /// 実行コマンドを実行する
        /// </summary>
        private void ExecuteRunCommand()
        {
            // キャンセルトークン生成
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            // プロセス生成
            var mouseExecutor = new MouseExecutor(MouseMinRandomSec, MouseMaxRandomSec, MousePrecision);
            _ = mouseExecutor.ExecuteAsync(_cancellationToken);
            var processHandleExecutor = new ProcessHandleExecutor(ProcessHandleMinRandomSec, ProcessHandleMaxRandomSec, new List<string>(TargetProcesses));
            _ = processHandleExecutor.ExecuteAsync(_cancellationToken);

            IsRunning = true;
        }
        /// <summary>
        /// 実行コマンドが実行可能かどうか
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteRunCommand()
        {
            return !IsRunning;
        }

        /// <summary>
        /// 停止コマンドを実行する
        /// </summary>
        private void ExecuteSuspendCommand()
        {
            _cancellationTokenSource.Cancel();
            IsRunning = false;
        }
        /// <summary>
        /// 停止コマンドが実行可能かどうか
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteSuspendCommand()
        {
            return IsRunning;
        }

        /// <summary>
        /// 対象プロセス一覧に追加コマンドを実行する
        /// </summary>
        private void ExecuteAddToTargetProcessesCommand()
        {
            TargetProcesses.Add(InputProcess);
            InputProcess = "";
        }
        /// <summary>
        /// 対象プロセス一覧に追加コマンドが実行可能かどうか
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteAddToTargetProcessesCommand()
        {
            return !(string.IsNullOrEmpty(InputProcess) || TargetProcesses.Contains(InputProcess));
        }

        /// <summary>
        /// 対象プロセス一覧から削除コマンドを実行する
        /// </summary>
        private void ExecuteRemoveFromTargetProcessesCommand()
        {
            TargetProcesses.Remove(SelectedProcess);
            SelectedProcess = "";
        }
        /// <summary>
        /// 対象プロセス一覧から削除コマンドが実行可能かどうか
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteRemoveFromTargetProcessesCommand()
        {
            return !string.IsNullOrEmpty(SelectedProcess) && TargetProcesses.Contains(SelectedProcess);
        }

        /// <summary>
        /// ディスパッチャタイマーのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDispatcherTimerTicked(object sender, EventArgs e)
        {
            // 起動処理
            if (!IsRunning)
            {
                if (!IsStartManually && TimeSpan.Compare(DateTime.Now.TimeOfDay, StartTime.TimeOfDay) > 0)
                {
                    Debug.WriteLine("設定の時間内のため自動で処理を開始します");
                    ExecuteRunCommand();
                }
            }

            // 終了処理
            if (IsRunning)
            {
                if (!IsEndManually && TimeSpan.Compare(DateTime.Now.TimeOfDay, EndTime.TimeOfDay) > 0)
                {
                    Debug.WriteLine("設定の時間外のため自動で処理を終了します");
                    ExecuteSuspendCommand();
                }
            }
        }
    }
}
