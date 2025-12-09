using Kdx.Contracts.DTOs;
using Kdx.Contracts.Interfaces;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.ViewModels;
using MnemonicSpeedDevice = Kdx.Contracts.DTOs.MnemonicSpeedDevice;

namespace KdxDesigner.Utils.Interlock
{
    public class InterlockBuilder
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IErrorAggregator _errorAggregator;
        private readonly IIOAddressService _ioService;
        private readonly ISupabaseRepository _repository;


        public InterlockBuilder(MainViewModel mainViewModel, IErrorAggregator errorAggregator, IIOAddressService ioService, ISupabaseRepository repository)
        {
            _mainViewModel = mainViewModel;
            _errorAggregator = errorAggregator;
            _ioService = ioService;
            _repository = repository;
        }

        public async Task<List<LadderCsvRow>> GenerateLadder(
            List<MnemonicDeviceWithCylinder> cylinders,
            List<IO> ioList)
        {
            LadderCsvRow.ResetKeyCounter();
            var result = new List<LadderCsvRow>();

            foreach (var cylinder in cylinders)
            {
                switch (cylinder.Cylinder.DriveSubId)
                {
                    case 1:
                    case 4:
                    default:
                        break;
                }
            }
            return result;
        }

    }
}
