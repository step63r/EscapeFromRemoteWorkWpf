using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromRemoteWorkWpf.Common
{
    /// <summary>
    /// カーソル座標構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// X座標
        /// </summary>
        public int X;
        /// <summary>
        /// Y座標
        /// </summary>
        public int Y;
    }

    /// <summary>
    /// Windows APIクラス
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// カーソル座標を設定する
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>成功/失敗</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// カーソル座標を取得する
        /// </summary>
        /// <param name="lpPoint">カーソル座標構造体</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);
    }
}
