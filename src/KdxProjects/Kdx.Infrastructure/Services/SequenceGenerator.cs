using Kdx.Core.Domain.Services;
using System.Threading;

namespace Kdx.Infrastructure.Services
{
    public sealed class SequenceGenerator : ISequenceGenerator
    {
        private int _current;
        public SequenceGenerator(int seed = 0) => _current = seed;
        public int Next() => Interlocked.Increment(ref _current);
    }
}
