using EscapeFromRemoteWorkWpf.Common;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;

namespace EscapeFromRemoteWorkWpf.Behaviors
{
    /// <summary>
    /// Window.Closingイベントのビヘイビア
    /// </summary>
    public class WindowClosingBehavior : Behavior<Window>
    {
        /// <summary>
        /// OnAttached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += OnWindowClosing;
        }

        /// <summary>
        /// OnDetaching
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closing -= OnWindowClosing;
        }

        /// <summary>
        /// Window.Closingイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var window = sender as Window;

            // ViewModelがインタフェースを実装していたらメソッドを実行する
            if (window.DataContext is IClosing)
            {
                e.Cancel = (window.DataContext as IClosing).OnClosing();
            }
        }
    }
}
