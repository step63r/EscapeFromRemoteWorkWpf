using EscapeFromRemoteWorkWpf.Common;
using System;
using System.Diagnostics;
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
        /// ランダム秒の最小値
        /// </summary>
        private readonly int _minRandomSec = 10;
        /// <summary>
        /// ランダム秒の最大値
        /// </summary>
        private readonly int _maxRandomSec = 60;
        /// <summary>
        /// カーソル座標が移動されたかどうかの誤差 (X, Y)
        /// </summary>
        private readonly (int X, int Y) _precision = (5, 5);
        /// <summary>
        /// 最後に検出されたカーソル座標 (X, Y)
        /// </summary>
        private (int? X, int? Y) _lastCursorPos = (null, null);
        #endregion

        /// <summary>
        /// マウス操作を実行する
        /// </summary>
        public async Task Execute()
        {
            await Task.Run(() =>
            {
                // TODO: CanellationToken
                while(true)
                {
                    int awaitSec = new Random().Next(_minRandomSec, _maxRandomSec);
                    Debug.WriteLine($"スレッドを {awaitSec} 秒待ちます...");
                    Thread.Sleep(awaitSec * 1000);

                    // 現在のカーソル座標を取得
                    NativeMethods.GetCursorPos(out var currentCursorPos);

                    // カーソル座標が一度も取得されていない
                    if (_lastCursorPos.X == null || _lastCursorPos.Y == null)
                    {
                        // 取得して終わり
                        _lastCursorPos = (currentCursorPos.X, currentCursorPos.Y);
                        Debug.WriteLine($"カーソル座標初期化: ({_lastCursorPos.X}, {_lastCursorPos.Y})");
                    }
                    else
                    {
                        // ある程度カーソルが動いているか
                        if (Math.Abs((int)_lastCursorPos.X - currentCursorPos.X) > _precision.X || 
                        Math.Abs((int)_lastCursorPos.Y - currentCursorPos.Y) > _precision.Y)
                        {
                            // 取得して終わり
                            _lastCursorPos = (currentCursorPos.X, currentCursorPos.Y);
                            Debug.WriteLine($"マウスは操作中: ({_lastCursorPos.X}, {_lastCursorPos.Y})");
                        }
                        else
                        {
                            // 画面上の適当な座標にカーソルを飛ばす
                            int nextPosX = new Random().Next(0, (int)SystemParameters.VirtualScreenWidth);
                            int nextPosY = new Random().Next(0, (int)SystemParameters.VirtualScreenHeight);
                            NativeMethods.SetCursorPos(nextPosX, nextPosY);
                            _lastCursorPos = (nextPosX, nextPosY);
                            Debug.WriteLine($"カーソル座標変更: ({nextPosX}, {nextPosY})");
                        }
                    }
                }
            }).ConfigureAwait(false);
        }
    }
}
