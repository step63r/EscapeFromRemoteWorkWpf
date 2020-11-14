using System;
using System.Threading;

namespace EscapeFromRemoteWorkWpf.Common
{
    /// <summary>
    /// スレッドローカルなRandomを生成するクラス
    /// </summary>
    public static class RandomProvider
    {
        #region メンバ変数
        /// <summary>
        /// シード値
        /// </summary>
        private static int seed = Environment.TickCount;
        /// <summary>
        /// スレッドローカルなRandomオブジェクト
        /// </summary>
        private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
        #endregion

        #region メソッド
        public static Random GetThreadRandom()
        {
            return randomWrapper.Value;
        }
        #endregion
    }
}
