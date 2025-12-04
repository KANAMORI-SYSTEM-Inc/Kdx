using Kdx.Contracts.Enums;

namespace Kdx.Contracts.DTOs
{
    public class FindIOResult
    {
        public FindIOResultState State { get; init; }
        public string? SingleAddress { get; init; }
        public List<IO>? MultipleMatches { get; init; }
    }
}