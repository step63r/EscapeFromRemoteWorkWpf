using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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
    /// キーボードの状態の構造体
    /// </summary>
    public struct KeyboardState
    {
        public Stroke Stroke;
        public Keys Key;
        public uint ScanCode;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    /// <summary>
    /// 挙動の列挙型
    /// </summary>
    public enum Stroke
    {
        /// <summary>
        /// キーダウン
        /// </summary>
        KEY_DOWN,
        /// <summary>
        /// キーアップ
        /// </summary>
        KEY_UP,
        /// <summary>
        /// システムキーダウン
        /// </summary>
        SYSKEY_DOWN,
        /// <summary>
        /// システムキーアップ
        /// </summary>
        SYSKEY_UP,
        /// <summary>
        /// 不明
        /// </summary>
        UNKNOWN
    }

    /// <summary>
    /// キーボード入力イベント構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// ウィンドウ属性
    /// </summary>
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS,
        DWMWA_HAS_ICONIC_BITMAP,
        DWMWA_DISALLOW_PEEK,
        DWMWA_EXCLUDED_FROM_PEEK,
        DWMWA_CLOAK,
        DWMWA_CLOAKED,
        DWMWA_FREEZE_REPRESENTATION,
        DWMWA_LAST
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

        #region キー系
        /// <summary>
        /// フックプロシージャのデリゲート
        /// </summary>
        /// <param name="nCode">フックプロシージャに渡すフックコード</param>
        /// <param name="msg">フックプロシージャに渡す値</param>
        /// <param name="kbdllhookstruct">フックプロシージャに渡す値</param>
        /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
        public delegate IntPtr KeyboardHookCallback(int nCode, uint msg, ref KBDLLHOOKSTRUCT kbdllhookstruct);

        /// <summary>
        /// アプリケーション定義のフックプロシージャをフックチェーン内にインストールする
        /// </summary>
        /// <param name="idHook">フックタイプ</param>
        /// <param name="lpfn">フックプロシージャ</param>
        /// <param name="hMod">アプリケーションインスタンスのハンドル</param>
        /// <param name="dwThreadId">スレッドID</param>
        /// <returns>フックプロシージャのハンドル（失敗したらNULL）</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookCallback lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// 現在のフックチェーン内の次のフックプロシージャにフック情報を渡す
        /// </summary>
        /// <param name="hhk">現在のフックのハンドル</param>
        /// <param name="nCode">フックプロシージャに渡すフックコード</param>
        /// <param name="msg">フックプロシージャに渡す値</param>
        /// <param name="kbdllhookstruct">フックプロシージャに渡す値</param>
        /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, uint msg, ref KBDLLHOOKSTRUCT kbdllhookstruct);

        /// <summary>
        /// インストールされたフックプロシージャを削除する
        /// </summary>
        /// <param name="hhk">削除対象のフックプロシージャのハンドル</param>
        /// <returns>成功/失敗</returns>
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
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

        /// <summary>
        /// ウィンドウ属性を取得する
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="dwAttributem"></param>
        /// <param name="pvAttribute"></param>
        /// <param name="cbAttribute"></param>
        /// <returns></returns>
        [DllImport("dwmapi.dll")]
        public static extern long DwmGetWindowAttribute(IntPtr hWnd, DWMWINDOWATTRIBUTE dwAttributem, out bool pvAttribute, int cbAttribute);
        #endregion
    }

    /// <summary>
    /// キーボードフックの補助クラス
    /// </summary>
    public static class KeyboardHook
    {
        #region 構造体・列挙型
        /// <summary>
        /// 挙動の列挙型
        /// </summary>
        public enum Stroke
        {
            KEY_DOWN,
            KEY_UP,
            SYSKEY_DOWN,
            SYSKEY_UP,
            UNKNOWN
        }

        /// <summary>
        /// キーボードの状態の構造体
        /// </summary>
        public struct KeyboardState
        {
            public Stroke Stroke;
            public Keys Key;
            public uint ScanCode;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// キーボードのグローバルフックを実行しているかどうか
        /// </summary>
        public static bool IsHooking { get; private set; }
        /// <summary>
        /// キーボードのグローバルフックを中断しているかどうか
        /// </summary>
        public static bool IsPaused { get; private set; }
        #endregion

        #region デリゲート
        /// <summary>
        /// フックプロシージャ内でのイベント用のデリゲート
        /// </summary>
        /// <param name="state"></param>
        public delegate void HookHandler(ref KeyboardState state);
        #endregion

        #region メンバ変数
        /// <summary>
        /// キーボードの状態
        /// </summary>
        private static KeyboardState _state;
        /// <summary>
        /// フックプロシージャのハンドル
        /// </summary>
        private static IntPtr _handle;
        /// <summary>
        /// 入力をキャンセルするかどうか
        /// </summary>
        private static bool _isCancel;
        /// <summary>
        /// 登録イベントのリスト
        /// </summary>
        private static List<HookHandler> _events;
        /// <summary>
        /// フックプロシージャ内でのイベント
        /// </summary>
        private static event HookHandler _hookEvent;
        /// <summary>
        /// フックチェーンにインストールするフックプロシージャのイベント
        /// </summary>
        private static event NativeMethods.KeyboardHookCallback _hookCallBack;
        #endregion

        #region メソッド
        public static void Start()
        {
            if (IsHooking)
            {
                return;
            }

            IsHooking = true;
            IsPaused = false;

            _hookCallBack = HookProcedure;
            var h = Marshal.GetHINSTANCE(typeof(KeyboardHook).Assembly.GetModules()[0]);

            // WH_KEYBOARD_LL = 13
            _handle = NativeMethods.SetWindowsHookEx(13, _hookCallBack, h, 0);

            if (_handle == IntPtr.Zero)
            {
                IsHooking = false;
                IsPaused = true;

                throw new System.ComponentModel.Win32Exception();
            }
        }

        public static void Stop()
        {
            if (!IsHooking)
            {
                return;
            }

            if (_handle != IntPtr.Zero)
            {
                IsHooking = false;
                IsPaused = true;

                ClearEvent();

                NativeMethods.UnhookWindowsHookEx(_handle);
                _handle = IntPtr.Zero;
                _hookCallBack -= HookProcedure;
            }
        }

        /// <summary>
        /// キーボードのグローバルフックを中断する
        /// </summary>
        public static void Cancel()
        {
            IsPaused = true;
        }

        /// <summary>
        /// キーボード操作時のイベントを追加する
        /// </summary>
        /// <param name="hookHandler"></param>
        public static void AddEvent(HookHandler hookHandler)
        {
            if (_events == null)
            {
                _events = new List<HookHandler>();
            }

            _events.Add(hookHandler);
            _hookEvent += hookHandler;
        }

        /// <summary>
        /// キーボード操作時のイベントを削除する
        /// </summary>
        /// <param name="hookHandler"></param>
        public static void RemoveEvent(HookHandler hookHandler)
        {
            if (_events == null)
            {
                return;
            }

            _hookEvent -= hookHandler;
            _events.Remove(hookHandler);
        }

        /// <summary>
        /// キーボード操作時のイベントを全て削除する
        /// </summary>
        public static void ClearEvent()
        {
            if (_events == null)
            {
                return;
            }

            foreach (var e in _events)
            {
                _hookEvent -= e;
            }

            _events.Clear();
        }

        /// <summary>
        /// フックチェーンにインストールするフックプロシージャ
        /// </summary>
        /// <param name="nCode">フックプロシージャに渡すフックコード</param>
        /// <param name="msg">フックプロシージャに渡す値</param>
        /// <param name="mslhookstruct">フックプロシージャに渡す値</param>
        /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
        private static IntPtr HookProcedure(int nCode, uint msg, ref KBDLLHOOKSTRUCT mslhookstruct)
        {
            if (nCode >= 0 && _hookEvent != null && !IsPaused)
            {
                _state.Stroke = GetStroke(msg);
                _state.Key = (Keys)mslhookstruct.vkCode;
                _state.ScanCode = mslhookstruct.scanCode;
                _state.Flags = mslhookstruct.flags;
                _state.Time = mslhookstruct.time;
                _state.ExtraInfo = mslhookstruct.dwExtraInfo;

                _hookEvent(ref _state);

                if (_isCancel)
                {
                    _isCancel = false;
                    return (IntPtr)1;
                }
            }

            return NativeMethods.CallNextHookEx(_handle, nCode, msg, ref mslhookstruct);
        }

        /// <summary>
        /// キーボードの挙動を取得する
        /// </summary>
        /// <param name="msg">キーボードに関するウィンドウメッセージ</param>
        /// <returns>キーボードの挙動</returns>
        private static Stroke GetStroke(uint msg)
        {
            switch (msg)
            {
                case 0x100:
                    return Stroke.KEY_DOWN;
                case 0x101:
                    return Stroke.KEY_UP;
                case 0x104:
                    return Stroke.SYSKEY_DOWN;
                case 0x105:
                    return Stroke.SYSKEY_UP;
                default:
                    return Stroke.UNKNOWN;
            }
        }
        #endregion
    }
}
