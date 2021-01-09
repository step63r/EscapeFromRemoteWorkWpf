using EscapeFromRemoteWorkWpf.Common;
using EscapeFromRemoteWorkWpf.Services;
using log4net;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EscapeFromRemoteWorkWpf.Models
{
    /// <summary>
    /// マウス操作系クラス
    /// </summary>
    public class MouseExecutor : IExecutor
    {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// スレッド状態管理クラス（シングルトン）
        /// </summary>
        private ThreadStatusService _threadStatusService = ThreadStatusService.GetInstance();
        /// <summary>
        /// ランダム秒の最小値
        /// </summary>
        private int _minRandomSec = 10;
        /// <summary>
        /// ランダム秒の最大値
        /// </summary>
        private int _maxRandomSec = 60;
        /// <summary>
        /// カーソル座標が移動されたかどうかの誤差 (X, Y)
        /// </summary>
        private (int X, int Y) _precision = (5, 5);
        /// <summary>
        /// 最後に検出されたカーソル座標 (X, Y)
        /// </summary>
        private (int? X, int? Y) _lastCursorPos = (null, null);
        /// <summary>
        /// スレッドローカルなランダムオブジェクト
        /// </summary>
        private readonly Random _random;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MouseExecutor(int minRandomSec = 10, int maxRandomSec = 60, int precision = 5)
        {
            // 設定値を取得する
            _minRandomSec = minRandomSec;
            _maxRandomSec = maxRandomSec;
            _precision = (precision, precision);

            // スレッド状態管理クラスに追加する
            _threadStatusService.AddStatus(GetType().Name);

            // ランダムオブジェクトを取得する
            _random = RandomProvider.GetThreadRandom();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// マウス操作を実行する
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
                    _logger.Info($"スレッドを {awaitSec} 秒待ちます...");
                    if (cancellationToken.WaitHandle.WaitOne(awaitSec * 1000))
                    {
                        _logger.Info("キャンセルされました");
                        // スレッド状態管理クラスから削除する
                        _threadStatusService.RemoveStatus(GetType().Name);
                        break;
                    }

                    // 現在のカーソル座標を取得
                    NativeMethods.GetCursorPos(out var currentCursorPos);

                    // カーソル座標が一度も取得されていない
                    if (_lastCursorPos.X == null || _lastCursorPos.Y == null)
                    {
                        // 取得して終わり
                        _lastCursorPos = (currentCursorPos.X, currentCursorPos.Y);
                        _threadStatusService.SetStatus(GetType().Name, true);
                        _logger.Info($"カーソル座標初期化: ({_lastCursorPos.X}, {_lastCursorPos.Y})");
                    }
                    else
                    {
                        // ある程度カーソルが動いているか
                        if (Math.Abs((int)_lastCursorPos.X - currentCursorPos.X) > _precision.X ||
                        Math.Abs((int)_lastCursorPos.Y - currentCursorPos.Y) > _precision.Y)
                        {
                            // 取得して終わり
                            _lastCursorPos = (currentCursorPos.X, currentCursorPos.Y);
                            _threadStatusService.SetStatus(GetType().Name, false);
                            _logger.Info($"マウスは操作中: ({_lastCursorPos.X}, {_lastCursorPos.Y})");
                        }
                        else
                        {
                            // 実行可能状態へ移行
                            _threadStatusService.SetStatus(GetType().Name, true);

                            // 実行中スレッドの全てのステータスがtrueであれば処理可能とみなす
                            if (!_threadStatusService.GetStatuses().Contains(false))
                            {
                                // 画面上の適当な座標にカーソルを飛ばす
                                int nextPosX = _random.Next(0, (int)SystemParameters.VirtualScreenWidth);
                                int nextPosY = _random.Next(0, (int)SystemParameters.VirtualScreenHeight);
                                NativeMethods.SetCursorPos(nextPosX, nextPosY);
                                _lastCursorPos = (nextPosX, nextPosY);
                                _logger.Info($"カーソル座標変更: ({nextPosX}, {nextPosY})");
                            }
                            else
                            {
                                _logger.Info($"実行可能状態でないスレッドがあるのでスキップ");
                            }
                        }
                    }
                }
            }).ConfigureAwait(false);
        }
        #endregion
    }
}
