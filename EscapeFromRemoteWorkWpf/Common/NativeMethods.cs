using System;
using System.Runtime.InteropServices;
using System.Text;

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
        #region マウスカーソル系
        /// <summary>
        /// カーソル座標を設定する
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>成功/失敗</returns>
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// カーソル座標を取得する
        /// </summary>
        /// <param name="lpPoint">カーソル座標構造体</param>
        /// <returns>成功/失敗</returns>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        #endregion

        #region プロセスハンドル系
        /// <summary>
        /// アクティブウィンドウのハンドルを取得する
        /// </summary>
        /// <returns>アクティブウィンドウのハンドル</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 指定されたウィンドウハンドルをアクティブにする
        /// </summary>
        /// <param name="hWnd">アクティブにするウィンドウハンドル</param>
        /// <returns>成功/失敗</returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 指定されたウィンドウを作成したスレッドIDとプロセスIDを取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpdwProcessId">プロセスID</param>
        /// <returns>スレッドID</returns>
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        /// <summary>
        /// 既存のプロセスオブジェクトのハンドルを開く
        /// </summary>
        /// <param name="dwDesiredAccess">プロセスオブジェクトで認められるアクセス方法</param>
        /// <param name="bInheritedHandle">取得されたハンドルを継承できるか</param>
        /// <param name="dwProcessId">プロセスID</param>
        /// <returns>指定されたプロセスのハンドル（失敗したらIntPtr.Zero）</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritedHandle, uint dwProcessId);

        /// <summary>
        /// 既存のプロセスオブジェクトのハンドルを閉じる
        /// </summary>
        /// <param name="handle">プロセスオブジェクトのハンドル</param>
        /// <returns>成功/失敗</returns>
        [DllImport("user32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// 指定されたモジュールのベース名を取得する
        /// </summary>
        /// <param name="hWnd">プロセスハンドル</param>
        /// <param name="hModule">モジュールハンドル</param>
        /// <param name="lpBaseName">モジュールのベース名を受け取る変数</param>
        /// <param name="nSize">モジュールのベース名を受け取る最大文字数</param>
        /// <returns>成功/失敗</returns>
        [DllImport("user32.dll")]
        public static extern uint GetModuleBaseName(IntPtr hWnd, IntPtr hModule, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder lpBaseName, uint nSize);

        /// <summary>
        /// EnumWindowsから呼び出されるコールバック関数のデリゲート
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lParam">アプリケーション定義の値</param>
        /// <returns>成功/失敗</returns>
        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// 画面上の全てのトップレベルウィンドウを列挙する
        /// </summary>
        /// <param name="lpEnumFunc">アプリケーション定義のコールバック関数</param>
        /// <param name="lParam">コールバック関数に渡すアプリケーション定義の値</param>
        /// <returns>成功/失敗</returns>
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// 指定されたウィンドウの表示状態を取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>表示状態</returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// 指定されたウィンドウのタイトルバーテキストの文字数を返す
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>テキストの文字数</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// 指定されたウィンドウのタイトルバーテキストを取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpString">タイトルバーテキストを受け取る変数</param>
        /// <param name="nMaxCount">タイトルバーテキストを受け取る最大値</param>
        /// <returns>テキストの文字数</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder lpString, int nMaxCount);
        #endregion
    }
}
