using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace EscapeFromRemoteWorkWpf.Extensions
{
    /// <summary>
    /// Bindingをbool値でスイッチする拡張クラス
    /// </summary>
    public class SwitchBindingExtension : Binding
    {
        #region プロパティ
        /// <summary>
        /// True時の値
        /// </summary>
        [ConstructorArgument("valueIfTrue")]
        public object ValueIfTrue { get; set; }
        /// <summary>
        /// False時の値
        /// </summary>
        [ConstructorArgument("valueIfFalse")]
        public object ValueIfFalse { get; set; }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// 引数なしコンストラクタ
        /// </summary>
        public SwitchBindingExtension()
        {
            Initialize();
        }
        /// <summary>
        /// 1引数コンストラクタ
        /// </summary>
        /// <param name="path">Path</param>
        public SwitchBindingExtension(string path) : base(path)
        {
            Initialize();
        }
        /// <summary>
        /// 3引数コンストラクタ
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="valueIfTrue">ValueIfTrue</param>
        /// <param name="valueIfFalse">ValueIfFalse</param>
        public SwitchBindingExtension(string path, object valueIfTrue, object valueIfFalse) : base(path)
        {
            Initialize();
            ValueIfTrue = valueIfTrue;
            ValueIfFalse = valueIfFalse;
        }
        #endregion

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            ValueIfTrue = DoNothing;
            ValueIfFalse = DoNothing;
            Converter = new SwitchConverter(this);
        }

        /// <summary>
        /// 内部コンバーター
        /// </summary>
        private class SwitchConverter : IValueConverter
        {
            /// <summary>
            /// SwitchBindingExtension
            /// </summary>
            private SwitchBindingExtension _switch;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="switchExtension"></param>
            public SwitchConverter(SwitchBindingExtension switchExtension)
            {
                _switch = switchExtension;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <param name="targetType"></param>
            /// <param name="parameter"></param>
            /// <param name="culture"></param>
            /// <returns></returns>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    bool b = System.Convert.ToBoolean(value);
                    return b ? _switch.ValueIfTrue : _switch.ValueIfFalse;
                }
                catch
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <param name="targetType"></param>
            /// <param name="parameter"></param>
            /// <param name="culture"></param>
            /// <returns></returns>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return DoNothing;
            }
        }
    }
}
