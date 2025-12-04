using System.Text;
using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Contracts.Interfaces;
using Kdx.Infrastructure.Supabase.Repositories;

namespace Kdx.Core.Application
{
    /// <summary>
    /// IOアドレス検索サービスの実装
    /// </summary>
    public class IOAddressService : IIOAddressService
    {
        private const string _lengthPrefix = "L-";
        private const string _underscorePrefix = "_";

        private readonly IErrorAggregator _errorAggregator;
        private readonly ISupabaseRepository _repository;
        private readonly IIOSelectorService _ioSelectorService;
        private readonly int _plcId;

        public IOAddressService(
            IErrorAggregator errorAggregator,
            ISupabaseRepository repository,
            int plcId,
            IIOSelectorService ioSelectorService)
        {
            _errorAggregator = errorAggregator;
            _repository = repository;
            _plcId = plcId;
            _ioSelectorService = ioSelectorService;
        }

        public string? GetSingleAddress(
            List<IO> ioList,
            string ioText,
            bool isOutput,
            string? recordName,
            int? recordId,
            string? isnotInclude)
        {
            if (ioText == "null")
            {
                return null;
            }

            // isOutput (Y/X) でフィルタリング
            var workingList = isOutput
                ? ioList.Where(io => io.Address != null && (io.Address.Contains("Y") || io.Address.Contains("Ｙ"))).ToList()
                : ioList.Where(io => io.Address != null && (io.Address.Contains("X") || io.Address.Contains("Ｘ"))).ToList();

            // isnotInclude パラメータに値が指定されている場合、その単語を含むIOを除外
            if (!string.IsNullOrEmpty(isnotInclude))
            {
                workingList = workingList.Where(io => io.IOName != null && !io.IOName.Contains(isnotInclude)).ToList();
            }

            var result = FindByIOTextInternal(workingList, ioText, recordName ?? string.Empty, recordId);

            switch (result.State)
            {
                case FindIOResultState.FoundOne:
                    return result.SingleAddress;

                case FindIOResultState.FoundMultiple:
                    // 複数件ヒットした場合、UI選択サービスを呼び出す
                    IO? selectedIo = _ioSelectorService.SelectIoFromMultiple(
                        ioText,
                        result.MultipleMatches ?? new List<IO>(),
                        recordName ?? string.Empty,
                        recordId);

                    if (selectedIo != null)
                    {
                        // ユーザーが項目を選択した場合、そのアドレスを返す
                        // LinkDeviceが設定されていればそれを優先
                        string? addressToReturn = !string.IsNullOrWhiteSpace(selectedIo.LinkDevice)
                            ? selectedIo.LinkDevice
                            : selectedIo.Address;
                        return addressToReturn?.Normalize(NormalizationForm.FormKC);
                    }
                    else
                    {
                        // ユーザーが選択をキャンセルした場合、エラーとして報告
                        _errorAggregator.AddError(new OutputError
                        {
                            Message = $"IO '{ioText}' の選択がキャンセルされました。",
                            RecordName = recordName ?? string.Empty,
                            RecordId = recordId,
                            IsCritical = true // 処理を続行できないため致命的なエラーとする
                        });
                        return null;
                    }

                case FindIOResultState.NotFound:
                default:
                    // 0件ヒットはエラー (既に内部でエラー追加済み)
                    return null;
            }
        }

        public string? GetSingleAddressOperation(
            List<IO> ioList,
            string ioText,
            bool isOutput,
            Operation operation,
            string? isnotInclude)
        {
            // isOutput (Y/X) でフィルタリング
            var workingList = isOutput
                ? ioList.Where(io => io.Address != null && (io.Address.Contains("Y") || io.Address.Contains("Ｙ"))).ToList()
                : ioList.Where(io => io.Address != null && (io.Address.Contains("X") || io.Address.Contains("Ｘ"))).ToList();

            // isnotInclude パラメータに値が指定されている場合、その単語を含むIOを除外
            if (!string.IsNullOrEmpty(isnotInclude))
            {
                workingList = workingList.Where(io => io.IOName != null && !io.IOName.Contains(isnotInclude)).ToList();
            }

            var result = FindByIOTextInternal(workingList, ioText, operation.OperationName ?? string.Empty, operation.Id);

            switch (result.State)
            {
                case FindIOResultState.FoundOne:
                    return result.SingleAddress;

                case FindIOResultState.FoundMultiple:
                    // 複数件ヒットした場合、UI選択サービスを呼び出す
                    IO? selectedIo = _ioSelectorService.SelectIoFromMultiple(
                        ioText,
                        result.MultipleMatches ?? new List<IO>(),
                        operation.OperationName ?? string.Empty,
                        operation.Id);

                    if (selectedIo != null)
                    {
                        // ユーザーが項目を選択した場合、そのアドレスを返す
                        // LinkDeviceが設定されていればそれを優先
                        string? addressToReturn = !string.IsNullOrWhiteSpace(selectedIo.LinkDevice)
                            ? selectedIo.LinkDevice
                            : selectedIo.Address;
                        return addressToReturn?.Normalize(NormalizationForm.FormKC);
                    }
                    else
                    {
                        // ユーザーが選択をキャンセルした場合、エラーとして報告
                        _errorAggregator.AddError(new OutputError
                        {
                            Message = $"IO '{ioText}' の選択がキャンセルされました。",
                            RecordName = operation.OperationName ?? string.Empty,
                            RecordId = operation.Id,
                            IsCritical = true // 処理を続行できないため致命的なエラーとする
                        });
                        return null;
                    }

                case FindIOResultState.NotFound:
                default:
                    // 0件ヒットはエラー (既に内部でエラー追加済み)
                    return null;
            }
        }

        public List<IO> GetAddressRange(
            List<IO> ioList,
            string ioText,
            string processName,
            int recordId,
            bool errorIfNotFound = false)
        {
            if (string.IsNullOrEmpty(ioText))
            {
                _errorAggregator.AddError(new OutputError
                {
                    Message = "範囲指定でIOテキストが指定されていません。",
                    RecordName = processName,
                    RecordId = recordId,
                });
                return new List<IO>();
            }

            string searchText = ioText.StartsWith("_") ? ioText.Substring(1) : ioText;
            var matches = ioList.Where(io => io.IOName != null && io.IOName.Contains(searchText)).ToList();

            if (!matches.Any() && errorIfNotFound)
            {
                _errorAggregator.AddError(new OutputError
                {
                    Message = $"指定された範囲のIO '{ioText}' が見つかりませんでした。",
                    RecordName = processName,
                    RecordId = recordId
                });
            }
            return matches;
        }

        private FindIOResult FindByIOTextInternal(
            List<IO> ioList,
            string ioText,
            string recordName,
            int? recordId)
        {
            if (string.IsNullOrEmpty(ioText))
            {
                _errorAggregator.AddError(new OutputError
                {
                    Message = "IOテキストが指定されていません。",
                    RecordName = recordName,
                    RecordId = recordId
                });
                return new FindIOResult { State = FindIOResultState.NotFound };
            }

            if (ioText.StartsWith(_lengthPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var lengthList = Task.Run(async () => await _repository.GetLengthByPlcIdAsync(_plcId)).GetAwaiter().GetResult();

                // まず、条件に一致するオブジェクト自体を検索
                var foundLengthObject = lengthList?.FirstOrDefault(s =>
                    s != null && s.LengthName != null && s.LengthName.Contains(ioText));

                if (foundLengthObject != null)
                {
                    string? foundAddress = foundLengthObject.Device;
                    return new FindIOResult
                    {
                        State = FindIOResultState.FoundOne,
                        SingleAddress = foundAddress
                    };
                }
                else
                {
                    // 見つからなかった場合はエラーとして処理
                    _errorAggregator.AddError(new OutputError
                    {
                        Message = $"指定されたアドレス '{ioText}' が見つかりませんでした。",
                        RecordName = recordName,
                        RecordId = recordId
                    });
                    return new FindIOResult { State = FindIOResultState.NotFound };
                }
            }

            string searchText = ioText.StartsWith("_") ? ioText.Substring(1) : ioText;
            var matches = ioList.Where(io => io.IOName != null && io.IOName.Contains(searchText)).ToList();

            if (matches.Count == 0)
            {
                _errorAggregator.AddError(new OutputError
                {
                    Message = $"センサー '{ioText}' が見つかりませんでした。",
                    RecordName = recordName,
                    RecordId = recordId
                });
                return new FindIOResult { State = FindIOResultState.NotFound };
            }

            if (matches.Count == 1)
            {
                var foundIo = matches[0];
                string? addressToReturn = !string.IsNullOrWhiteSpace(foundIo.LinkDevice)
                    ? foundIo.LinkDevice
                    : foundIo.Address;

                string? normalizedAddress = addressToReturn?.Normalize(NormalizationForm.FormKC);

                return new FindIOResult
                {
                    State = FindIOResultState.FoundOne,
                    SingleAddress = normalizedAddress
                };
            }

            return new FindIOResult
            {
                State = FindIOResultState.FoundMultiple,
                MultipleMatches = matches
            };
        }
    }
}