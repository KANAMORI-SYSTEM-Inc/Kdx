using Kdx.Contracts.DTOs;
using Kdx.Core.Application;
using Kdx.Infrastructure.Supabase.Repositories;

namespace Kdx.Infrastructure.Services
{
    /// <summary>
    /// プロセスフロー図の構築と管理を行うサービスの実装
    /// </summary>
    public class ProcessFlowService : IProcessFlowService
    {
        private readonly ISupabaseRepository _repository;

        public ProcessFlowService(ISupabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public List<ProcessDetail> GetProcessDetailsByCycle(int cycleId)
        {
            var allProcessDetails = Task.Run(async () => await _repository.GetProcessDetailsAsync()).GetAwaiter().GetResult();
            var processes = Task.Run(async () => await _repository.GetProcessesAsync()).GetAwaiter().GetResult();

            return allProcessDetails
                .Where(pd => processes.Any(p => p.Id == pd.ProcessId && p.CycleId == cycleId))
                .ToList();
        }

        public List<ProcessDetailConnection> GetConnections(int cycleId)
        {
            return Task.Run(async () => await _repository.GetProcessDetailConnectionsAsync(cycleId)).GetAwaiter().GetResult();
        }

        public List<ProcessDetailFinish> GetFinishConditions(int cycleId)
        {
            return Task.Run(async () => await _repository.GetProcessDetailFinishesAsync(cycleId)).GetAwaiter().GetResult();
        }

        public List<ProcessStartCondition> GetStartConditions(int cycleId)
        {
            return Task.Run(async () => await _repository.GetProcessStartConditionsAsync(cycleId)).GetAwaiter().GetResult();
        }

        public List<ProcessFinishCondition> GetProcessFinishConditions(int cycleId)
        {
            return Task.Run(async () => await _repository.GetProcessFinishConditionsAsync(cycleId)).GetAwaiter().GetResult();
        }

        public void AddConnection(ProcessDetailConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            Task.Run(async () => await _repository.AddProcessDetailConnectionAsync(connection)).GetAwaiter().GetResult();
        }

        public void DeleteConnection(int fromProcessDetailId, int toProcessDetailId)
        {
            if (fromProcessDetailId <= 0)
                throw new ArgumentException("無効な接続元IDです", nameof(fromProcessDetailId));
            if (toProcessDetailId <= 0)
                throw new ArgumentException("無効な接続先IDです", nameof(toProcessDetailId));

            Task.Run(async () => await _repository.DeleteProcessDetailConnectionAsync(fromProcessDetailId, toProcessDetailId)).GetAwaiter().GetResult();
        }

        public void AddFinishCondition(ProcessDetailFinish finish)
        {
            if (finish == null)
                throw new ArgumentNullException(nameof(finish));

            Task.Run(async () => await _repository.AddProcessDetailFinishAsync(finish)).GetAwaiter().GetResult();
        }

        public void DeleteFinishCondition(int processDetailId, int finishProcessDetailId)
        {
            if (processDetailId <= 0)
                throw new ArgumentException("無効な工程詳細IDです", nameof(processDetailId));
            if (finishProcessDetailId <= 0)
                throw new ArgumentException("無効な終了先工程詳細IDです", nameof(finishProcessDetailId));

            Task.Run(async () => await _repository.DeleteProcessDetailFinishAsync(processDetailId, finishProcessDetailId)).GetAwaiter().GetResult();
        }

        public bool ValidateConnection(int fromId, int toId)
        {
            if (fromId <= 0 || toId <= 0)
                return false;

            if (fromId == toId)
                return false;

            return true;
        }

        public bool HasCircularReference(int fromId, int toId, int cycleId)
        {
            var connections = GetConnections(cycleId);
            var visited = new HashSet<int>();

            return HasCircularReferenceRecursive(toId, fromId, connections, visited);
        }

        /// <summary>
        /// 循環参照を再帰的にチェック
        /// </summary>
        private bool HasCircularReferenceRecursive(int currentId, int targetId, List<ProcessDetailConnection> connections, HashSet<int> visited)
        {
            if (visited.Contains(currentId))
                return false;

            visited.Add(currentId);

            var outgoingConnections = connections.Where(c => c.FromProcessDetailId == currentId);

            foreach (var connection in outgoingConnections)
            {
                if (!connection.ToProcessDetailId.HasValue)
                    continue;

                if (connection.ToProcessDetailId.Value == targetId)
                    return true;

                if (HasCircularReferenceRecursive(connection.ToProcessDetailId.Value, targetId, connections, visited))
                    return true;
            }

            return false;
        }
    }
}
