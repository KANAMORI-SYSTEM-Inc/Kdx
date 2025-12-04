using Kdx.Contracts.DTOs;

namespace Kdx.Core.Application
{
    /// <summary>
    /// インターロック設定のバリデーションを行うサービスのインターフェース
    /// </summary>
    public interface IInterlockValidationService
    {
        /// <summary>
        /// インターロック設定が有効かどうかを検証
        /// </summary>
        /// <param name="interlock">インターロック設定</param>
        /// <returns>検証結果（エラーメッセージのリスト）</returns>
        List<string> ValidateInterlock(Interlock interlock);

        /// <summary>
        /// 循環参照をチェック
        /// </summary>
        /// <param name="cylinderId">シリンダーID</param>
        /// <param name="conditionCylinderId">条件シリンダーID</param>
        /// <param name="plcId">PLC ID</param>
        /// <param name="existingInterlocks">既存のインターロック設定リスト</param>
        /// <returns>循環参照がある場合true</returns>
        bool HasCircularDependency(int cylinderId, int conditionCylinderId, int plcId, List<Interlock> existingInterlocks);

        /// <summary>
        /// 重複したインターロック設定をチェック
        /// </summary>
        /// <param name="interlock">インターロック設定</param>
        /// <param name="existingInterlocks">既存のインターロック設定リスト</param>
        /// <returns>重複がある場合true</returns>
        bool IsDuplicate(Interlock interlock, List<Interlock> existingInterlocks);

        /// <summary>
        /// 前提条件の組み合わせが有効かどうかを検証
        /// </summary>
        /// <param name="preCondition1">前提条件1</param>
        /// <param name="preCondition2">前提条件2</param>
        /// <returns>有効な場合true</returns>
        bool ValidatePreConditionCombination(int? preCondition1, int? preCondition2);
    }
}
