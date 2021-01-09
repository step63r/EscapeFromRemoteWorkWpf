using log4net.Appender;
using System;
using System.IO;

namespace EscapeFromRemoteWorkWpf.Common
{
    /// <summary>
    /// ログファイルを日付で世代管理する拡張アペンダ
    /// </summary>
    /// <remarks>ファイル名の拡張子は *.log のみに対応</remarks>
    public class DailyRollingFileAppender : RollingFileAppender
    {
        #region プロパティ
        /// <summary>
        /// ログを保持する
        /// </summary>
        public TimeSpan MaxAgeRollBackups { get; set; }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DailyRollingFileAppender() : base()
        {
            PreserveLogFileNameExtension = true;
            StaticLogFileName = false;
        }

        /// <summary>
        /// ログ追記前にファイルを調整する
        /// </summary>
        protected override void AdjustFileBeforeAppend()
        {
            base.AdjustFileBeforeAppend();

            string logFolder = Path.GetDirectoryName(File);
            var checkTime = DateTime.Now.Subtract(MaxAgeRollBackups);
            foreach (string file in Directory.GetFiles(logFolder, "*.log"))
            {
                if (System.IO.File.GetLastWriteTime(file) < checkTime)
                {
                    DeleteFile(file);
                }
            }
        }
    }
}
