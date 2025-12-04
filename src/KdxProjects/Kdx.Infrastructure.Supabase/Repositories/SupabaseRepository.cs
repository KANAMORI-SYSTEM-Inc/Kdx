using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Entities;
using Supabase;
using System.Diagnostics;
using Timer = Kdx.Contracts.DTOs.Timer;

namespace Kdx.Infrastructure.Supabase.Repositories
{
    /// <summary>
    /// Supabase繝・・繧ｿ繝吶・繧ｹ縺ｸ縺ｮ繧｢繧ｯ繧ｻ繧ｹ讖溯・繧呈署萓帙☆繧九Μ繝昴ず繝医Μ縺ｮ螳溯｣・
    /// </summary>
    public partial class SupabaseRepository : ISupabaseRepository
    {
        private readonly Client _supabaseClient;

        public SupabaseRepository(Client supabaseClient)
        {
            _supabaseClient = supabaseClient ?? throw new ArgumentNullException(nameof(supabaseClient));
        }

        public async Task<List<Company>> GetCompaniesAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Calling Supabase API for Companies...");

                var response = await _supabaseClient
                    .From<CompanyEntity>()
                    .Select("*")
                    .Get();

                System.Diagnostics.Debug.WriteLine($"Supabase returned {response?.Models?.Count ?? 0} companies");

                var companies = response?.Models?.Select(e => e.ToDto()).ToList() ?? new List<Company>();

                if (companies.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No companies found. Attempting to insert test data...");
                    try
                    {
                        var testCompany = new Company
                        {
                            CompanyName = "Test Company",
                            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        };

                        var insertResponse = await _supabaseClient
                            .From<CompanyEntity>()
                            .Insert(CompanyEntity.FromDto(testCompany));

                        System.Diagnostics.Debug.WriteLine("Test company inserted successfully");

                        // 謖ｿ蜈･蠕後↓蜀榊ｺｦ蜿門ｾ・
                        response = await _supabaseClient
                            .From<CompanyEntity>()
                            .Select("*")
                            .Get();

                        companies = response?.Models?.Select(e => e.ToDto()).ToList() ?? new List<Company>();
                        System.Diagnostics.Debug.WriteLine($"After insert, found {companies.Count} companies");
                    }
                    catch (Exception insertEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to insert test data: {insertEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"This might indicate RLS is enabled. Check Supabase dashboard.");
                    }
                }

                return companies;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Supabase API error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<List<Model>> GetModelsAsync()
        {
            var response = await _supabaseClient
                .From<ModelEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<PLC>> GetPLCsAsync()
        {
            var response = await _supabaseClient
                .From<PLCEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<Cycle>> GetCyclesAsync()
        {
            var response = await _supabaseClient
                .From<CycleEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<CylinderCycle>> GetCylinderCyclesByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<CylinderCycleEntity>()
                .Where(c => c.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<CylinderCycle>> GetCylinderCyclesByCylinderIdAsync(int cylinderId)
        {
            var response = await _supabaseClient
                .From<CylinderCycleEntity>()
                .Where(c => c.CylinderId == cylinderId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task AddCylinderCycleAsync(CylinderCycle cylinderCycle)
        {
            var entity = CylinderCycleEntity.FromDto(cylinderCycle);
            await _supabaseClient
                .From<CylinderCycleEntity>()
                .Insert(entity);
        }

        public async Task DeleteCylinderCycleAsync(int cylinderId, int cycleId)
        {
            await _supabaseClient
                .From<CylinderCycleEntity>()
                .Where(c => c.CylinderId == cylinderId && c.CycleId == cycleId)
                .Delete();
        }

        public async Task<List<ControlBox>> GetControlBoxesByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<ControlBoxEntity>()
                .Where(c => c.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<CylinderControlBox>> GetCylinderControlBoxesByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<CylinderControlBoxEntity>()
                .Where(c => c.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<CylinderControlBox>> GetCylinderControlBoxesAsync(int plcId, int cylinderId)
        {
            var response = await _supabaseClient
                .From<CylinderControlBoxEntity>()
                .Where(c => c.PlcId == plcId)
                .Where(c => c.CylinderId == cylinderId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task UpsertCylinderControlBoxAsync(CylinderControlBox item)
        {
            var entity = CylinderControlBoxEntity.FromDto(item);
            await _supabaseClient
                .From<CylinderControlBoxEntity>()
                .Upsert(entity);
        }

        public async Task DeleteCylinderControlBoxAsync(int plcId, int cylinderId, int boxId)
        {
            await _supabaseClient
                .From<CylinderControlBoxEntity>()
                .Where(c => c.PlcId == plcId)
                .Where(c => c.CylinderId == cylinderId)
                .Where(c => c.ManualNumber == boxId)
                .Delete();
        }

        public async Task<List<Kdx.Contracts.DTOs.Process>> GetProcessesAsync()
        {
            var response = await _supabaseClient
                .From<ProcessEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<int> AddProcessAsync(Kdx.Contracts.DTOs.Process process)
        {
            // PostgreSQL関数を使用してINSERTし、新しいIDを取得
            var result = await _supabaseClient.Rpc<long>("insert_process", new
            {
                process_name = process.ProcessName,
                cycle_id = process.CycleId,
                test_start = process.TestStart,
                test_condition = process.TestCondition,
                test_mode = process.TestMode,
                auto_mode = process.AutoMode,
                auto_start = process.AutoStart,
                il_start = process.ILStart,
                process_category_id = process.ProcessCategoryId,
                comment1 = process.Comment1,
                comment2 = process.Comment2,
                sort_number = process.SortNumber
            });

            return (int)result;
        }

        public async Task UpdateProcessAsync(Kdx.Contracts.DTOs.Process process)
        {
            var entity = ProcessEntity.FromDto(process);
            await _supabaseClient
                .From<ProcessEntity>()
                .Where(p => p.Id == process.Id)
                .Update(entity);
        }

        public async Task<List<Machine>> GetMachinesAsync()
        {
            var response = await _supabaseClient
                .From<MachineEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MachineName>> GetMachineNamesAsync()
        {
            try
            {
                Debug.WriteLine("Calling Supabase API for MachineNames...");

                var response = await _supabaseClient
                    .From<MachineNameEntity>()
                    .Select("*")
                    .Get();

                Debug.WriteLine($"Supabase returned {response?.Models?.Count ?? 0} machine names");

                var machineNames = response?.Models?.Select(e => e.ToDto()).ToList() ?? new List<MachineName>();

                if (machineNames.Any())
                {
                    foreach (var mn in machineNames)
                    {
                        Debug.WriteLine($"MachineName: Id={mn.Id}, FullName={mn.FullName}, ShortName={mn.ShortName}");
                    }
                }

                return machineNames;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting machine names: {ex.Message}");
                Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return new List<MachineName>();
            }
        }

        public async Task<Machine?> GetMachineByIdAsync(int nameId, int driveSubId)
        {
            var response = await _supabaseClient
                .From<MachineEntity>()
                .Where(m => m.MachineNameId == nameId)
                .Where(m => m.DriveSubId == driveSubId)
                .Single();
            return response?.ToDto();
        }

        public async Task<MachineName?> GetMachineNameByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<MachineNameEntity>()
                .Where(m => m.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<List<DriveMain>> GetDriveMainsAsync()
        {
            var response = await _supabaseClient
                .From<DriveMainEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<DriveSub>> GetDriveSubsAsync()
        {
            var response = await _supabaseClient
                .From<DriveSubEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<DriveSub?> GetDriveSubByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<DriveSubEntity>()
                .Where(d => d.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<List<Cylinder>> GetCYsAsync()
        {
            var response = await _supabaseClient
                .From<CylinderEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<Cylinder>> GetCyListAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<CylinderEntity>()
                .Where(c => c.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<Cylinder?> GetCYByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<CylinderEntity>()
                .Where(c => c.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<List<Timer>> GetTimersAsync()
        {
            // ページング処理を使用して全タイマーを取得（1000件制限を回避）
            var allTimers = new List<Timer>();
            const int pageSize = 1000;
            int offset = 0;
            bool hasMore = true;

            while (hasMore)
            {
                var response = await _supabaseClient
                    .From<TimerEntity>()
                    .Range(offset, offset + pageSize - 1)
                    .Get();

                if (response?.Models != null && response.Models.Any())
                {
                    allTimers.AddRange(response.Models.Select(e => e.ToDto()));
                    offset += pageSize;
                    hasMore = response.Models.Count == pageSize;
                }
                else
                {
                    hasMore = false;
                }
            }

            return allTimers;
        }

        public async Task<List<Timer>> GetTimersByCycleIdAsync(int cycleId)
        {
            var response = await _supabaseClient
                .From<TimerEntity>()
                .Where(t => t.CycleId == cycleId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MnemonicTimerDevice>> GetTimersByRecordIdAsync(int cycleId, int mnemonicId, int recordId)
        {
            var response = await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Where(t => t.MnemonicId == mnemonicId)
                .Where(t => t.RecordId == recordId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<int> AddTimerAsync(Timer timer)
        {
            // PostgreSQL関数を使用してINSERTし、新しいIDを取得
            var result = await _supabaseClient.Rpc<long>("insert_timer", new
            {
                cycle_id_param = timer.CycleId,
                timer_category_id_param = timer.TimerCategoryId,
                timer_num_param = timer.TimerNum,
                timer_name_param = timer.TimerName,
                mnemonic_id_param = timer.MnemonicId,
                example_param = timer.Example
            });

            return (int)result;
        }

        public async Task UpdateTimerAsync(Timer timer)
        {
            var entity = TimerEntity.FromDto(timer);
            await _supabaseClient
                .From<TimerEntity>()
                .Where(t => t.ID == timer.ID)
                .Update(entity);
        }

        public async Task DeleteTimerAsync(int id)
        {
            await _supabaseClient
                .From<TimerEntity>()
                .Where(t => t.ID == id)
                .Delete();
        }

        public async Task<List<int>> GetTimerRecordIdsAsync(int timerId)
        {
            try
            {

                var response = await _supabaseClient
                    .From<TimerRecordIdEntity>()
                    .Where(t => t.TimerId == timerId)
                    .Get();

                var recordIds = response?.Models?.Select(t => t.ToDto().RecordId).ToList() ?? new List<int>();

                return recordIds;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetTimerRecordIdsAsync] 繧ｨ繝ｩ繝ｼ: {ex.Message}");
                return new List<int>();
            }
        }

        public async Task AddTimerRecordIdAsync(int timerId, int recordId)
        {
            // TimerRecordId髢｢騾｣繝・・繝悶Ν縺悟ｿ・ｦ√↑蝣ｴ蜷医∝ｮ溯｣・ｒ霑ｽ蜉
            var timerRecord = new TimerRecordId
            {
                TimerId = timerId,
                RecordId = recordId
            };
            var entity = TimerRecordIdEntity.FromDto(timerRecord);
            await _supabaseClient
                .From<TimerRecordIdEntity>()
                .Insert(entity);
        }

        public async Task DeleteTimerRecordIdAsync(int timerId, int recordId)
        {
            // TimerRecordId髢｢騾｣繝・・繝悶Ν縺悟ｿ・ｦ√↑蝣ｴ蜷医∝ｮ溯｣・ｒ霑ｽ蜉
            await _supabaseClient
                .From<TimerRecordIdEntity>()
                .Where(t => t.TimerId == timerId)
                .Where(t => t.RecordId == recordId)
                .Delete();
        }

        public async Task DeleteAllTimerRecordIdsAsync(int timerId)
        {
            // TimerRecordId髢｢騾｣繝・・繝悶Ν縺悟ｿ・ｦ√↑蝣ｴ蜷医∝ｮ溯｣・ｒ霑ｽ蜉
            await _supabaseClient
                .From<TimerRecordIdEntity>()
                .Where(t => t.TimerId == timerId)
                .Delete();
        }

        public async Task<List<Operation>> GetOperationsAsync()
        {
            var response = await _supabaseClient
                .From<OperationEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<Operation>> GetOperationsByCycleIdAsync(int cycleId)
        {
            var response = await _supabaseClient
                .From<OperationEntity>()
                .Where(o => o.CycleId == cycleId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<Operation?> GetOperationByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<OperationEntity>()
                .Where(o => o.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<List<Length>?> GetLengthByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<LengthEntity>()
                .Where(l => l.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<int> AddOperationAsync(Operation operation)
        {
            // PostgreSQL関数を使用してINSERTし、新しいIDを取得
            var result = await _supabaseClient.Rpc<long>("insert_operation", new
            {
                operation_name = operation.OperationName,
                cy_id = operation.CYId,
                category_id = operation.CategoryId,
                go_back = operation.GoBack,
                start_val = operation.Start,
                finish = operation.Finish,
                valve1 = operation.Valve1,
                s1 = operation.S1,
                s2 = operation.S2,
                s3 = operation.S3,
                s4 = operation.S4,
                s5 = operation.S5,
                ss1 = operation.SS1,
                ss2 = operation.SS2,
                ss3 = operation.SS3,
                ss4 = operation.SS4,
                pil = operation.PIL,
                sc = operation.SC,
                fc = operation.FC,
                cycle_id = operation.CycleId,
                sort_number = operation.SortNumber,
                con = operation.Con
            });

            return (int)result;
        }

        public async Task UpdateOperationAsync(Operation operation)
        {
            var entity = OperationEntity.FromDto(operation);
            await _supabaseClient
                .From<OperationEntity>()
                .Where(o => o.Id == operation.Id)
                .Update(entity);
        }

        public async Task<List<ProcessDetail>> GetProcessDetailsAsync()
        {
            var response = await _supabaseClient
                .From<ProcessDetailEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task UpdateProcessDetailAsync(ProcessDetail processDetail)
        {
            var entity = ProcessDetailEntity.FromDto(processDetail);
            await _supabaseClient
                .From<ProcessDetailEntity>()
                .Where(p => p.Id == processDetail.Id)
                .Update(entity);
        }

        public async Task<int> AddProcessDetailAsync(ProcessDetail processDetail)
        {
            // PostgreSQL関数を使用してINSERTし、新しいIDを取得
            var result = await _supabaseClient.Rpc<long>("insert_process_detail", new
            {
                process_id = processDetail.ProcessId,
                operation_id = processDetail.OperationId,
                detail_name = processDetail.DetailName,
                start_sensor = processDetail.StartSensor,
                category_id = processDetail.CategoryId,
                finish_sensor = processDetail.FinishSensor,
                block_number = processDetail.BlockNumber,
                skip_mode = processDetail.SkipMode,
                cycle_id = processDetail.CycleId,
                sort_number = processDetail.SortNumber,
                comment = processDetail.Comment,
                il_start = processDetail.ILStart,
                start_timer_id = processDetail.StartTimerId
            });

            return (int)result;
        }

        public async Task DeleteProcessAsync(int id)
        {
            await _supabaseClient
                .From<ProcessEntity>()
                .Where(p => p.Id == id)
                .Delete();
        }

        public async Task DeleteProcessDetailAsync(int id)
        {
            await _supabaseClient
                .From<ProcessDetailEntity>()
                .Where(p => p.Id == id)
                .Delete();
        }

        public async Task DeleteOperationAsync(int id)
        {
            await _supabaseClient
                .From<OperationEntity>()
                .Where(o => o.Id == id)
                .Delete();
        }

        public async Task<List<ProcessCategory>> GetProcessCategoriesAsync()
        {
            var response = await _supabaseClient
                .From<ProcessCategoryEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<ProcessDetailCategory>> GetProcessDetailCategoriesAsync()
        {
            var response = await _supabaseClient
                .From<ProcessDetailCategoryEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<ProcessDetailCategory?> GetProcessDetailCategoryByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<ProcessDetailCategoryEntity>()
                .Where(p => p.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<List<OperationCategory>> GetOperationCategoriesAsync()
        {
            var response = await _supabaseClient
                .From<OperationCategoryEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesAsync()
        {
            var response = await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByClcleIdAsync(int plcId, int cycleId)
        {
            var response = await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Where(m => m.PlcId == plcId)
                .Where(m => m.CycleId == cycleId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Where(m => m.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByCycleAndMnemonicIdAsync(int plcId, int cycleId, int mnemonicId)
        {
            var response = await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Where(m => m.PlcId == plcId)
                .Where(m => m.CycleId == cycleId)
                .Where(m => m.MnemonicId == mnemonicId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByMnemonicIdAsync(int plcId, int mnemonicId)
        {
            try
            {
                var response = await _supabaseClient
                    .From<MnemonicTimerDeviceEntity>()
                    .Where(m => m.PlcId == plcId)
                    .Where(m => m.MnemonicId == mnemonicId)
                    .Get();
                return response?.Models?.Select(e => e.ToDto()).ToList() ?? new List<MnemonicTimerDevice>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetMnemonicTimerDevicesByMnemonicIdAsync] 繧ｨ繝ｩ繝ｼ: {ex.Message}");
                return new List<MnemonicTimerDevice>();
            }
        }

        public async Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByTimerIdAsync(int plcId, int timerId)
        {
            var response = await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Where(m => m.PlcId == plcId)
                .Where(m => m.TimerId == timerId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task UpdateMnemonicTimerDeviceAsync(MnemonicTimerDevice device)
        {
            var entity = MnemonicTimerDeviceEntity.FromDto(device);
            await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Where(m => m.MnemonicId == device.MnemonicId)
                .Where(m => m.RecordId == device.RecordId)
                .Where(m => m.TimerId == device.TimerId)
                .Update(entity);
        }

        public async Task DeleteAllMnemonicTimerDeviceAsync()
        {
            // Supabase requires a WHERE clause for DELETE. To delete all records, we use a condition that's always true
            await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Filter("PlcId", Postgrest.Constants.Operator.GreaterThanOrEqual, "0")
                .Delete();
        }

        public async Task DeleteMnemonicTimerDeviceAsync(int mnemonicId, int recordId, int timerId)
        {
            await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Where(m => m.MnemonicId == mnemonicId)
                .Where(m => m.RecordId == recordId)
                .Where(m => m.TimerId == timerId)
                .Delete();
        }

        public async Task AddMnemonicTimerDeviceAsync(MnemonicTimerDevice device)
        {
            var entity = MnemonicTimerDeviceEntity.FromDto(device);
            await _supabaseClient
                .From<MnemonicTimerDeviceEntity>()
                .Insert(entity);
        }

        public async Task UpsertMnemonicTimerDeviceAsync(MnemonicTimerDevice device)
        {
            try
            {
                Debug.WriteLine($"[UpsertMnemonicTimerDeviceAsync] 開始");
                Debug.WriteLine($"  MnemonicId: {device.MnemonicId}, RecordId: {device.RecordId}, TimerId: {device.TimerId}");
                Debug.WriteLine($"  PlcId: {device.PlcId}, CycleId: {device.CycleId}");
                Debug.WriteLine($"  TimerDeviceT: {device.TimerDeviceT}, TimerDeviceZR: {device.TimerDeviceZR}");
                Debug.WriteLine($"  Comment1: {device.Comment1}");

                // Supabase縺ｮupsert讖溯・繧剃ｽｿ逕ｨ
                // upsert縺ｯ閾ｪ蜍慕噪縺ｫ荳ｻ繧ｭ繝ｼ縺ｧ遶ｶ蜷医ｒ讀懷・縺励※譖ｴ譁ｰ縺ｾ縺溘・謖ｿ蜈･繧定｡後≧
                var entity = MnemonicTimerDeviceEntity.FromDto(device);
                var response = await _supabaseClient
                    .From<MnemonicTimerDeviceEntity>()
                    .Upsert(entity);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpsertMnemonicTimerDeviceAsync] 繧ｨ繝ｩ繝ｼ逋ｺ逕・ {ex.Message}");
                Debug.WriteLine($"  繧ｹ繧ｿ繝・け繝医Ξ繝ｼ繧ｹ: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"  蜀・Κ繧ｨ繝ｩ繝ｼ: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<List<IO>> GetIoListAsync()
        {
            var allIOs = new List<IO>();
            const int pageSize = 1000;
            int offset = 0;
            bool hasMore = true;

            while (hasMore)
            {
                var response = await _supabaseClient
                    .From<IOEntity>()
                    .Range(offset, offset + pageSize - 1)
                    .Get();

                if (response?.Models != null && response.Models.Any())
                {
                    allIOs.AddRange(response.Models.Select(e => e.ToDto()));
                    offset += pageSize;
                    hasMore = response.Models.Count == pageSize;
                }
                else
                {
                    hasMore = false;
                }
            }

            return allIOs;
        }

        public async Task<List<TimerCategory>> GetTimerCategoryAsync()
        {
            var response = await _supabaseClient
                .From<TimerCategoryEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<Servo>> GetServosAsync(int? plcId, int? cylinderId)
        {
            var query = _supabaseClient.From<ServoEntity>();

            if (plcId.HasValue && cylinderId.HasValue)
            {
                var response = await query
                    .Where(s => s.PlcId == plcId.Value)
                    .Where(s => s.CylinderId == cylinderId.Value)
                    .Get();
                return response.Models.Select(e => e.ToDto()).ToList();
            }
            else if (plcId.HasValue)
            {
                var response = await query.Where(s => s.PlcId == plcId.Value).Get();
                return response.Models.Select(e => e.ToDto()).ToList();
            }
            else if (cylinderId.HasValue)
            {
                var response = await query.Where(s => s.CylinderId == cylinderId.Value).Get();
                return response.Models.Select(e => e.ToDto()).ToList();
            }
            else
            {
                var response = await query.Get();
                return response.Models.Select(e => e.ToDto()).ToList();
            }
        }

        public async Task UpdateIoLinkDevicesAsync(IEnumerable<IO> ioRecordsToUpdate)
        {
            // 繝舌ャ繝∵峩譁ｰ繧貞ｮ溯｡・
            foreach (var io in ioRecordsToUpdate)
            {
                var entity = IOEntity.FromDto(io);
                await _supabaseClient
                    .From<IOEntity>()
                    .Where(i => i.Address == io.Address)
                    .Where(i => i.PlcId == io.PlcId)
                    .Update(entity);
            }
        }

        public async Task UpdateAndLogIoChangesAsync(List<IO> iosToUpdate, List<IOHistory> histories)
        {
            // 繝医Λ繝ｳ繧ｶ繧ｯ繧ｷ繝ｧ繝ｳ蜃ｦ逅・′蠢・ｦ√↑蝣ｴ蜷医・縲ヾupabase縺ｮ髢｢謨ｰ繧剃ｽｿ逕ｨ縺吶ｋ縺九・
            // 蛟句挨縺ｮ蜃ｦ逅・→縺励※螳溯｣・
            foreach (var io in iosToUpdate)
            {
                var entity = IOEntity.FromDto(io);
                // Upsertを使用して、新規レコードの挿入と既存レコードの更新の両方に対応
                await _supabaseClient
                    .From<IOEntity>()
                    .Upsert(entity);
            }

            foreach (var history in histories)
            {
                var entity = IOHistoryEntity.FromDto(history);
                await _supabaseClient
                    .From<IOHistoryEntity>()
                    .Insert(entity);
            }
        }

        public async Task<List<ProcessDetailConnection>> GetProcessDetailConnectionsAsync(int cycleId)
        {
            // ProcessDetailConnection doesn't have CycleId column in the database
            // We need to get connections through ProcessDetail relationships
            var processDetails = await _supabaseClient
                .From<ProcessDetailEntity>()
                .Where(pd => pd.CycleId == cycleId)
                .Get();

            if (!processDetails.Models.Any())
                return new List<ProcessDetailConnection>();

            var processDetailIds = processDetails.Models.Select(pd => pd.Id).ToList();

            // Get connections where either From or To ProcessDetailId is in our list
            var connections = new List<ProcessDetailConnection>();

            // Get connections FROM our process details
            foreach (var pdId in processDetailIds)
            {
                var fromConnections = await _supabaseClient
                    .From<ProcessDetailConnectionEntity>()
                    .Where(c => c.FromProcessDetailId == pdId)
                    .Get();
                connections.AddRange(fromConnections.Models.Select(e => e.ToDto()).ToList());
            }

            // Get connections TO our process details
            foreach (var pdId in processDetailIds)
            {
                var toConnections = await _supabaseClient
                    .From<ProcessDetailConnectionEntity>()
                    .Where(c => c.ToProcessDetailId == pdId)
                    .Get();
                connections.AddRange(toConnections.Models.Select(e => e.ToDto()).ToList());
            }

            // Remove duplicates and return
            return connections.Distinct().ToList();
        }

        public async Task<List<ProcessDetailConnection>> GetAllProcessDetailConnectionsAsync()
        {
            var response = await _supabaseClient
                .From<ProcessDetailConnectionEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<ProcessDetailConnection>> GetConnectionsByToIdAsync(int toProcessDetailId)
        {
            var response = await _supabaseClient
                .From<ProcessDetailConnectionEntity>()
                .Where(p => p.ToProcessDetailId == toProcessDetailId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<ProcessDetailConnection>> GetConnectionsByFromIdAsync(int fromProcessDetailId)
        {
            var response = await _supabaseClient
                .From<ProcessDetailConnectionEntity>()
                .Where(p => p.FromProcessDetailId == fromProcessDetailId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task AddProcessDetailConnectionAsync(ProcessDetailConnection connection)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SupabaseRepository] AddProcessDetailConnectionAsync 開始");
                System.Diagnostics.Debug.WriteLine($"  FromProcessDetailId: {connection.FromProcessDetailId}");
                System.Diagnostics.Debug.WriteLine($"  ToProcessDetailId: {connection.ToProcessDetailId}");
                System.Diagnostics.Debug.WriteLine($"  CycleId: {connection.CycleId}");

                // CycleIdがnullの場合、FromProcessDetailから取得
                if (!connection.CycleId.HasValue)
                {
                    var processDetail = await _supabaseClient
                        .From<ProcessDetailEntity>()
                        .Where(p => p.Id == connection.FromProcessDetailId)
                        .Single();

                    if (processDetail != null)
                    {
                        connection.CycleId = processDetail.CycleId;
                        System.Diagnostics.Debug.WriteLine($"  CycleIdをProcessDetailから取得: {connection.CycleId}");
                    }
                    else
                    {
                        throw new InvalidOperationException($"ProcessDetail (ID={connection.FromProcessDetailId}) が見つかりません。");
                    }
                }

                // 既存の接続を確認（デバッグ用）
                var existingConnections = await _supabaseClient
                    .From<ProcessDetailConnectionEntity>()
                    .Where(c => c.FromProcessDetailId == connection.FromProcessDetailId)
                    .Where(c => c.ToProcessDetailId == connection.ToProcessDetailId)
                    .Get();

                if (existingConnections.Models.Any())
                {
                    Debug.WriteLine($"  ⚠️ 警告: 既に同じ接続がデータベースに存在します！");
                    foreach (var existing in existingConnections.Models)
                    {
                        Debug.WriteLine($"    既存: From={existing.FromProcessDetailId}, To={existing.ToProcessDetailId}, CycleId={existing.CycleId}");
                    }
                    throw new InvalidOperationException(
                        $"同じ接続が既にデータベースに存在します。FromProcessDetailId={connection.FromProcessDetailId}, ToProcessDetailId={connection.ToProcessDetailId}");
                }

                var entity = ProcessDetailConnectionEntity.FromDto(connection);
                await _supabaseClient
                    .From<ProcessDetailConnectionEntity>()
                    .Insert(entity);

                System.Diagnostics.Debug.WriteLine("  ✓ 保存成功");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SupabaseRepository] エラー: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  詳細: {ex}");
                throw;
            }
        }

        public async Task DeleteProcessDetailConnectionAsync(int fromProcessDetailId, int toProcessDetailId)
        {
            await _supabaseClient
                .From<ProcessDetailConnectionEntity>()
                .Where(p => p.FromProcessDetailId == fromProcessDetailId)
                .Where(p => p.ToProcessDetailId == toProcessDetailId)
                .Delete();
        }

        public async Task DeleteConnectionsByFromAndToAsync(int fromId, int toId)
        {
            await _supabaseClient
                .From<ProcessDetailConnectionEntity>()
                .Where(p => p.FromProcessDetailId == fromId)
                .Where(p => p.ToProcessDetailId == toId)
                .Delete();
        }

        public async Task DeleteConnectionsByFromIdAsync(int fromProcessDetailId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[SupabaseRepository] DeleteConnectionsByFromIdAsync 開始");
                System.Diagnostics.Debug.WriteLine($"  FromProcessDetailId: {fromProcessDetailId}");

                // 削除前に既存の接続を確認
                var existingConnections = await _supabaseClient
                    .From<ProcessDetailConnectionEntity>()
                    .Where(p => p.FromProcessDetailId == fromProcessDetailId)
                    .Get();

                System.Diagnostics.Debug.WriteLine($"  削除対象の接続数: {existingConnections.Models.Count()}");
                foreach (var conn in existingConnections.Models)
                {
                    System.Diagnostics.Debug.WriteLine($"    削除予定: CycleId={conn.CycleId}, From={conn.FromProcessDetailId}, To={conn.ToProcessDetailId}");
                }

                // 削除実行
                await _supabaseClient
                    .From<ProcessDetailConnectionEntity>()
                    .Where(p => p.FromProcessDetailId == fromProcessDetailId)
                    .Delete();

                System.Diagnostics.Debug.WriteLine("  ✓ 削除完了");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SupabaseRepository] 削除エラー: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteConnectionsByToIdAsync(int toProcessDetailId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[SupabaseRepository] DeleteConnectionsByToIdAsync 開始");
                System.Diagnostics.Debug.WriteLine($"  ToProcessDetailId: {toProcessDetailId}");

                // 削除前に既存の接続を確認
                var existingConnections = await _supabaseClient
                    .From<ProcessDetailConnectionEntity>()
                    .Where(p => p.ToProcessDetailId == toProcessDetailId)
                    .Get();

                System.Diagnostics.Debug.WriteLine($"  削除対象の接続数: {existingConnections.Models.Count()}");
                foreach (var conn in existingConnections.Models)
                {
                    System.Diagnostics.Debug.WriteLine($"    削除予定: CycleId={conn.CycleId}, From={conn.FromProcessDetailId}, To={conn.ToProcessDetailId}");
                }

                // 削除実行
                await _supabaseClient
                    .From<ProcessDetailConnectionEntity>()
                    .Where(p => p.ToProcessDetailId == toProcessDetailId)
                    .Delete();

                System.Diagnostics.Debug.WriteLine("  ✓ 削除完了");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SupabaseRepository] 削除エラー: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ProcessDetailFinish>> GetProcessDetailFinishesAsync(int cycleId)
        {
            // ProcessDetailFinish繝・・繝悶Ν縺ｫ縺ｯCycleId縺後↑縺・◆繧√・
            // ProcessDetail繝・・繝悶Ν縺九ｉ隧ｲ蠖薙☆繧気ycleId縺ｮProcessDetailId繧貞叙蠕励＠縺ｦ繝輔ぅ繝ｫ繧ｿ繝ｪ繝ｳ繧ｰ
            var processDetails = await _supabaseClient
                .From<ProcessDetailEntity>()
                .Where(pd => pd.CycleId == cycleId)
                .Get();

            if (!processDetails.Models.Any())
                return new List<ProcessDetailFinish>();

            var processDetailIds = processDetails.Models.Select(pd => pd.ToDto().Id).ToList();
            var finishes = new List<ProcessDetailFinish>();

            // ProcessDetailId縺斐→縺ｫFinish繧貞叙蠕・
            foreach (var processDetailId in processDetailIds)
            {
                var response = await _supabaseClient
                    .From<ProcessDetailFinishEntity>()
                    .Where(f => f.ProcessDetailId == processDetailId)
                    .Get();

                finishes.AddRange(response.Models.Select(e => e.ToDto()).ToList());
            }

            return finishes;
        }

        public async Task<List<ProcessDetailFinish>> GetAllProcessDetailFinishesAsync()
        {
            var response = await _supabaseClient
                .From<ProcessDetailFinishEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<ProcessDetailFinish>> GetFinishesByProcessDetailIdAsync(int processDetailId)
        {
            var response = await _supabaseClient
                .From<ProcessDetailFinishEntity>()
                .Where(p => p.ProcessDetailId == processDetailId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<ProcessDetailFinish>> GetFinishesByFinishIdAsync(int finishProcessDetailId)
        {
            var response = await _supabaseClient
                .From<ProcessDetailFinishEntity>()
                .Where(p => p.FinishProcessDetailId == finishProcessDetailId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task AddProcessDetailFinishAsync(ProcessDetailFinish finish)
        {
            await _supabaseClient
                .From<ProcessDetailFinishEntity>()
                .Insert(ProcessDetailFinishEntity.FromDto(finish));
        }

        public async Task DeleteProcessDetailFinishAsync(int processDetailId, int finishProcessDetailId)
        {
            await _supabaseClient
                .From<ProcessDetailFinishEntity>()
                .Where(p => p.ProcessDetailId == processDetailId)
                .Where(p => p.FinishProcessDetailId == finishProcessDetailId)
                .Delete();
        }

        public async Task DeleteFinishesByProcessAndFinishAsync(int processDetailId, int finishProcessDetailId)
        {
            await _supabaseClient
                .From<ProcessDetailFinishEntity>()
                .Where(p => p.ProcessDetailId == processDetailId)
                .Where(p => p.FinishProcessDetailId == finishProcessDetailId)
                .Delete();
        }

        public async Task DeleteFinishesByProcessDetailIdAsync(int processDetailId)
        {
            await _supabaseClient
                .From<ProcessDetailFinishEntity>()
                .Where(p => p.ProcessDetailId == processDetailId)
                .Delete();
        }

        #region ProcessStartCondition Methods

        public async Task<List<ProcessStartCondition>> GetProcessStartConditionsAsync(int cycleId)
        {
            // ProcessStartCondition繝・・繝悶Ν縺ｫ縺ｯCycleId縺後↑縺・◆繧√・
            // 縺ｾ縺啀rocess繝・・繝悶Ν縺九ｉ隧ｲ蠖薙☆繧気ycleId縺ｮProcessId繧貞叙蠕励＠縲・
            // 縺昴ｌ繧峨・ProcessId縺ｫ髢｢騾｣縺吶ｋStartCondition繧貞叙蠕励☆繧・
            var processes = await _supabaseClient
                .From<ProcessEntity>()
                .Where(p => p.CycleId == cycleId)
                .Get();

            if (!processes.Models.Any())
                return new List<ProcessStartCondition>();

            var processIds = processes.Models.Select(p => p.ToDto().Id).ToList();
            var conditions = new List<ProcessStartCondition>();

            // ProcessId縺斐→縺ｫStartCondition繧貞叙蠕・
            foreach (var processId in processIds)
            {
                var response = await _supabaseClient
                    .From<ProcessStartConditionEntity>()
                    .Where(c => c.ProcessId == processId)
                    .Get();

                conditions.AddRange(response.Models.Select(e => e.ToDto()).ToList());
            }

            return conditions;
        }

        public async Task<List<ProcessStartCondition>> GetStartConditionsByProcessIdAsync(int processId)
        {
            var response = await _supabaseClient
                .From<ProcessStartConditionEntity>()
                .Where(p => p.ProcessId == processId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task AddProcessStartConditionAsync(ProcessStartCondition condition)
        {
            await _supabaseClient
                .From<ProcessStartConditionEntity>()
                .Insert(ProcessStartConditionEntity.FromDto(condition));
        }

        public async Task UpdateProcessStartConditionAsync(ProcessStartCondition condition)
        {
            await _supabaseClient
                .From<ProcessStartConditionEntity>()
                .Where(p => p.ProcessId == condition.ProcessId)
                .Where(p => p.StartProcessDetailId == condition.StartProcessDetailId)
                .Update(ProcessStartConditionEntity.FromDto(condition));
        }

        public async Task DeleteProcessStartConditionAsync(int processId, int startProcessDetailId)
        {
            await _supabaseClient
                .From<ProcessStartConditionEntity>()
                .Where(p => p.ProcessId == processId)
                .Where(p => p.StartProcessDetailId == startProcessDetailId)
                .Delete();
        }

        public async Task DeleteStartConditionsByProcessAsync(int processId)
        {
            await _supabaseClient
                .From<ProcessStartConditionEntity>()
                .Where(p => p.ProcessId == processId)
                .Delete();
        }

        #endregion

        #region ProcessFinishCondition Methods

        public async Task<List<ProcessFinishCondition>> GetProcessFinishConditionsAsync(int cycleId)
        {
            // ProcessFinishCondition繝・・繝悶Ν縺ｫ繧・ycleId縺後↑縺・◆繧√・
            // ProcessStartCondition縺ｨ蜷梧ｧ倥・蜃ｦ逅・ｒ陦後≧
            var processes = await _supabaseClient
                .From<ProcessEntity>()
                .Where(p => p.CycleId == cycleId)
                .Get();

            if (!processes.Models.Any())
                return new List<ProcessFinishCondition>();

            var processIds = processes.Models.Select(p => p.ToDto().Id).ToList();
            var conditions = new List<ProcessFinishCondition>();

            // ProcessId縺斐→縺ｫFinishCondition繧貞叙蠕・
            foreach (var processId in processIds)
            {
                var response = await _supabaseClient
                    .From<ProcessFinishConditionEntity>()
                    .Where(c => c.ProcessId == processId)
                    .Get();

                conditions.AddRange(response.Models.Select(e => e.ToDto()).ToList());
            }

            return conditions;
        }

        public async Task<List<ProcessFinishCondition>> GetFinishConditionsByProcessIdAsync(int processId)
        {
            var response = await _supabaseClient
                .From<ProcessFinishConditionEntity>()
                .Where(p => p.ProcessId == processId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task AddProcessFinishConditionAsync(ProcessFinishCondition condition)
        {
            await _supabaseClient
                .From<ProcessFinishConditionEntity>()
                .Insert(ProcessFinishConditionEntity.FromDto(condition));
        }

        public async Task UpdateProcessFinishConditionAsync(ProcessFinishCondition condition)
        {
            await _supabaseClient
                .From<ProcessFinishConditionEntity>()
                .Where(p => p.ProcessId == condition.ProcessId)
                .Where(p => p.FinishProcessDetailId == condition.FinishProcessDetailId)
                .Update(ProcessFinishConditionEntity.FromDto(condition));
        }

        public async Task DeleteProcessFinishConditionAsync(int processId, int finishProcessDetailId)
        {
            await _supabaseClient
                .From<ProcessFinishConditionEntity>()
                .Where(p => p.ProcessId == processId)
                .Where(p => p.FinishProcessDetailId == finishProcessDetailId)
                .Delete();
        }

        public async Task DeleteFinishConditionsByProcessAsync(int processId)
        {
            await _supabaseClient
                .From<ProcessFinishConditionEntity>()
                .Where(p => p.ProcessId == processId)
                .Delete();
        }

        #endregion

        #region CylinderIO Methods

        public async Task<List<CylinderIO>> GetCylinderIOsAsync(int cylinderId, int plcId)
        {
            var response = await _supabaseClient
                .From<CylinderIOEntity>()
                .Where(c => c.CylinderId == cylinderId)
                .Where(c => c.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<CylinderIO>> GetIOCylindersAsync(string ioAddress, int plcId)
        {
            var response = await _supabaseClient
                .From<CylinderIOEntity>()
                .Where(c => c.IOAddress == ioAddress)
                .Where(c => c.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task AddCylinderIOAssociationAsync(int cylinderId, string ioAddress, int plcId, string ioType, string? comment = null)
        {
            var cylinderIO = new CylinderIO
            {
                CylinderId = cylinderId,
                Address = ioAddress,
                PlcId = plcId,
                IOType = ioType,
                Comment = comment
            };
            await _supabaseClient
                .From<CylinderIOEntity>()
                .Insert(CylinderIOEntity.FromDto(cylinderIO));
        }

        public async Task RemoveCylinderIOAssociationAsync(int cylinderId, string ioAddress, int plcId)
        {
            await _supabaseClient
                .From<CylinderIOEntity>()
                .Where(c => c.CylinderId == cylinderId)
                .Where(c => c.IOAddress == ioAddress)
                .Where(c => c.PlcId == plcId)
                .Delete();
        }

        public async Task<List<CylinderIO>> GetAllCylinderIOAssociationsAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<CylinderIOEntity>()
                .Where(c => c.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        #endregion

        #region OperationIO Methods

        public async Task<List<OperationIO>> GetOperationIOsAsync(int operationId)
        {
            var response = await _supabaseClient
                .From<OperationIOEntity>()
                .Where(o => o.OperationId == operationId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<OperationIO>> GetIOOperationsAsync(string ioAddress, int plcId)
        {
            var response = await _supabaseClient
                .From<OperationIOEntity>()
                .Where(o => o.IOAddress == ioAddress)
                .Where(o => o.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task AddOperationIOAssociationAsync(int operationId, string ioAddress, int plcId, string ioUsage, string? comment = null)
        {
            var operationIO = new OperationIO
            {
                OperationId = operationId,
                Address = ioAddress,
                PlcId = plcId,
                Usage = ioUsage,
                Comment = comment
            };
            await _supabaseClient.From<OperationIOEntity>().Insert(OperationIOEntity.FromDto(operationIO));
        }

        public async Task RemoveOperationIOAssociationAsync(int operationId, string ioAddress, int plcId)
        {
            await _supabaseClient
                .From<OperationIOEntity>()
                .Where(o => o.OperationId == operationId)
                .Where(o => o.IOAddress == ioAddress)
                .Where(o => o.PlcId == plcId)
                .Delete();
        }

        public async Task<List<OperationIO>> GetAllOperationIOAssociationsAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<OperationIOEntity>()
                .Where(o => o.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        #endregion

        #region Error Methods

        public async Task DeleteErrorTableAsync()
        {
            await _supabaseClient
                .From<ProcessErrorEntity>()
                .Filter("PlcId", Postgrest.Constants.Operator.GreaterThanOrEqual, "0") // 蜈ｨ蜑企勁縺ｮ縺溘ａ縺ｮ譚｡莉ｶ
                .Delete();
        }

        public async Task<List<ErrorMessage>> GetErrorMessagesAsync(int mnemonicId)
        {
            var response = await _supabaseClient
                .From<ErrorMessageEntity>()
                .Where(e => e.MnemonicId == mnemonicId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<ProcessError>> GetErrorsAsync(int plcId, int cycleId, int mnemonicId)
        {
            var response = await _supabaseClient
                .From<ProcessErrorEntity>()
                .Where(e => e.PlcId == plcId)
                .Where(e => e.CycleId == cycleId)
                .Where(e => e.MnemonicId == mnemonicId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task SaveErrorsAsync(List<ProcessError> errors)
        {
            if (errors == null || !errors.Any()) return;

            var errorsToInsert = errors.Select(e => new ProcessError
            {
                PlcId = e.PlcId,
                CycleId = e.CycleId,
                Device = e.Device,
                MnemonicId = e.MnemonicId,
                RecordId = e.RecordId,
                AlarmId = e.AlarmId,
                ErrorNum = e.ErrorNum,
                Comment1 = e.Comment1,
                Comment2 = e.Comment2,
                Comment3 = e.Comment3,
                Comment4 = e.Comment4,
                AlarmComment = e.AlarmComment,
                MessageComment = e.MessageComment,
                ErrorTime = e.ErrorTime,
                ErrorTimeDevice = e.ErrorTimeDevice
            }).ToList();

            await _supabaseClient
                .From<ProcessErrorEntity>()
                .Insert(errorsToInsert.Select(e => ProcessErrorEntity.FromDto(e)).ToList());  // 荳諡ｬ謖ｿ蜈･
        }



        public async Task UpdateErrorsAsync(List<ProcessError> errors)
        {
            if (errors == null || !errors.Any())
                return;
            foreach (var error in errors)
            {
                await _supabaseClient
                    .From<ProcessErrorEntity>()
                    .Where(e => e.PlcId == error.PlcId)
                    .Where(e => e.AlarmId == error.AlarmId)
                    .Update(ProcessErrorEntity.FromDto(error));
            }
        }


        #endregion

        #region MnemonicDevice Methods

        public async Task<List<MnemonicDevice>> GetMnemonicDevicesAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<MnemonicDeviceEntity>()
                .Where(m => m.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MnemonicDevice>> GetMnemonicDevicesByMnemonicAsync(int plcId, int mnemonicId)
        {
            var response = await _supabaseClient
                .From<MnemonicDeviceEntity>()
                .Where(m => m.PlcId == plcId)
                .Where(m => m.MnemonicId == mnemonicId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task DeleteMnemonicDeviceAsync(int plcId, int mnemonicId)
        {
            await _supabaseClient
                .From<MnemonicDeviceEntity>()
                .Where(m => m.PlcId == plcId)
                .Where(m => m.MnemonicId == mnemonicId)
                .Delete();
        }

        public async Task DeleteAllMnemonicDevicesAsync()
        {
            // Supabase requires a WHERE clause for DELETE. To delete all records, we use a condition that's always true
            await _supabaseClient
                .From<MnemonicDeviceEntity>()
                .Filter("PlcId", Postgrest.Constants.Operator.GreaterThanOrEqual, "0")
                .Delete();
        }

        public async Task SaveOrUpdateMnemonicDeviceAsync(MnemonicDevice device)
        {
            // Supabase縺ｮupsert讖溯・繧剃ｽｿ逕ｨ縲１lcId, MnemonicId, RecordId縺ｮ隍・粋荳ｻ繧ｭ繝ｼ縺ｫ蝓ｺ縺･縺・
            var entity = MnemonicDeviceEntity.FromDto(device);
            await _supabaseClient
                .From<MnemonicDeviceEntity>()
                .Upsert(entity);
        }

        #endregion

        #region MnemonicSpeedDevice Methods

        public async Task<List<MnemonicSpeedDevice>> GetMnemonicSpeedDevicesAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<MnemonicSpeedDeviceEntity>()
                .Where(m => m.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task DeleteSpeedTableAsync()
        {
            // Supabase requires a WHERE clause for DELETE. To delete all records, we use a condition that's always true
            await _supabaseClient
                .From<MnemonicSpeedDeviceEntity>()
                .Filter("ID", Postgrest.Constants.Operator.GreaterThanOrEqual, "0")
                .Delete();
        }

        public async Task SaveOrUpdateMnemonicSpeedDeviceAsync(MnemonicSpeedDevice device)
        {
            // Supabase縺ｮupsert讖溯・繧剃ｽｿ逕ｨ
            var entity = MnemonicSpeedDeviceEntity.FromDto(device);
            await _supabaseClient
                .From<MnemonicSpeedDeviceEntity>()
                .Upsert(entity);
        }

        #endregion

        #region ProsTime Methods

        public async Task<List<ProsTime>> GetProsTimeByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<ProsTimeEntity>()
                .Where(p => p.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<ProsTime>> GetProsTimeByMnemonicIdAsync(int plcId, int mnemonicId)
        {
            try
            {

                var response = await _supabaseClient
                    .From<ProsTimeEntity>()
                    .Where(p => p.PlcId == plcId)
                    .Where(p => p.MnemonicId == mnemonicId)
                    .Get();

                var prosTimes = response?.Models?.Select(e => e.ToDto()).ToList() ?? new List<ProsTime>();

                return prosTimes;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetProsTimeByMnemonicIdAsync] 繧ｨ繝ｩ繝ｼ: {ex.Message}");
                return new List<ProsTime>();
            }
        }

        public async Task DeleteProsTimeTableAsync()
        {
            // Supabase requires a WHERE clause for DELETE. To delete all records, we use a condition that's always true
            await _supabaseClient
                .From<ProsTimeEntity>()
                .Filter("SortId", Postgrest.Constants.Operator.GreaterThanOrEqual, "0")
                .Delete();
        }

        public async Task SaveOrUpdateProsTimeAsync(ProsTime prosTime)
        {
            // Supabase縺ｮupsert讖溯・繧剃ｽｿ逕ｨ縲１lcId, MnemonicId, RecordId, SortId縺ｮ隍・粋荳ｻ繧ｭ繝ｼ縺ｫ蝓ｺ縺･縺・
            var entity = ProsTimeEntity.FromDto(prosTime);
            await _supabaseClient
                .From<ProsTimeEntity>()
                .Upsert(entity);
        }

        public async Task SaveOrUpdateProsTimesBatchAsync(List<ProsTime> prosTimes)
        {
            if (prosTimes == null || !prosTimes.Any())
                return;

            // Supabase縺ｮupsert讖溯・繧剃ｽｿ逕ｨ縺励※荳諡ｬ譖ｴ譁ｰ
            var entities = prosTimes.Select(pt => ProsTimeEntity.FromDto(pt)).ToList();
            await _supabaseClient
                .From<ProsTimeEntity>()
                .Upsert(entities);
        }

        public async Task<List<ProsTimeDefinitions>> GetProsTimeDefinitionsAsync()
        {
            try
            {
                var response = await _supabaseClient
                    .From<ProsTimeDefinitionsEntity>()
                    .Get();

                return response?.Models?.Select(e => e.ToDto()).ToList() ?? new List<ProsTimeDefinitions>();
            }
            catch (Exception ex)
            {
                // 繧ｨ繝ｩ繝ｼ: {ex.Message}
                Debug.WriteLine($"[GetProsTimeDefinitionsAsync] 繧ｨ繝ｩ繝ｼ: {ex.Message}");
                return new List<ProsTimeDefinitions>();
            }
        }

        #endregion

        #region Memory Methods

        public async Task<List<Memory>> GetMemoriesAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<MemoryEntity>()
                .Where(m => m.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<MemoryCategory>> GetMemoryCategoriesAsync()
        {
            var response = await _supabaseClient
                .From<MemoryCategoryEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task SaveOrUpdateMemoryAsync(Memory memory)
        {
            // Supabase縺ｮupsert讖溯・繧剃ｽｿ逕ｨ縲１lcId縺ｨDevice縺ｮ隍・粋荳ｻ繧ｭ繝ｼ縺ｫ蝓ｺ縺･縺・
            var entity = MemoryEntity.FromDto(memory);
            await _supabaseClient
                .From<MemoryEntity>()
                .Upsert(entity);
        }

        public async Task SaveOrUpdateMemoriesBatchAsync(List<Memory> memories)
        {
            if (memories == null || !memories.Any())
                return;

            // Supabase縺ｮupsert讖溯・繧剃ｽｿ逕ｨ縺励※荳諡ｬ譖ｴ譁ｰ縲１lcId縺ｨDevice縺ｮ隍・粋荳ｻ繧ｭ繝ｼ縺ｫ蝓ｺ縺･縺・
            // Supabase縺ｯ隍・焚繝ｬ繧ｳ繝ｼ繝峨・荳諡ｬupsert繧偵し繝昴・繝・
            var entities = memories.Select(m => MemoryEntity.FromDto(m)).ToList();
            await _supabaseClient
                .From<MemoryEntity>()
                .Upsert(entities);
        }

        public async Task DeleteMemoriesByPlcIdAsync(int plcId)
        {
            // 指定されたPLC IDに紐づくMemoryテーブルの全レコードを削除
            await _supabaseClient
                .From<MemoryEntity>()
                .Where(m => m.PlcId == plcId)
                .Delete();
        }

        #endregion

        #region Definitions Methods

        public async Task<List<Definitions>> GetDefinitionsAsync(string category)
        {
            var response = await _supabaseClient
                .From<DefinitionsEntity>()
                .Where(d => d.Category == category)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<Definitions?> GetDefinitionAsync(string category)
        {
            var response = await _supabaseClient
                .From<DefinitionsEntity>()
                .Where(d => d.Category == category)
                .Single();
            return response?.ToDto();
        }

        #endregion

        #region Interlock Methods

        public async Task<List<Interlock>> GetInterlocksByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<InterlockEntity>()
                .Where(i => i.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<Interlock>> GetInterlocksByCylindrIdAsync(int cylinderId)
        {
            var response = await _supabaseClient
    .From<InterlockEntity>()
    .Where(i => i.CylinderId == cylinderId)
    .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task UpsertInterlockAsync(Interlock interlock)
        {
            var entity = InterlockEntity.FromDto(interlock);
            await _supabaseClient
                .From<InterlockEntity>()
                .Upsert(entity);
        }

        public async Task UpsertInterlocksAsync(List<Interlock> interlocks)
        {
            // 重複を除去（同じPlcId + CylinderId + SortIdを持つレコードは1つだけ保持）
            var uniqueInterlocks = interlocks
                .GroupBy(i => new { i.PlcId, i.CylinderId, i.SortId })
                .Select(g => g.First()) // 最初のレコードを使用
                .ToList();

            // 既存レコードを取得して、Insert/Updateを判断
            var plcIds = uniqueInterlocks.Select(i => i.PlcId).Distinct().ToList();
            if (!plcIds.Any()) return;

            var existingRecords = new List<Interlock>();
            foreach (var plcId in plcIds)
            {
                var records = await _supabaseClient
                    .From<InterlockEntity>()
                    .Where(i => i.PlcId == plcId)
                    .Get();
                existingRecords.AddRange(records.Models.Select(e => e.ToDto()));
            }

            // 新規作成と更新に分ける
            var toInsert = new List<Interlock>();
            var toUpdate = new List<Interlock>();

            foreach (var interlock in uniqueInterlocks)
            {
                var existing = existingRecords.FirstOrDefault(e =>
                    e.PlcId == interlock.PlcId &&
                    e.CylinderId == interlock.CylinderId &&
                    e.SortId == interlock.SortId);

                if (existing != null)
                {
                    // 既存レコードを更新
                    toUpdate.Add(interlock);
                }
                else
                {
                    // 新規作成
                    toInsert.Add(interlock);
                }
            }

            // 新規作成: Insert
            if (toInsert.Any())
            {
                var newEntities = toInsert.Select(i => InterlockEntity.FromDto(i)).ToList();
                await _supabaseClient
                    .From<InterlockEntity>()
                    .Insert(newEntities);
            }

            // 既存更新: Update (1件ずつ更新)
            if (toUpdate.Any())
            {
                foreach (var interlock in toUpdate)
                {
                    var entity = InterlockEntity.FromDto(interlock);
                    await _supabaseClient
                        .From<InterlockEntity>()
                        .Where(i => i.CylinderId == entity.CylinderId)
                        .Where(i => i.SortId == entity.SortId)
                        .Update(entity);
                }
            }
        }

        public async Task DeleteInterlockAsync(Interlock interlock)
        {
            await _supabaseClient
                .From<InterlockEntity>()
                .Where(i => i.CylinderId == interlock.CylinderId)
                .Where(i => i.SortId == interlock.SortId)
                .Delete();
        }

        public async Task DeleteInterlocksAsync(List<Interlock> interlocks)
        {
            foreach (var interlock in interlocks)
            {
                await _supabaseClient
                    .From<InterlockEntity>()
                    .Where(i => i.CylinderId == interlock.CylinderId)
                    .Where(i => i.SortId == interlock.SortId)
                    .Delete();
            }
        }

        // InterlockCondition
        public async Task<List<InterlockConditionDTO>> GetInterlockConditionsByInterlockIdAsync(int interlockId)
        {
            var response = await _supabaseClient
                .From<InterlockConditionDTOEntity>()
                .Where(ic => ic.InterlockId == interlockId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task UpsertInterlockConditionAsync(InterlockConditionDTO interlockCondition)
        {
            // Create a clean copy without navigation properties
            var cleanCondition = new InterlockCondition
            {
                InterlockId = interlockCondition.InterlockId,
                ConditionNumber = interlockCondition.ConditionNumber,
                InterlockSortId = interlockCondition.InterlockSortId,
                ConditionTypeId = interlockCondition.ConditionTypeId,
                Name = interlockCondition.Name,
                Device = interlockCondition.Device,
                IsOnCondition = interlockCondition.IsOnCondition
                // ConditionType is intentionally not copied
            };

            var entity = InterlockConditionEntity.FromDto(cleanCondition);
            await _supabaseClient
                .From<InterlockConditionEntity>()
                .Upsert(entity);
        }

        public async Task UpsertInterlockConditionsAsync(List<InterlockConditionDTO> interlockConditions)
        {
            // Create clean copies without navigation properties
            var cleanConditions = interlockConditions.Select(c => new InterlockCondition
            {
                InterlockId = c.InterlockId,
                ConditionNumber = c.ConditionNumber,
                InterlockSortId = c.InterlockSortId,
                ConditionTypeId = c.ConditionTypeId,
                Name = c.Name,
                Device = c.Device,
                IsOnCondition = c.IsOnCondition
                // ConditionType is intentionally not copied
            }).ToList();

            // 重複を除去（同じInterlockId + ConditionNumber + InterlockSortIdを持つレコードは1つだけ保持）
            var uniqueConditions = cleanConditions
                .GroupBy(c => new { c.InterlockId, c.ConditionNumber, c.InterlockSortId })
                .Select(g => g.First()) // 最初のレコードを使用
                .ToList();

            // 既存レコードを取得して、Insert/Updateを判断
            var interlockIds = uniqueConditions.Select(c => c.InterlockId).Distinct().ToList();
            if (!interlockIds.Any()) return;

            var existingRecords = new List<InterlockCondition>();
            foreach (var interlockId in interlockIds)
            {
                var records = await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Where(c => c.InterlockId == interlockId)
                    .Get();
                existingRecords.AddRange(records.Models.Select(e => e.ToDto()));
            }

            // 新規作成と更新に分ける
            var toInsert = new List<InterlockCondition>();
            var toUpdate = new List<InterlockCondition>();

            foreach (var condition in uniqueConditions)
            {
                var existing = existingRecords.FirstOrDefault(e =>
                    e.InterlockId == condition.InterlockId &&
                    e.ConditionNumber == condition.ConditionNumber &&
                    e.InterlockSortId == condition.InterlockSortId);

                if (existing != null)
                {
                    // 既存レコードを更新
                    toUpdate.Add(condition);
                }
                else
                {
                    // 新規作成
                    toInsert.Add(condition);
                }
            }

            // 新規作成: Insert
            if (toInsert.Any())
            {
                var newEntities = toInsert.Select(c => InterlockConditionEntity.FromDto(c)).ToList();
                await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Insert(newEntities);
            }

            // 既存更新: Update (1件ずつ更新)
            if (toUpdate.Any())
            {
                foreach (var condition in toUpdate)
                {
                    var entity = InterlockConditionEntity.FromDto(condition);
                    await _supabaseClient
                        .From<InterlockConditionEntity>()
                        .Where(c => c.InterlockId == entity.InterlockId)
                        .Where(c => c.ConditionNumber == entity.ConditionNumber)
                        .Where(c => c.InterlockSortId == entity.InterlockSortId)
                        .Update(entity);
                }
            }
        }

        public async Task DeleteInterlockConditionAsync(InterlockConditionDTO interlockCondition)
        {
            await _supabaseClient
                .From<InterlockConditionEntity>()
                .Where(ic => ic.InterlockId == interlockCondition.InterlockId)
                .Where(ic => ic.ConditionNumber == interlockCondition.ConditionNumber)
                .Where(ic => ic.InterlockSortId == interlockCondition.InterlockSortId)
                .Delete();
        }

        public async Task DeleteInterlockConditionsAsync(List<InterlockConditionDTO> interlockConditions)
        {
            foreach (var condition in interlockConditions)
            {
                await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Where(ic => ic.InterlockId == condition.InterlockId)
                    .Where(ic => ic.ConditionNumber == condition.ConditionNumber)
                    .Where(ic => ic.InterlockSortId == condition.InterlockSortId)
                    .Delete();
            }
        }

        public async Task<List<ViewInterlockConditions>> GetViewInterlockConditionsByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<ViewInterlockConditionsEntity>()
                .Where(vic => vic.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        // InterlockIO
        public async Task<List<InterlockIO>> GetInterlockIOsByInterlockIdAsync(int interlockId)
        {
            var response = await _supabaseClient
                .From<InterlockIOEntity>()
                .Where(i => i.InterlockId == interlockId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<InterlockIO>> GetIOInterlocksAsync(string ioAddress, int plcId)
        {
            var response = await _supabaseClient
                .From<InterlockIOEntity>()
                .Where(i => i.IOAddress == ioAddress)
                .Where(i => i.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task AddInterlockIOAssociationAsync(InterlockIO interlockIO)
        {
            await _supabaseClient
                .From<InterlockIOEntity>()
                .Insert(InterlockIOEntity.FromDto(interlockIO));
        }
        public async Task DeleteInterlockIOAsync(InterlockIO interlockIO)
        {
            await _supabaseClient
                .From<InterlockIOEntity>()
                .Where(i => i.InterlockId == interlockIO.InterlockId)
                .Where(i => i.PlcId == interlockIO.PlcId)
                .Where(i => i.IOAddress == interlockIO.IOAddress)
                .Where(i => i.InterlockSortId == interlockIO.InterlockSortId)
                .Where(i => i.ConditionNumber == interlockIO.ConditionNumber)
                .Delete();
        }

        public async Task<List<InterlockConditionType>> GetInterlockConditionTypesAsync()
        {
            var response = await _supabaseClient
                .From<InterlockConditionTypeEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<List<InterlockPrecondition1>> GetInterlockPrecondition1ListAsync()
        {
            var response = await _supabaseClient
                .From<InterlockPrecondition1Entity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task UpsertInterlockPrecondition1ListAsync(List<InterlockPrecondition1> preconditions)
        {
            foreach (var precondition in preconditions)
            {
                var entity = InterlockPrecondition1Entity.FromDto(precondition);
                await _supabaseClient
                    .From<InterlockPrecondition1Entity>()
                    .Upsert(entity);
            }
        }

        public async Task<List<InterlockPrecondition2>> GetInterlockPrecondition2ListAsync()
        {
            var response = await _supabaseClient
                .From<InterlockPrecondition2Entity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task UpsertInterlockPrecondition2ListAsync(List<InterlockPrecondition2> preconditions)
        {
            foreach (var precondition in preconditions)
            {
                var entity = InterlockPrecondition2Entity.FromDto(precondition);
                await _supabaseClient
                    .From<InterlockPrecondition2Entity>()
                    .Upsert(entity);
            }
        }

        #endregion

        #region MemoryProfile

        public async Task<List<MemoryProfile>> GetMemoryProfilesAsync()
        {
            var response = await _supabaseClient
                .From<MemoryProfileEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        public async Task<MemoryProfile?> GetMemoryProfileByCycleIdAsync(int cycleId)
        {
            var response = await _supabaseClient
                .From<MemoryProfileEntity>()
                .Where(m => m.CycleId == cycleId)
                .Single();
            return response?.ToDto();
        }

        public async Task AddMemoryProfileAsync(MemoryProfile profile)
        {
            var entity = MemoryProfileEntity.FromDtoForInsert(profile);
            await _supabaseClient
                .From<MemoryProfileEntityForInsert>()
                .Insert(entity);
        }

        public async Task UpdateMemoryProfileAsync(MemoryProfile profile)
        {
            var entity = MemoryProfileEntity.FromDto(profile);
            entity.UpdatedAt = DateTime.UtcNow;
            await _supabaseClient
                .From<MemoryProfileEntity>()
                .Where(m => m.CycleId == profile.CycleId)
                .Update(entity);
        }

        public async Task DeleteMemoryProfileAsync(int cycleId)
        {
            await _supabaseClient
                .From<MemoryProfileEntity>()
                .Where(m => m.CycleId == cycleId)
                .Delete();
        }

        #endregion

    }
}

