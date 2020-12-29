using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromRemoteWorkWpf.Common
{
    /// <summary>
    /// 操作系インタフェース
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// 操作を実行する
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
