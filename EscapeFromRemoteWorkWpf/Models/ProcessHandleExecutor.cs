using EscapeFromRemoteWorkWpf.Common;
using EscapeFromRemoteWorkWpf.Extensions;
using EscapeFromRemoteWorkWpf.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromRemoteWorkWpf.Models
{
    /// <summary>
    /// プロセスハンドル操作系クラス
    /// </summary>
    public class ProcessHandleExecutor : IExecutor
    {
        #region メンバ変数
        /// <summary>
        /// スレッド状態管理クラス（シングルトン）
        /// </summary>
        private ThreadStatusService _threadStatusService = ThreadStatusService.GetInstance();
        /// <summary>
        /// ランダム秒の最小値
        /// </summary>
        private readonly int _minRandomSec = 10;
        /// <summary>
        /// ランダム秒の最大値
        /// </summary>
        private readonly int _maxRandomSec = 60;
        /// <summary>
        /// 最後に検出されたアクティブウィンドウのプロセスID
        /// </summary>
        private int? _lastWindowProcessId = null;
        /// <summary>
        /// 全てのウィンドウハンドル
        /// </summary>
        private static List<IntPtr> _currentWindowHandles = new List<IntPtr>();
        /// <summary>
        /// スレッドローカルなランダムオブジェクト
        /// </summary>
        private readonly Random _random;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProcessHandleExecutor(int minRandomSec = 10, int maxRandomSec = 60)
        {
            // 設定値を取得する
            _minRandomSec = minRandomSec;
            _maxRandomSec = maxRandomSec;

            // スレッド状態管理クラスに追加する
            _threadStatusService.AddStatus(GetType().Name);

            // ランダムオブジェクトを取得する
            _random = RandomProvider.GetThreadRandom();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// プロセスハンドル操作を実行する
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    int awaitSec = _random.Next(_minRandomSec, _maxRandomSec);
                    Debug.WriteLine($"スレッドを {awaitSec} 秒待ちます...");
                    if (cancellationToken.WaitHandle.WaitOne(awaitSec * 1000))
                    {
                        Debug.WriteLine("キャンセルされました");
                        // スレッド状態管理クラスから削除する
                        _threadStatusService.RemoveStatus(GetType().Name);
                        break;
                    }

                    // 現在のアクティブウィンドウのプロセスIDを取得
                    IntPtr ptr = NativeMethods.GetForegroundWindow();
                    _ = NativeMethods.GetWindowThreadProcessId(ptr, out int currentProcessId);

                    // プロセスIDが一度も取得されていない
                    if (_lastWindowProcessId == null)
                    {
                        // 取得して終わり
                        _lastWindowProcessId = currentProcessId;
                        _threadStatusService.SetStatus(GetType().Name, true);
                        Debug.WriteLine($"アクティブウィンドウ初期化: {_lastWindowProcessId}");
                    }
                    else
                    {
                        // アクティブウィンドウが変更されているか
                        if (_lastWindowProcessId != currentProcessId)
                        {
                            // 取得して終わり
                            _lastWindowProcessId = currentProcessId;
                            _threadStatusService.SetStatus(GetType().Name, false);
                            Debug.WriteLine($"アクティブウィンドウが変更されている: {_lastWindowProcessId}");
                        }
                        else
                        {
                            // 実行可能状態へ移行
                            _threadStatusService.SetStatus(GetType().Name, true);

                            // 実行中スレッドの全てのステータスがtrueであれば処理可能とみなす
                            if (!_threadStatusService.GetStatuses().Contains(false))
                            {
                                _currentWindowHandles = new List<IntPtr>();
                                NativeMethods.EnumWindows(EnumrateWindows, IntPtr.Zero);
                                if (_currentWindowHandles.Count > 0)
                                {
                                    // 適当なハンドルを取得してアクティブにする
                                    IntPtr nextHandle = _currentWindowHandles.GetAtRandom();
                                    _ = NativeMethods.GetWindowThreadProcessId(nextHandle, out int nextProcessId);
                                    NativeMethods.SetForegroundWindow(nextHandle);
                                    _lastWindowProcessId = nextProcessId;
                                    Debug.WriteLine($"アクティブウィンドウ変更: {nextProcessId}");
                                }
                                else
                                {
                                    Debug.WriteLine($"ウィンドウが1つもないので処理スキップ");
                                }
                            }
                        }
                    }
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// ウィンドウを列挙するコールバック関数
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lParam">アプリケーション定義の値</param>
        /// <returns></returns>
        private static bool EnumrateWindows(IntPtr hWnd, IntPtr lParam)
        {
            // 可視状態にあり、かつウィンドウ名が空文字でないハンドルに絞り込む
            if (NativeMethods.IsWindowVisible(hWnd) &&
                NativeMethods.GetWindowTextLength(hWnd) != 0)
            {
                _currentWindowHandles.Add(hWnd);
            }
            return true;
        }
        #endregion
    }
}
