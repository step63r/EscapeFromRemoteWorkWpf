using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace EscapeFromRemoteWorkWpf.Behaviors
{
    /// <summary>
    /// TextBox添付ビヘイビア
    /// </summary>
    public class TextBoxBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// IsNumericプロパティ
        /// </summary>
        public static readonly DependencyProperty IsNumericProperty = DependencyProperty.RegisterAttached("IsNumeric", typeof(bool), typeof(TextBoxBehavior), new UIPropertyMetadata(false, IsNumericChanged));

        /// <summary>
        /// IsNumericプロパティを取得する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetIsNumeric(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumericProperty);
        }

        /// <summary>
        /// IsNumericプロパティを設定する
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetIsNumeric(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumericProperty, value);
        }

        /// <summary>
        /// IsNumericプロパティが変更されたときのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IsNumericChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.KeyDown -= OnKeyDown;
                textBox.TextChanged -= OnTextChanged;

                bool newValue = (bool)e.NewValue;
                if (newValue)
                {
                    textBox.KeyDown += OnKeyDown;
                    textBox.TextChanged += OnTextChanged;
                }
            }
        }

        /// <summary>
        /// キー入力イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if ((Key.D0 <= e.Key && e.Key <= Key.D9) || 
                    (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9) || 
                    (Key.Delete == e.Key) || (Key.Back == e.Key) || (Key.Tab == e.Key))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// テキスト変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = "0";
                }
            }
        }
    }
}
