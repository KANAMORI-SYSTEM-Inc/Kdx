using Kdx.Contracts.DTOs;
using Kdx.Contracts.Interfaces;
using Kdx.Infrastructure.Supabase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kdx.Infrastructure.Services
{
    /// <summary>
    /// OperationIOテーブルのデータ操作を行うサービスクラス
    /// </summary>
    public class OperationIOService : IOperationIOService
    {
        private readonly ISupabaseRepository _repository;

        public OperationIOService(ISupabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// 指定されたOperationに関連付けられたIOのリストを取得
        /// </summary>
        public List<OperationIO> GetOperationIOs(int operationId)
        {
            try
            {
                return Task.Run(async () => await _repository.GetOperationIOsAsync(operationId)).GetAwaiter().GetResult();
            }
            catch (Exception ex) when (ex.Message.Contains("OperationIO") && ex.Message.Contains("見つかりませんでした"))
            {
                // テーブルが存在しない場合は空のリストを返す
                return new List<OperationIO>();
            }
        }

        /// <summary>
        /// 指定されたIOに関連付けられたOperationのリストを取得
        /// </summary>
        public List<OperationIO> GetIOOperations(string ioAddress, int plcId)
        {
            try
            {
                return Task.Run(async () => await _repository.GetIOOperationsAsync(ioAddress, plcId)).GetAwaiter().GetResult();
            }
            catch (Exception ex) when (ex.Message.Contains("OperationIO") && ex.Message.Contains("見つかりませんでした"))
            {
                return new List<OperationIO>();
            }
        }

        /// <summary>
        /// OperationとIOの関連付けを追加
        /// </summary>
        public void AddAssociation(int operationId, string ioAddress, int plcId, string ioUsage, string? comment = null)
        {
            // 既存の関連付けをチェック
            var existing = Task.Run(async () => await _repository.GetOperationIOsAsync(operationId)).GetAwaiter().GetResult()
                .FirstOrDefault(o => o.IOAddress == ioAddress && o.PlcId == plcId);

            if (existing != null)
            {
                throw new InvalidOperationException("この関連付けは既に存在します。");
            }

            Task.Run(async () => await _repository.AddOperationIOAssociationAsync(operationId, ioAddress, plcId, ioUsage, comment)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// OperationとIOの関連付けを削除
        /// </summary>
        public void RemoveAssociation(int operationId, string ioAddress, int plcId)
        {
            Task.Run(async () => await _repository.RemoveOperationIOAssociationAsync(operationId, ioAddress, plcId)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 指定されたPLCのすべての関連付けを取得
        /// </summary>
        public List<OperationIO> GetAllAssociations(int plcId)
        {
            try
            {
                return Task.Run(async () => await _repository.GetAllOperationIOAssociationsAsync(plcId)).GetAwaiter().GetResult();
            }
            catch (Exception ex) when (ex.Message.Contains("OperationIO") && ex.Message.Contains("見つかりませんでした"))
            {
                return new List<OperationIO>();
            }
        }
    }
}