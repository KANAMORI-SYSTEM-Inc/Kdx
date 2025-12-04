using Kdx.Contracts.DTOs;
using Kdx.Core.Application;

namespace Kdx.Infrastructure.Services
{
    /// <summary>
    /// インターロック設定のバリデーションを行うサービスの実装
    /// </summary>
    public class InterlockValidationService : IInterlockValidationService
    {
        public List<string> ValidateInterlock(Interlock interlock)
        {
            var errors = new List<string>();

            if (interlock == null)
            {
                errors.Add("インターロック設定がnullです");
                return errors;
            }

            if (interlock.PlcId <= 0)
            {
                errors.Add("PLC IDが無効です");
            }

            if (interlock.CylinderId <= 0)
            {
                errors.Add("シリンダーIDが無効です");
            }

            if (interlock.ConditionCylinderId <= 0)
            {
                errors.Add("条件シリンダーIDが無効です");
            }

            if (interlock.CylinderId == interlock.ConditionCylinderId)
            {
                errors.Add("シリンダーと条件シリンダーが同じです");
            }

            if (!ValidatePreConditionCombination(interlock.PreConditionID1, interlock.PreConditionID2))
            {
                errors.Add("前提条件の組み合わせが無効です");
            }

            return errors;
        }

        public bool HasCircularDependency(int cylinderId, int conditionCylinderId, int plcId, List<Interlock> existingInterlocks)
        {
            var visited = new HashSet<int>();
            return HasCircularDependencyRecursive(conditionCylinderId, cylinderId, plcId, existingInterlocks, visited);
        }

        /// <summary>
        /// 循環依存を再帰的にチェック
        /// </summary>
        private bool HasCircularDependencyRecursive(int currentCylinderId, int targetCylinderId, int plcId, List<Interlock> interlocks, HashSet<int> visited)
        {
            if (visited.Contains(currentCylinderId))
                return false;

            visited.Add(currentCylinderId);

            var dependencies = interlocks
                .Where(i => i.PlcId == plcId && i.CylinderId == currentCylinderId)
                .Select(i => i.ConditionCylinderId);

            foreach (var dependencyCylinderId in dependencies)
            {
                if (dependencyCylinderId == targetCylinderId)
                    return true;

                if (HasCircularDependencyRecursive(dependencyCylinderId, targetCylinderId, plcId, interlocks, visited))
                    return true;
            }

            return false;
        }

        public bool IsDuplicate(Interlock interlock, List<Interlock> existingInterlocks)
        {
            if (interlock == null || existingInterlocks == null)
                return false;

            return existingInterlocks.Any(existing =>
                existing.PlcId == interlock.PlcId &&
                existing.CylinderId == interlock.CylinderId &&
                existing.ConditionCylinderId == interlock.ConditionCylinderId &&
                existing.SortId == interlock.SortId &&
                existing.PreConditionID1 == interlock.PreConditionID1 &&
                existing.PreConditionID2 == interlock.PreConditionID2);
        }

        public bool ValidatePreConditionCombination(int? preCondition1, int? preCondition2)
        {
            // 前提条件は両方nullまたは両方設定されている必要がある
            // （実際のビジネスルールに応じて調整）
            if (preCondition1.HasValue && !preCondition2.HasValue)
                return false;

            if (!preCondition1.HasValue && preCondition2.HasValue)
                return false;

            // 両方設定されている場合、同じ値は許可しない
            if (preCondition1.HasValue && preCondition2.HasValue && preCondition1.Value == preCondition2.Value)
                return false;

            return true;
        }
    }
}
