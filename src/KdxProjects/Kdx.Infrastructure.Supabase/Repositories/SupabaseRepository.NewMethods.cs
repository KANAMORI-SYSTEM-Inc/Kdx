using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Entities;
using Supabase;

namespace Kdx.Infrastructure.Supabase.Repositories
{
    /// <summary>
    /// 新規追加メソッドの実装
    /// </summary>
    public partial class SupabaseRepository
    {
        #region Company CRUD

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<CompanyEntity>()
                .Where(c => c.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<int> AddCompanyAsync(Company company)
        {
            var entity = CompanyEntity.FromDto(company);
            var response = await _supabaseClient
                .From<CompanyEntity>()
                .Insert(entity);
            return response.Models.FirstOrDefault()?.ToDto().Id ?? 0;
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            var entity = CompanyEntity.FromDto(company);
            await _supabaseClient
                .From<CompanyEntity>()
                .Where(c => c.Id == company.Id)
                .Update(entity);
        }

        public async Task DeleteCompanyAsync(int id)
        {
            await _supabaseClient
                .From<CompanyEntity>()
                .Where(c => c.Id == id)
                .Delete();
        }

        #endregion

        #region Model CRUD

        public async Task<Model?> GetModelByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<ModelEntity>()
                .Where(m => m.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<int> AddModelAsync(Model model)
        {
            var entity = ModelEntity.FromDto(model);
            var response = await _supabaseClient
                .From<ModelEntity>()
                .Insert(entity);
            return response.Models.FirstOrDefault()?.ToDto().Id ?? 0;
        }

        public async Task UpdateModelAsync(Model model)
        {
            var entity = ModelEntity.FromDto(model);
            await _supabaseClient
                .From<ModelEntity>()
                .Where(m => m.Id == model.Id)
                .Update(entity);
        }

        public async Task DeleteModelAsync(int id)
        {
            await _supabaseClient
                .From<ModelEntity>()
                .Where(m => m.Id == id)
                .Delete();
        }

        #endregion

        #region PLC CRUD

        public async Task<PLC?> GetPLCByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<PLCEntity>()
                .Where(p => p.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<int> AddPLCAsync(PLC plc)
        {
            var entity = PLCEntity.FromDto(plc);
            var response = await _supabaseClient
                .From<PLCEntity>()
                .Insert(entity);
            return response.Models.FirstOrDefault()?.ToDto().Id ?? 0;
        }

        public async Task UpdatePLCAsync(PLC plc)
        {
            var entity = PLCEntity.FromDto(plc);
            await _supabaseClient
                .From<PLCEntity>()
                .Where(p => p.Id == plc.Id)
                .Update(entity);
        }

        public async Task DeletePLCAsync(int id)
        {
            await _supabaseClient
                .From<PLCEntity>()
                .Where(p => p.Id == id)
                .Delete();
        }

        #endregion

        #region Cycle CRUD

        public async Task<Cycle?> GetCycleByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<CycleEntity>()
                .Where(c => c.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<int> AddCycleAsync(Cycle cycle)
        {
            var entity = CycleEntity.FromDto(cycle);
            var response = await _supabaseClient
                .From<CycleEntity>()
                .Insert(entity);
            return response.Models.FirstOrDefault()?.ToDto().Id ?? 0;
        }

        public async Task UpdateCycleAsync(Cycle cycle)
        {
            var entity = CycleEntity.FromDto(cycle);
            await _supabaseClient
                .From<CycleEntity>()
                .Where(c => c.Id == cycle.Id)
                .Update(entity);
        }

        public async Task DeleteCycleAsync(int id)
        {
            await _supabaseClient
                .From<CycleEntity>()
                .Where(c => c.Id == id)
                .Delete();
        }

        #endregion

        #region Machine CRUD

        public async Task<int> AddMachineAsync(Machine machine)
        {
            var entity = MachineEntity.FromDto(machine);
            var response = await _supabaseClient
                .From<MachineEntity>()
                .Insert(entity);
            // Machine doesn't have Id, returning MachineNameId as workaround
            return response.Models.FirstOrDefault()?.ToDto().MachineNameId ?? 0;
        }

        public async Task UpdateMachineAsync(Machine machine)
        {
            var entity = MachineEntity.FromDto(machine);
            await _supabaseClient
                .From<MachineEntity>()
                .Where(m => m.MachineNameId == machine.MachineNameId && m.DriveSubId == machine.DriveSubId)
                .Update(entity);
        }

        public async Task DeleteMachineAsync(int nameId, int driveSubId)
        {
            await _supabaseClient
                .From<MachineEntity>()
                .Where(m => m.MachineNameId == nameId && m.DriveSubId == driveSubId)
                .Delete();
        }

        #endregion

        #region MachineName CRUD

        public async Task<int> AddMachineNameAsync(MachineName machineName)
        {
            var entity = MachineNameEntity.FromDto(machineName);
            var response = await _supabaseClient
                .From<MachineNameEntity>()
                .Insert(entity);
            return response.Models.FirstOrDefault()?.ToDto().Id ?? 0;
        }

        public async Task UpdateMachineNameAsync(MachineName machineName)
        {
            var entity = MachineNameEntity.FromDto(machineName);
            await _supabaseClient
                .From<MachineNameEntity>()
                .Where(m => m.Id == machineName.Id)
                .Update(entity);
        }

        public async Task DeleteMachineNameAsync(int id)
        {
            await _supabaseClient
                .From<MachineNameEntity>()
                .Where(m => m.Id == id)
                .Delete();
        }

        #endregion

        #region DriveMain CRUD

        public async Task<DriveMain?> GetDriveMainByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<DriveMainEntity>()
                .Where(d => d.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<int> AddDriveMainAsync(DriveMain driveMain)
        {
            var entity = DriveMainEntity.FromDto(driveMain);
            var response = await _supabaseClient
                .From<DriveMainEntity>()
                .Insert(entity);
            return response.Models.FirstOrDefault()?.ToDto().Id ?? 0;
        }

        public async Task UpdateDriveMainAsync(DriveMain driveMain)
        {
            var entity = DriveMainEntity.FromDto(driveMain);
            await _supabaseClient
                .From<DriveMainEntity>()
                .Where(d => d.Id == driveMain.Id)
                .Update(entity);
        }

        public async Task DeleteDriveMainAsync(int id)
        {
            await _supabaseClient
                .From<DriveMainEntity>()
                .Where(d => d.Id == id)
                .Delete();
        }

        #endregion

        #region DriveSub CRUD

        public async Task<int> AddDriveSubAsync(DriveSub driveSub)
        {
            var entity = DriveSubEntity.FromDto(driveSub);
            var response = await _supabaseClient
                .From<DriveSubEntity>()
                .Insert(entity);
            return response.Models.FirstOrDefault()?.ToDto().Id ?? 0;
        }

        public async Task UpdateDriveSubAsync(DriveSub driveSub)
        {
            var entity = DriveSubEntity.FromDto(driveSub);
            await _supabaseClient
                .From<DriveSubEntity>()
                .Where(d => d.Id == driveSub.Id)
                .Update(entity);
        }

        public async Task DeleteDriveSubAsync(int id)
        {
            await _supabaseClient
                .From<DriveSubEntity>()
                .Where(d => d.Id == id)
                .Delete();
        }

        #endregion

        #region Cylinder CRUD

        public async Task<int> AddCylinderAsync(Cylinder cylinder)
        {
            // PostgreSQL RPC関数を使用してINSERTを実行し、新しいIDを取得
            var result = await _supabaseClient.Rpc<long>("insert_cylinder", new
            {
                plc_id = cylinder.PlcId,
                puco = cylinder.PUCO,
                cynum = cylinder.CYNum,
                go = cylinder.Go,
                back = cylinder.Back,
                oil_num = cylinder.OilNum,
                machine_name_id = cylinder.MachineNameId,
                drive_sub_id = cylinder.DriveSubId,
                place_id = cylinder.PlaceId,
                cyname_sub = cylinder.CYNameSub,
                sensor_id = cylinder.SensorId,
                flow_type = cylinder.FlowType,
                go_sensor_count = cylinder.GoSensorCount,
                back_sensor_count = cylinder.BackSensorCount,
                retention_sensor_go = cylinder.RetentionSensorGo,
                retention_sensor_back = cylinder.RetentionSensorBack,
                sort_number = cylinder.SortNumber,
                flow_count = cylinder.FlowCount,
                flow_cygo = cylinder.FlowCYGo,
                flow_cyback = cylinder.FlowCYBack
            });
            return (int)result;
        }

        public async Task UpdateCylinderAsync(Cylinder cylinder)
        {
            var entity = CylinderEntity.FromDto(cylinder);
            await _supabaseClient
                .From<CylinderEntity>()
                .Where(c => c.Id == cylinder.Id)
                .Update(entity);
        }

        public async Task DeleteCylinderAsync(int id)
        {
            await _supabaseClient
                .From<CylinderEntity>()
                .Where(c => c.Id == id)
                .Delete();
        }

        #endregion

        #region Timer CRUD

        public async Task<Contracts.DTOs.Timer?> GetTimerByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<TimerEntity>()
                .Where(t => t.ID == id)
                .Single();
            return response?.ToDto();
        }

        #endregion

        #region Process CRUD

        public async Task<Process?> GetProcessByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<ProcessEntity>()
                .Where(p => p.Id == id)
                .Single();
            return response?.ToDto();
        }

        #endregion

        #region ProcessDetail CRUD

        public async Task<ProcessDetail?> GetProcessDetailByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<ProcessDetailEntity>()
                .Where(p => p.Id == id)
                .Single();
            return response?.ToDto();
        }

        #endregion

        #region Category Methods

        public async Task<ProcessCategory?> GetProcessCategoryByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<ProcessCategoryEntity>()
                .Where(c => c.Id == id)
                .Single();
            return response?.ToDto();
        }

        public async Task<OperationCategory?> GetOperationCategoryByIdAsync(int id)
        {
            var response = await _supabaseClient
                .From<OperationCategoryEntity>()
                .Where(c => c.Id == id)
                .Single();
            return response?.ToDto();
        }

        #endregion

        #region IO Methods

        public async Task<IO?> GetIoByIdAsync(int id)
        {
            // Note: IO entity has composite key (Address + PlcId), not a single Id
            // This method signature doesn't match the entity structure
            // Implementing with PlcId as workaround
            var response = await _supabaseClient
                .From<IOEntity>()
                .Where(io => io.PlcId == id)
                .Get();
            return response.Models.FirstOrDefault()?.ToDto();
        }

        #endregion
    }
}
