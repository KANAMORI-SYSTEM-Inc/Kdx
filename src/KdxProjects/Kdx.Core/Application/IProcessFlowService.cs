using Kdx.Contracts.DTOs;

namespace Kdx.Core.Application
{
    /// <summary>
    /// プロセスフロー図の構築と管理を行うサービスのインターフェース
    /// </summary>
    public interface IProcessFlowService
    {
        /// <summary>
        /// 指定されたサイクルのプロセス詳細とその接続情報を取得
        /// </summary>
        /// <param name="cycleId">サイクルID</param>
        /// <returns>プロセス詳細のリスト</returns>
        List<ProcessDetail> GetProcessDetailsByCycle(int cycleId);

        /// <summary>
        /// プロセス詳細間の接続を取得
        /// </summary>
        /// <param name="cycleId">サイクルID</param>
        /// <returns>プロセス詳細接続のリスト</returns>
        List<ProcessDetailConnection> GetConnections(int cycleId);

        /// <summary>
        /// プロセス詳細の終了条件を取得
        /// </summary>
        /// <param name="cycleId">サイクルID</param>
        /// <returns>プロセス詳細終了条件のリスト</returns>
        List<ProcessDetailFinish> GetFinishConditions(int cycleId);

        /// <summary>
        /// プロセスの開始条件を取得
        /// </summary>
        /// <param name="cycleId">サイクルID</param>
        /// <returns>プロセス開始条件のリスト</returns>
        List<ProcessStartCondition> GetStartConditions(int cycleId);

        /// <summary>
        /// プロセスの終了条件を取得
        /// </summary>
        /// <param name="cycleId">サイクルID</param>
        /// <returns>プロセス終了条件のリスト</returns>
        List<ProcessFinishCondition> GetProcessFinishConditions(int cycleId);

        /// <summary>
        /// プロセス詳細間の接続を追加
        /// </summary>
        /// <param name="connection">接続情報</param>
        void AddConnection(ProcessDetailConnection connection);

        /// <summary>
        /// プロセス詳細間の接続を削除
        /// </summary>
        /// <param name="fromProcessDetailId">接続元のプロセス詳細ID</param>
        /// <param name="toProcessDetailId">接続先のプロセス詳細ID</param>
        void DeleteConnection(int fromProcessDetailId, int toProcessDetailId);

        /// <summary>
        /// プロセス詳細の終了条件を追加
        /// </summary>
        /// <param name="finish">終了条件</param>
        void AddFinishCondition(ProcessDetailFinish finish);

        /// <summary>
        /// プロセス詳細の終了条件を削除
        /// </summary>
        /// <param name="processDetailId">工程詳細ID</param>
        /// <param name="finishProcessDetailId">終了先の工程詳細ID</param>
        void DeleteFinishCondition(int processDetailId, int finishProcessDetailId);

        /// <summary>
        /// 接続が有効かどうかを検証
        /// </summary>
        /// <param name="fromId">接続元のプロセス詳細ID</param>
        /// <param name="toId">接続先のプロセス詳細ID</param>
        /// <returns>有効な場合true</returns>
        bool ValidateConnection(int fromId, int toId);

        /// <summary>
        /// 循環参照をチェック
        /// </summary>
        /// <param name="fromId">接続元のプロセス詳細ID</param>
        /// <param name="toId">接続先のプロセス詳細ID</param>
        /// <param name="cycleId">サイクルID</param>
        /// <returns>循環参照がある場合true</returns>
        bool HasCircularReference(int fromId, int toId, int cycleId);
    }
}
