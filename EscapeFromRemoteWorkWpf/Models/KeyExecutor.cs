using EscapeFromRemoteWorkWpf.Common;
using EscapeFromRemoteWorkWpf.Services;
using log4net;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromRemoteWorkWpf.Models
{
    /// <summary>
    /// キー操作系クラス
    /// </summary>
    /// <remarks>現在はキー入力の監視のみを行う</remarks>
    public class KeyExecutor : IExecutor
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
        /// 最後にキーフックされたタイムスタンプ
        /// </summary>
        private uint _lastHookTime = 0;
        /// <summary>
        /// スレッドローカルなランダムオブジェクト
        /// </summary>
        private readonly Random _random;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="minRandomSec">ランダム秒の最小値</param>
        /// <param name="maxRandomSec">ランダム秒の最大値</param>
        public KeyExecutor(int minRandomSec = 10, int maxRandomSec = 60)
        {
            // 設定値を取得する
            _minRandomSec = minRandomSec;
            _maxRandomSec = maxRandomSec;

            // スレッド状態管理クラスに追加する
            _threadStatusService.AddStatus(GetType().Name);

            // ランダムオブジェクトを取得する
            _random = RandomProvider.GetThreadRandom();

            // キーボードフックを開始する
            if (!KeyboardHook.IsHooking)
            {
                KeyboardHook.AddEvent(HookKeyboard);
                KeyboardHook.Start();
            }
        }
        #endregion

        #region ファイナライザ
        ~KeyExecutor()
        {
            // キーボードフックを終了する
            if (KeyboardHook.IsHooking)
            {
                KeyboardHook.Stop();
            }
        }
        #endregion

        #region メソッド
        /// <summary>
        /// キー操作を実行する
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                uint currentHookTimeStamp = _lastHookTime;
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

                    // 最後にキーフックされた時間が更新されているか
                    if (_lastHookTime > currentHookTimeStamp)
                    {
                        // 取得して終わり
                        currentHookTimeStamp = _lastHookTime;
                        _threadStatusService.SetStatus(GetType().Name, false);
                        _logger.Info($"キーボードは操作中: {_lastHookTime}");
                    }
                    else
                    {
                        // 取得して終わり
                        currentHookTimeStamp = _lastHookTime;
                        _threadStatusService.SetStatus(GetType().Name, true);
                        _logger.Info($"キーボードは操作されていない");
                    }
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// キーボードフック時の処理
        /// </summary>
        /// <param name="state"></param>
        private void HookKeyboard(ref KeyboardHook.KeyboardState state)
        {
            _lastHookTime = state.Time;
        }
        #endregion
    }
}
