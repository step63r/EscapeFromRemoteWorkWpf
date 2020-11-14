using System.Collections.Generic;
using System.Linq;

namespace EscapeFromRemoteWorkWpf.Services
{
    /// <summary>
    /// スレッドの状態を管理するクラス
    /// </summary>
    public sealed class ThreadStatusService
    {
        #region シングルトン
        /// <summary>
        /// インスタンス
        /// </summary>
        private static readonly ThreadStatusService _instance = new ThreadStatusService();

        /// <summary>
        /// インスタンスを取得する
        /// </summary>
        /// <returns></returns>
        public static ThreadStatusService GetInstance()
        {
            return _instance;
        }
        #endregion

        #region メンバ変数
        /// <summary>
        /// ロックオブジェクト
        /// </summary>
        private object _lockObject = new object();
        /// <summary>
        /// 各スレッドの状態（ユーザによって操作されているか）を管理する辞書
        /// </summary>
        private Dictionary<string, bool> _statusDictionary = new Dictionary<string, bool>();
        #endregion

        #region メソッド
        /// <summary>
        /// 状態を追加する
        /// </summary>
        /// <param name="name">対象のクラス名</param>
        public void AddStatus(string name)
        {
            // 一応スレッドセーフにしておく
            lock (_lockObject)
            {
                _statusDictionary.Add(name, false);
            }
        }

        /// <summary>
        /// 状態を更新する
        /// </summary>
        /// <param name="name">クラス名</param>
        /// <param name="status">状態</param>
        public void SetStatus(string name, bool status)
        {
            // 一応スレッドセーフにしておく
            lock (_lockObject)
            {
                _statusDictionary[name] = status;
            }
        }

        /// <summary>
        /// 全てのクラスの状態を取得する
        /// </summary>
        /// <returns>全てのクラスの状態</returns>
        public List<bool> GetStatuses()
        {
            return _statusDictionary.Values.ToList();
        }
        #endregion
    }
}
