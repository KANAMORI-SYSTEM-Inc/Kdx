using Kdx.Contracts.DTOs;
using Timer = Kdx.Contracts.DTOs.Timer;

namespace Kdx.Infrastructure.Supabase.Repositories
{
    /// <summary>
    /// Supabaseデータベースへのアクセス機能を提供するリポジトリのインターフェース。
    /// </summary>
    public interface ISupabaseRepository
    {
        #region Company

        /// <summary>
        /// 全ての会社情報を取得します。
        /// </summary>
        /// <returns>会社情報のリスト。</returns>
        Task<List<Company>> GetCompaniesAsync();

        /// <summary>
        /// 指定されたIDの会社情報を取得します。
        /// </summary>
        /// <param name="id">会社ID。</param>
        /// <returns>会社情報。見つからない場合はnull。</returns>
        Task<Company?> GetCompanyByIdAsync(int id);

        /// <summary>
        /// 新しい会社情報を追加します。
        /// </summary>
        /// <param name="company">追加する会社情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddCompanyAsync(Company company);

        /// <summary>
        /// 会社情報を更新します。
        /// </summary>
        /// <param name="company">更新する会社情報。</param>
        Task UpdateCompanyAsync(Company company);

        /// <summary>
        /// 会社情報を削除します。
        /// </summary>
        /// <param name="id">削除する会社ID。</param>
        Task DeleteCompanyAsync(int id);

        #endregion

        #region Model

        /// <summary>
        /// 全ての機種情報を取得します。
        /// </summary>
        /// <returns>機種情報のリスト。</returns>
        Task<List<Model>> GetModelsAsync();

        /// <summary>
        /// 指定されたIDの機種情報を取得します。
        /// </summary>
        /// <param name="id">機種ID。</param>
        /// <returns>機種情報。見つからない場合はnull。</returns>
        Task<Model?> GetModelByIdAsync(int id);

        /// <summary>
        /// 新しい機種情報を追加します。
        /// </summary>
        /// <param name="model">追加する機種情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddModelAsync(Model model);

        /// <summary>
        /// 機種情報を更新します。
        /// </summary>
        /// <param name="model">更新する機種情報。</param>
        Task UpdateModelAsync(Model model);

        /// <summary>
        /// 機種情報を削除します。
        /// </summary>
        /// <param name="id">削除する機種ID。</param>
        Task DeleteModelAsync(int id);

        #endregion

        #region PLC

        /// <summary>
        /// 全てのPLC情報を取得します。
        /// </summary>
        /// <returns>PLC情報のリスト。</returns>
        Task<List<PLC>> GetPLCsAsync();

        /// <summary>
        /// 指定されたIDのPLC情報を取得します。
        /// </summary>
        /// <param name="id">PLC ID。</param>
        /// <returns>PLC情報。見つからない場合はnull。</returns>
        Task<PLC?> GetPLCByIdAsync(int id);

        /// <summary>
        /// 新しいPLC情報を追加します。
        /// </summary>
        /// <param name="plc">追加するPLC情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddPLCAsync(PLC plc);

        /// <summary>
        /// PLC情報を更新します。
        /// </summary>
        /// <param name="plc">更新するPLC情報。</param>
        Task UpdatePLCAsync(PLC plc);

        /// <summary>
        /// PLC情報を削除します。
        /// </summary>
        /// <param name="id">削除するPLC ID。</param>
        Task DeletePLCAsync(int id);

        #endregion

        #region Cycle

        /// <summary>
        /// 全てのサイクル情報を取得します。
        /// </summary>
        /// <returns>サイクル情報のリスト。</returns>
        Task<List<Cycle>> GetCyclesAsync();

        /// <summary>
        /// 指定されたIDのサイクル情報を取得します。
        /// </summary>
        /// <param name="id">サイクルID。</param>
        /// <returns>サイクル情報。見つからない場合はnull。</returns>
        Task<Cycle?> GetCycleByIdAsync(int id);

        /// <summary>
        /// 新しいサイクル情報を追加します。
        /// </summary>
        /// <param name="cycle">追加するサイクル情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddCycleAsync(Cycle cycle);

        /// <summary>
        /// サイクル情報を更新します。
        /// </summary>
        /// <param name="cycle">更新するサイクル情報。</param>
        Task UpdateCycleAsync(Cycle cycle);

        /// <summary>
        /// サイクル情報を削除します。
        /// </summary>
        /// <param name="id">削除するサイクルID。</param>
        Task DeleteCycleAsync(int id);

        #endregion

        #region CylinderCycle

        /// <summary>
        /// 指定されたPLC IDに紐づくシリンダーサイクル中間テーブル情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>シリンダーサイクル情報のリスト。</returns>
        Task<List<CylinderCycle>> GetCylinderCyclesByPlcIdAsync(int plcId);

        /// <summary>
        /// 指定されたシリンダーIDに紐づくシリンダーサイクル中間テーブル情報を取得します。
        /// </summary>
        /// <param name="cylinderId">シリンダーID。</param>
        /// <returns>シリンダーサイクル情報のリスト。</returns>
        Task<List<CylinderCycle>> GetCylinderCyclesByCylinderIdAsync(int cylinderId);

        /// <summary>
        /// シリンダーサイクルの関連を追加します。
        /// </summary>
        /// <param name="cylinderCycle">追加するシリンダーサイクル情報。</param>
        Task AddCylinderCycleAsync(CylinderCycle cylinderCycle);

        /// <summary>
        /// シリンダーサイクルの関連を削除します。
        /// </summary>
        /// <param name="cylinderId">シリンダーID。</param>
        /// <param name="cycleId">サイクルID。</param>
        Task DeleteCylinderCycleAsync(int cylinderId, int cycleId);

        #endregion

        #region ControlBox

        /// <summary>
        /// 指定されたPLC IDに紐づくコントロールボックス情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>コントロールボックス情報のリスト。</returns>
        Task<List<ControlBox>> GetControlBoxesByPlcIdAsync(int plcId);

        #endregion

        #region CylinderControlBox

        /// <summary>
        /// 指定されたPLC IDに紐づくシリンダーコントロールボックス情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>シリンダーコントロールボックス情報のリスト。</returns>
        Task<List<CylinderControlBox>> GetCylinderControlBoxesByPlcIdAsync(int plcId);

        /// <summary>
        /// 指定されたPLC IDとシリンダーIDに紐づくシリンダーコントロールボックス情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="cylinderId">シリンダーID。</param>
        /// <returns>シリンダーコントロールボックス情報のリスト。</returns>
        Task<List<CylinderControlBox>> GetCylinderControlBoxesAsync(int plcId, int cylinderId);

        /// <summary>
        /// シリンダーコントロールボックス情報を挿入または更新します。
        /// </summary>
        /// <param name="item">挿入または更新するシリンダーコントロールボックス情報。</param>
        Task UpsertCylinderControlBoxAsync(CylinderControlBox item);

        /// <summary>
        /// シリンダーコントロールボックス情報を削除します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="cylinderId">シリンダーID。</param>
        /// <param name="boxId">ボックスID。</param>
        Task DeleteCylinderControlBoxAsync(int plcId, int cylinderId, int boxId);

        #endregion

        #region Process

        /// <summary>
        /// 全ての工程情報を取得します。
        /// </summary>
        /// <returns>工程情報のリスト。</returns>
        Task<List<Process>> GetProcessesAsync();

        /// <summary>
        /// 指定されたIDの工程情報を取得します。
        /// </summary>
        /// <param name="id">工程ID。</param>
        /// <returns>工程情報。見つからない場合はnull。</returns>
        Task<Process?> GetProcessByIdAsync(int id);

        /// <summary>
        /// 新しい工程情報を追加します。
        /// </summary>
        /// <param name="process">追加する工程情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddProcessAsync(Process process);

        /// <summary>
        /// 工程情報を更新します。
        /// </summary>
        /// <param name="process">更新する工程情報。</param>
        Task UpdateProcessAsync(Process process);

        /// <summary>
        /// 指定されたIDの工程情報を削除します。
        /// </summary>
        /// <param name="id">削除する工程ID。</param>
        Task DeleteProcessAsync(int id);

        #endregion

        #region Machine

        /// <summary>
        /// 全ての機械情報を取得します。
        /// </summary>
        /// <returns>機械情報のリスト。</returns>
        Task<List<Machine>> GetMachinesAsync();

        /// <summary>
        /// 指定されたIDの機械情報を取得します。
        /// </summary>
        /// <param name="nameId">機械名称ID。</param>
        /// <param name="driveSubId">駆動部(副)ID。</param>
        /// <returns>機械情報。見つからない場合はnull。</returns>
        Task<Machine?> GetMachineByIdAsync(int nameId, int driveSubId);

        /// <summary>
        /// 新しい機械情報を追加します。
        /// </summary>
        /// <param name="machine">追加する機械情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddMachineAsync(Machine machine);

        /// <summary>
        /// 機械情報を更新します。
        /// </summary>
        /// <param name="machine">更新する機械情報。</param>
        Task UpdateMachineAsync(Machine machine);

        /// <summary>
        /// 機械情報を削除します。
        /// </summary>
        /// <param name="nameId">機械名称ID。</param>
        /// <param name="driveSubId">駆動部(副)ID。</param>
        Task DeleteMachineAsync(int nameId, int driveSubId);

        #endregion

        #region MachineName

        /// <summary>
        /// 全ての機械名称情報を取得します。
        /// </summary>
        /// <returns>機械名称情報のリスト。</returns>
        Task<List<MachineName>> GetMachineNamesAsync();

        /// <summary>
        /// 指定されたIDの機械名称情報を取得します。
        /// </summary>
        /// <param name="id">機械名称ID。</param>
        /// <returns>機械名称情報。見つからない場合はnull。</returns>
        Task<MachineName?> GetMachineNameByIdAsync(int id);

        /// <summary>
        /// 新しい機械名称情報を追加します。
        /// </summary>
        /// <param name="machineName">追加する機械名称情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddMachineNameAsync(MachineName machineName);

        /// <summary>
        /// 機械名称情報を更新します。
        /// </summary>
        /// <param name="machineName">更新する機械名称情報。</param>
        Task UpdateMachineNameAsync(MachineName machineName);

        /// <summary>
        /// 機械名称情報を削除します。
        /// </summary>
        /// <param name="id">削除する機械名称ID。</param>
        Task DeleteMachineNameAsync(int id);

        #endregion

        #region DriveMain

        /// <summary>
        /// 全ての駆動部(主)情報を取得します。
        /// </summary>
        /// <returns>駆動部(主)情報のリスト。</returns>
        Task<List<DriveMain>> GetDriveMainsAsync();

        /// <summary>
        /// 指定されたIDの駆動部(主)情報を取得します。
        /// </summary>
        /// <param name="id">駆動部(主)ID。</param>
        /// <returns>駆動部(主)情報。見つからない場合はnull。</returns>
        Task<DriveMain?> GetDriveMainByIdAsync(int id);

        /// <summary>
        /// 新しい駆動部(主)情報を追加します。
        /// </summary>
        /// <param name="driveMain">追加する駆動部(主)情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddDriveMainAsync(DriveMain driveMain);

        /// <summary>
        /// 駆動部(主)情報を更新します。
        /// </summary>
        /// <param name="driveMain">更新する駆動部(主)情報。</param>
        Task UpdateDriveMainAsync(DriveMain driveMain);

        /// <summary>
        /// 駆動部(主)情報を削除します。
        /// </summary>
        /// <param name="id">削除する駆動部(主)ID。</param>
        Task DeleteDriveMainAsync(int id);

        #endregion

        #region DriveSub

        /// <summary>
        /// 全ての駆動部(副)情報を取得します。
        /// </summary>
        /// <returns>駆動部(副)情報のリスト。</returns>
        Task<List<DriveSub>> GetDriveSubsAsync();

        /// <summary>
        /// 指定されたIDの駆動部(副)情報を取得します。
        /// </summary>
        /// <param name="id">駆動部(副)ID。</param>
        /// <returns>駆動部(副)情報。見つからない場合はnull。</returns>
        Task<DriveSub?> GetDriveSubByIdAsync(int id);

        /// <summary>
        /// 新しい駆動部(副)情報を追加します。
        /// </summary>
        /// <param name="driveSub">追加する駆動部(副)情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddDriveSubAsync(DriveSub driveSub);

        /// <summary>
        /// 駆動部(副)情報を更新します。
        /// </summary>
        /// <param name="driveSub">更新する駆動部(副)情報。</param>
        Task UpdateDriveSubAsync(DriveSub driveSub);

        /// <summary>
        /// 駆動部(副)情報を削除します。
        /// </summary>
        /// <param name="id">削除する駆動部(副)ID。</param>
        Task DeleteDriveSubAsync(int id);

        #endregion

        #region Cylinder

        /// <summary>
        /// 全てのシリンダー情報を取得します。
        /// </summary>
        /// <returns>シリンダー情報のリスト。</returns>
        Task<List<Cylinder>> GetCYsAsync();

        /// <summary>
        /// 指定されたPLC IDに紐づくシリンダー情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>シリンダー情報のリスト。</returns>
        Task<List<Cylinder>> GetCyListAsync(int plcId);

        /// <summary>
        /// 指定されたIDのシリンダー情報を取得します。
        /// </summary>
        /// <param name="id">シリンダーID。</param>
        /// <returns>シリンダー情報。見つからない場合はnull。</returns>
        Task<Cylinder?> GetCYByIdAsync(int id);

        /// <summary>
        /// 新しいシリンダー情報を追加します。
        /// </summary>
        /// <param name="cylinder">追加するシリンダー情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddCylinderAsync(Cylinder cylinder);

        /// <summary>
        /// シリンダー情報を更新します。
        /// </summary>
        /// <param name="cylinder">更新するシリンダー情報。</param>
        Task UpdateCylinderAsync(Cylinder cylinder);

        /// <summary>
        /// シリンダー情報を削除します。
        /// </summary>
        /// <param name="id">削除するシリンダーID。</param>
        Task DeleteCylinderAsync(int id);

        #endregion

        #region Timer

        /// <summary>
        /// 全てのタイマー情報を取得します。
        /// </summary>
        /// <returns>タイマー情報のリスト。</returns>
        Task<List<Timer>> GetTimersAsync();

        /// <summary>
        /// 指定されたサイクルIDに紐づくタイマー情報を取得します。
        /// </summary>
        /// <param name="cycleId">サイクルID。</param>
        /// <returns>タイマー情報のリスト。</returns>
        Task<List<Timer>> GetTimersByCycleIdAsync(int cycleId);

        /// <summary>
        /// 指定された条件に紐づくタイマー情報を取得します。
        /// </summary>
        /// <param name="cycleId">サイクルID。</param>
        /// <param name="mnemonicId">ニーモニックID。</param>
        /// <param name="recordId">レコードID。</param>
        /// <returns>タイマー情報のリスト。</returns>
        Task<List<MnemonicTimerDevice>> GetTimersByRecordIdAsync(int cycleId, int mnemonicId, int recordId);

        /// <summary>
        /// 指定されたIDのタイマー情報を取得します。
        /// </summary>
        /// <param name="id">タイマーID。</param>
        /// <returns>タイマー情報。見つからない場合はnull。</returns>
        Task<Timer?> GetTimerByIdAsync(int id);

        /// <summary>
        /// 新しいタイマー情報を追加します。
        /// </summary>
        /// <param name="timer">追加するタイマー情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddTimerAsync(Timer timer);

        /// <summary>
        /// タイマー情報を更新します。
        /// </summary>
        /// <param name="timer">更新するタイマー情報。</param>
        Task UpdateTimerAsync(Timer timer);

        /// <summary>
        /// タイマー情報を削除します。
        /// </summary>
        /// <param name="id">削除するタイマーID。</param>
        Task DeleteTimerAsync(int id);

        #endregion

        #region TimerRecord

        /// <summary>
        /// 指定されたタイマーIDに関連するレコードIDを取得します。
        /// </summary>
        /// <param name="timerId">タイマーID。</param>
        /// <returns>レコードIDのリスト。</returns>
        Task<List<int>> GetTimerRecordIdsAsync(int timerId);

        /// <summary>
        /// タイマーとレコードの関連を追加します。
        /// </summary>
        /// <param name="timerId">タイマーID。</param>
        /// <param name="recordId">レコードID。</param>
        Task AddTimerRecordIdAsync(int timerId, int recordId);

        /// <summary>
        /// タイマーとレコードの関連を削除します。
        /// </summary>
        /// <param name="timerId">タイマーID。</param>
        /// <param name="recordId">レコードID。</param>
        Task DeleteTimerRecordIdAsync(int timerId, int recordId);

        /// <summary>
        /// 指定されたタイマーIDの全レコード関連を削除します。
        /// </summary>
        /// <param name="timerId">タイマーID。</param>
        Task DeleteAllTimerRecordIdsAsync(int timerId);

        #endregion

        #region Operation

        /// <summary>
        /// 全ての操作情報を取得します。
        /// </summary>
        /// <returns>操作情報のリスト。</returns>
        Task<List<Operation>> GetOperationsAsync();

        /// <summary>
        /// 指定されたサイクルIDの操作情報を取得します。
        /// </summary>
        /// <param name="cycleId">サイクルID。</param>
        /// <returns>操作情報のリスト。</returns>
        Task<List<Operation>> GetOperationsByCycleIdAsync(int cycleId);

        /// <summary>
        /// 指定されたIDの操作情報を取得します。
        /// </summary>
        /// <param name="id">操作ID。</param>
        /// <returns>操作情報。見つからない場合はnull。</returns>
        Task<Operation?> GetOperationByIdAsync(int id);

        /// <summary>
        /// 新しい操作情報を追加します。
        /// </summary>
        /// <param name="operation">追加する操作情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddOperationAsync(Operation operation);

        /// <summary>
        /// 操作情報を更新します。
        /// </summary>
        /// <param name="operation">更新する操作情報。</param>
        Task UpdateOperationAsync(Operation operation);

        /// <summary>
        /// 操作情報を削除します。
        /// </summary>
        /// <param name="id">削除する操作ID。</param>
        Task DeleteOperationAsync(int id);

        #endregion

        #region Length

        /// <summary>
        /// 指定されたPLC IDに紐づくLength情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>Length情報のリスト。見つからない場合はnull。</returns>
        Task<List<Length>?> GetLengthByPlcIdAsync(int plcId);

        #endregion

        #region ProcessDetail

        /// <summary>
        /// 全ての工程詳細情報を取得します。
        /// </summary>
        /// <returns>工程詳細情報のリスト。</returns>
        Task<List<ProcessDetail>> GetProcessDetailsAsync();

        /// <summary>
        /// 指定されたIDの工程詳細情報を取得します。
        /// </summary>
        /// <param name="id">工程詳細ID。</param>
        /// <returns>工程詳細情報。見つからない場合はnull。</returns>
        Task<ProcessDetail?> GetProcessDetailByIdAsync(int id);

        /// <summary>
        /// 新しい工程詳細情報を追加します。
        /// </summary>
        /// <param name="processDetail">追加する工程詳細情報。</param>
        /// <returns>追加されたレコードのID。</returns>
        Task<int> AddProcessDetailAsync(ProcessDetail processDetail);

        /// <summary>
        /// 工程詳細情報を更新します。
        /// </summary>
        /// <param name="processDetail">更新する工程詳細情報。</param>
        Task UpdateProcessDetailAsync(ProcessDetail processDetail);

        /// <summary>
        /// 工程詳細情報を削除します。
        /// </summary>
        /// <param name="id">削除する工程詳細ID。</param>
        Task DeleteProcessDetailAsync(int id);

        #endregion

        #region ProcessCategory

        /// <summary>
        /// 全ての工程カテゴリを取得します。
        /// </summary>
        /// <returns>工程カテゴリのリスト。</returns>
        Task<List<ProcessCategory>> GetProcessCategoriesAsync();

        /// <summary>
        /// 指定されたIDの工程カテゴリを取得します。
        /// </summary>
        /// <param name="id">工程カテゴリID。</param>
        /// <returns>工程カテゴリ情報。見つからない場合はnull。</returns>
        Task<ProcessCategory?> GetProcessCategoryByIdAsync(int id);

        #endregion

        #region ProcessDetailCategory

        /// <summary>
        /// 全ての工程詳細カテゴリを取得します。
        /// </summary>
        /// <returns>工程詳細カテゴリのリスト。</returns>
        Task<List<ProcessDetailCategory>> GetProcessDetailCategoriesAsync();

        /// <summary>
        /// 指定されたIDの工程詳細カテゴリを取得します。
        /// </summary>
        /// <param name="id">工程詳細カテゴリID。</param>
        /// <returns>工程詳細カテゴリ情報。見つからない場合はnull。</returns>
        Task<ProcessDetailCategory?> GetProcessDetailCategoryByIdAsync(int id);

        #endregion

        #region OperationCategory

        /// <summary>
        /// 全ての操作カテゴリを取得します。
        /// </summary>
        /// <returns>操作カテゴリのリスト。</returns>
        Task<List<OperationCategory>> GetOperationCategoriesAsync();

        /// <summary>
        /// 指定されたIDの操作カテゴリを取得します。
        /// </summary>
        /// <param name="id">操作カテゴリID。</param>
        /// <returns>操作カテゴリ情報。見つからない場合はnull。</returns>
        Task<OperationCategory?> GetOperationCategoryByIdAsync(int id);

        #endregion

        #region MnemonicTimerDevice

        /// <summary>
        /// 全てのMnemonicTimerDevice情報を取得します。
        /// </summary>
        /// <returns>MnemonicTimerDevice情報のリスト。</returns>
        Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesAsync();

        /// <summary>
        /// 指定されたPLC IDとサイクルIDに紐づくMnemonicTimerDevice情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="cycleId">サイクルID。</param>
        /// <returns>MnemonicTimerDevice情報のリスト。</returns>
        Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByClcleIdAsync(int plcId, int cycleId);

        /// <summary>
        /// 指定されたPLC IDとサイクルIDに紐づくMnemonicTimerDevice情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="cycleId">サイクルID。</param>
        /// <returns>MnemonicTimerDevice情報のリスト。</returns>
        Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByPlcIdAsync(int plcId);

        /// <summary>
        /// 指定されたPLC ID、サイクルID、ニーモニックIDに紐づくMnemonicTimerDevice情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="cycleId">サイクルID。</param>
        /// <param name="mnemonicId">ニーモニックID。</param>
        /// <returns>MnemonicTimerDevice情報のリスト。</returns>
        Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByCycleAndMnemonicIdAsync(int plcId, int cycleId, int mnemonicId);

        /// <summary>
        /// 指定されたPLC IDとニーモニックIDに紐づくMnemonicTimerDevice情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="mnemonicId">ニーモニックID。</param>
        /// <returns>MnemonicTimerDevice情報のリスト。</returns>
        Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByMnemonicIdAsync(int plcId, int mnemonicId);

        /// <summary>
        /// 指定されたPLC IDとタイマーIDに紐づくMnemonicTimerDevice情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="timerId">タイマーID。</param>
        /// <returns>MnemonicTimerDevice情報のリスト。</returns>
        Task<List<MnemonicTimerDevice>> GetMnemonicTimerDevicesByTimerIdAsync(int plcId, int timerId);

        /// <summary>
        /// 新しいMnemonicTimerDevice情報を追加します。
        /// </summary>
        /// <param name="device">追加するMnemonicTimerDevice情報。</param>
        Task AddMnemonicTimerDeviceAsync(MnemonicTimerDevice device);

        /// <summary>
        /// MnemonicTimerDevice情報を更新します。
        /// </summary>
        /// <param name="device">更新するMnemonicTimerDevice情報。</param>
        Task UpdateMnemonicTimerDeviceAsync(MnemonicTimerDevice device);

        /// <summary>
        /// MnemonicTimerDevice情報を挿入または更新します。
        /// </summary>
        /// <param name="device">挿入または更新するMnemonicTimerDevice情報。</param>
        Task UpsertMnemonicTimerDeviceAsync(MnemonicTimerDevice device);

        /// <summary>
        /// 指定されたMnemonicTimerDevice情報を削除します。
        /// </summary>
        /// <param name="mnemonicId">ニーモニックID。</param>
        /// <param name="recordId">レコードID。</param>
        /// <param name="timerId">タイマーID。</param>
        Task DeleteMnemonicTimerDeviceAsync(int mnemonicId, int recordId, int timerId);

        /// <summary>
        /// 全てのMnemonicTimerDevice情報を削除します。
        /// </summary>
        Task DeleteAllMnemonicTimerDeviceAsync();

        #endregion

        #region IO

        /// <summary>
        /// 全てのIO情報を取得します。
        /// </summary>
        /// <returns>IO情報のリスト。</returns>
        Task<List<IO>> GetIoListAsync();

        /// <summary>
        /// 指定されたIDのIO情報を取得します。
        /// </summary>
        /// <param name="id">IO ID。</param>
        /// <returns>IO情報。見つからない場合はnull。</returns>
        Task<IO?> GetIoByIdAsync(int id);

        /// <summary>
        /// IOレコードのリストを受け取り、LinkDeviceカラムをバッチ更新します。
        /// </summary>
        /// <param name="ioRecordsToUpdate">LinkDeviceが設定されたIOオブジェクトのリスト。</param>
        Task UpdateIoLinkDevicesAsync(IEnumerable<IO> ioRecordsToUpdate);

        /// <summary>
        /// IOレコードのリストを更新し、同時に変更履歴を保存します。
        /// これらの一連の処理は単一のトランザクション内で実行されます。
        /// </summary>
        /// <param name="iosToUpdate">更新対象のIOオブジェクトのリスト。</param>
        /// <param name="histories">保存する変更履歴のリスト。</param>
        Task UpdateAndLogIoChangesAsync(List<IO> iosToUpdate, List<IOHistory> histories);

        #endregion

        #region TimerCategory

        /// <summary>
        /// 全てのタイマーカテゴリ情報を取得します。
        /// </summary>
        /// <returns>タイマーカテゴリ情報のリスト。</returns>
        Task<List<TimerCategory>> GetTimerCategoryAsync();

        #endregion

        #region Servo

        /// <summary>
        /// サーボ情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID(オプション)。</param>
        /// <param name="cylinderId">シリンダーID(オプション)。</param>
        /// <returns>サーボ情報のリスト。</returns>
        Task<List<Servo>> GetServosAsync(int? plcId, int? cylinderId);

        #endregion

        #region ProcessDetailConnection

        /// <summary>
        /// 全ての工程詳細接続情報を取得します。
        /// </summary>
        /// <returns>工程詳細接続情報のリスト。</returns>
        Task<List<ProcessDetailConnection>> GetAllProcessDetailConnectionsAsync();

        /// <summary>
        /// 指定されたサイクルIDに紐づく工程詳細接続情報を取得します。
        /// </summary>
        /// <param name="cycleId">サイクルID。</param>
        /// <returns>工程詳細接続情報のリスト。</returns>
        Task<List<ProcessDetailConnection>> GetProcessDetailConnectionsAsync(int cycleId);

        /// <summary>
        /// 指定された始点工程詳細IDに紐づく接続情報を取得します。
        /// </summary>
        /// <param name="fromProcessDetailId">始点の工程詳細ID。</param>
        /// <returns>工程詳細接続情報のリスト。</returns>
        Task<List<ProcessDetailConnection>> GetConnectionsByFromIdAsync(int fromProcessDetailId);

        /// <summary>
        /// 指定された終点工程詳細IDに紐づく接続情報を取得します。
        /// </summary>
        /// <param name="toProcessDetailId">終点の工程詳細ID。</param>
        /// <returns>工程詳細接続情報のリスト。</returns>
        Task<List<ProcessDetailConnection>> GetConnectionsByToIdAsync(int toProcessDetailId);

        /// <summary>
        /// 新しい工程詳細接続情報を追加します。
        /// </summary>
        /// <param name="connection">追加する工程詳細接続情報。</param>
        Task AddProcessDetailConnectionAsync(ProcessDetailConnection connection);

        /// <summary>
        /// 工程詳細接続情報を削除します。
        /// </summary>
        /// <param name="fromProcessDetailId">始点の工程詳細ID。</param>
        /// <param name="toProcessDetailId">終点の工程詳細ID。</param>
        Task DeleteProcessDetailConnectionAsync(int fromProcessDetailId, int toProcessDetailId);

        /// <summary>
        /// 指定されたFromIdとToIdの組み合わせの接続情報を削除します。
        /// </summary>
        /// <param name="fromId">始点の工程詳細ID。</param>
        /// <param name="toId">終点の工程詳細ID。</param>
        Task DeleteConnectionsByFromAndToAsync(int fromId, int toId);

        /// <summary>
        /// 指定されたFromProcessDetailIdの全ての接続を削除します。
        /// </summary>
        /// <param name="fromProcessDetailId">始点の工程詳細ID。</param>
        Task DeleteConnectionsByFromIdAsync(int fromProcessDetailId);

        /// <summary>
        /// 指定されたToProcessDetailIdの全ての接続を削除します。
        /// </summary>
        /// <param name="toProcessDetailId">終点の工程詳細ID。</param>
        Task DeleteConnectionsByToIdAsync(int toProcessDetailId);

        #endregion

        #region ProcessDetailFinish

        /// <summary>
        /// 全ての工程詳細終了情報を取得します。
        /// </summary>
        /// <returns>工程詳細終了情報のリスト。</returns>
        Task<List<ProcessDetailFinish>> GetAllProcessDetailFinishesAsync();

        /// <summary>
        /// 指定されたサイクルIDに紐づく工程詳細終了情報を取得します。
        /// </summary>
        /// <param name="cycleId">サイクルID。</param>
        /// <returns>工程詳細終了情報のリスト。</returns>
        Task<List<ProcessDetailFinish>> GetProcessDetailFinishesAsync(int cycleId);

        /// <summary>
        /// 指定された工程詳細IDに紐づく終了情報を取得します。
        /// </summary>
        /// <param name="processDetailId">工程詳細ID。</param>
        /// <returns>工程詳細終了情報のリスト。</returns>
        Task<List<ProcessDetailFinish>> GetFinishesByProcessDetailIdAsync(int processDetailId);

        /// <summary>
        /// 指定された終了先工程詳細IDに紐づく終了情報を取得します。
        /// </summary>
        /// <param name="finishProcessDetailId">終了先の工程詳細ID。</param>
        /// <returns>工程詳細終了情報のリスト。</returns>
        Task<List<ProcessDetailFinish>> GetFinishesByFinishIdAsync(int finishProcessDetailId);

        /// <summary>
        /// 新しい工程詳細終了情報を追加します。
        /// </summary>
        /// <param name="finish">追加する工程詳細終了情報。</param>
        Task AddProcessDetailFinishAsync(ProcessDetailFinish finish);

        /// <summary>
        /// 工程詳細終了情報を削除します。
        /// </summary>
        /// <param name="processDetailId">工程詳細ID。</param>
        /// <param name="finishProcessDetailId">終了先の工程詳細ID。</param>
        Task DeleteProcessDetailFinishAsync(int processDetailId, int finishProcessDetailId);

        /// <summary>
        /// 指定された工程詳細IDと終了先工程詳細IDの組み合わせの終了情報を削除します。
        /// </summary>
        /// <param name="processDetailId">工程詳細ID。</param>
        /// <param name="finishProcessDetailId">終了先の工程詳細ID。</param>
        Task DeleteFinishesByProcessAndFinishAsync(int processDetailId, int finishProcessDetailId);

        /// <summary>
        /// 指定されたProcessDetailIdの全ての終了条件を削除します。
        /// </summary>
        /// <param name="processDetailId">工程詳細ID。</param>
        Task DeleteFinishesByProcessDetailIdAsync(int processDetailId);

        #endregion

        #region ProcessStartCondition

        /// <summary>
        /// 指定されたサイクルIDの全ての工程開始条件を取得します。
        /// </summary>
        /// <param name="cycleId">サイクルID。</param>
        /// <returns>工程開始条件のリスト。</returns>
        Task<List<ProcessStartCondition>> GetProcessStartConditionsAsync(int cycleId);

        /// <summary>
        /// 指定された工程IDの開始条件を取得します。
        /// </summary>
        /// <param name="processId">工程ID。</param>
        /// <returns>工程開始条件のリスト。</returns>
        Task<List<ProcessStartCondition>> GetStartConditionsByProcessIdAsync(int processId);

        /// <summary>
        /// 新しい工程開始条件を追加します。
        /// </summary>
        /// <param name="condition">追加する開始条件。</param>
        Task AddProcessStartConditionAsync(ProcessStartCondition condition);

        /// <summary>
        /// 工程開始条件を更新します。
        /// </summary>
        /// <param name="condition">更新する開始条件。</param>
        Task UpdateProcessStartConditionAsync(ProcessStartCondition condition);

        /// <summary>
        /// 工程開始条件を削除します。
        /// </summary>
        /// <param name="processId">工程ID。</param>
        /// <param name="startProcessDetailId">開始条件となる工程詳細ID。</param>
        Task DeleteProcessStartConditionAsync(int processId, int startProcessDetailId);

        /// <summary>
        /// 指定された工程IDの全ての開始条件を削除します。
        /// </summary>
        /// <param name="processId">工程ID。</param>
        Task DeleteStartConditionsByProcessAsync(int processId);

        #endregion

        #region ProcessFinishCondition

        /// <summary>
        /// 指定されたサイクルIDの全ての工程終了条件を取得します。
        /// </summary>
        /// <param name="cycleId">サイクルID。</param>
        /// <returns>工程終了条件のリスト。</returns>
        Task<List<ProcessFinishCondition>> GetProcessFinishConditionsAsync(int cycleId);

        /// <summary>
        /// 指定された工程IDの終了条件を取得します。
        /// </summary>
        /// <param name="processId">工程ID。</param>
        /// <returns>工程終了条件のリスト。</returns>
        Task<List<ProcessFinishCondition>> GetFinishConditionsByProcessIdAsync(int processId);

        /// <summary>
        /// 新しい工程終了条件を追加します。
        /// </summary>
        /// <param name="condition">追加する終了条件。</param>
        Task AddProcessFinishConditionAsync(ProcessFinishCondition condition);

        /// <summary>
        /// 工程終了条件を更新します。
        /// </summary>
        /// <param name="condition">更新する終了条件。</param>
        Task UpdateProcessFinishConditionAsync(ProcessFinishCondition condition);

        /// <summary>
        /// 工程終了条件を削除します。
        /// </summary>
        /// <param name="processId">工程ID。</param>
        /// <param name="finishProcessDetailId">終了条件となる工程詳細ID。</param>
        Task DeleteProcessFinishConditionAsync(int processId, int finishProcessDetailId);

        /// <summary>
        /// 指定された工程IDの全ての終了条件を削除します。
        /// </summary>
        /// <param name="processId">工程ID。</param>
        Task DeleteFinishConditionsByProcessAsync(int processId);

        #endregion

        #region CylinderIO

        /// <summary>
        /// 指定されたシリンダーIDとPLC IDに紐づくシリンダーIO情報を取得します。
        /// </summary>
        /// <param name="cylinderId">シリンダーID。</param>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>シリンダーIO情報のリスト。</returns>
        Task<List<CylinderIO>> GetCylinderIOsAsync(int cylinderId, int plcId);

        /// <summary>
        /// 指定されたIOアドレスとPLC IDに紐づくシリンダー情報を取得します。
        /// </summary>
        /// <param name="ioAddress">IOアドレス。</param>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>シリンダーIO情報のリスト。</returns>
        Task<List<CylinderIO>> GetIOCylindersAsync(string ioAddress, int plcId);

        /// <summary>
        /// 指定されたPLC IDに紐づく全てのシリンダーIO関連情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>シリンダーIO情報のリスト。</returns>
        Task<List<CylinderIO>> GetAllCylinderIOAssociationsAsync(int plcId);

        /// <summary>
        /// シリンダーとIOの関連を追加します。
        /// </summary>
        /// <param name="cylinderId">シリンダーID。</param>
        /// <param name="ioAddress">IOアドレス。</param>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="ioType">IOタイプ。</param>
        /// <param name="comment">コメント(オプション)。</param>
        Task AddCylinderIOAssociationAsync(int cylinderId, string ioAddress, int plcId, string ioType, string? comment = null);

        /// <summary>
        /// シリンダーとIOの関連を削除します。
        /// </summary>
        /// <param name="cylinderId">シリンダーID。</param>
        /// <param name="ioAddress">IOアドレス。</param>
        /// <param name="plcId">PLC ID。</param>
        Task RemoveCylinderIOAssociationAsync(int cylinderId, string ioAddress, int plcId);

        #endregion

        #region OperationIO

        /// <summary>
        /// 指定された操作IDに紐づくIO情報を取得します。
        /// </summary>
        /// <param name="operationId">操作ID。</param>
        /// <returns>操作IO情報のリスト。</returns>
        Task<List<OperationIO>> GetOperationIOsAsync(int operationId);

        /// <summary>
        /// 指定されたIOアドレスとPLC IDに紐づく操作情報を取得します。
        /// </summary>
        /// <param name="ioAddress">IOアドレス。</param>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>操作IO情報のリスト。</returns>
        Task<List<OperationIO>> GetIOOperationsAsync(string ioAddress, int plcId);

        /// <summary>
        /// 指定されたPLC IDに紐づく全ての操作IO関連情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>操作IO情報のリスト。</returns>
        Task<List<OperationIO>> GetAllOperationIOAssociationsAsync(int plcId);

        /// <summary>
        /// 操作とIOの関連を追加します。
        /// </summary>
        /// <param name="operationId">操作ID。</param>
        /// <param name="ioAddress">IOアドレス。</param>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="ioUsage">IO使用法。</param>
        /// <param name="comment">コメント(オプション)。</param>
        Task AddOperationIOAssociationAsync(int operationId, string ioAddress, int plcId, string ioUsage, string? comment = null);

        /// <summary>
        /// 操作とIOの関連を削除します。
        /// </summary>
        /// <param name="operationId">操作ID。</param>
        /// <param name="ioAddress">IOアドレス。</param>
        /// <param name="plcId">PLC ID。</param>
        Task RemoveOperationIOAssociationAsync(int operationId, string ioAddress, int plcId);

        #endregion

        #region Error

        /// <summary>
        /// 指定されたニーモニックIDに紐づくエラーメッセージを取得します。
        /// </summary>
        /// <param name="mnemonicId">ニーモニックID。</param>
        /// <returns>エラーメッセージのリスト。</returns>
        Task<List<ErrorMessage>> GetErrorMessagesAsync(int mnemonicId);

        /// <summary>
        /// 指定された条件に紐づくエラー情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="cycleId">サイクルID。</param>
        /// <param name="mnemonicId">ニーモニックID。</param>
        /// <returns>エラー情報のリスト。</returns>
        Task<List<ProcessError>> GetErrorsAsync(int plcId, int cycleId, int mnemonicId);

        /// <summary>
        /// エラー情報を保存します。
        /// </summary>
        /// <param name="errors">保存するエラー情報のリスト。</param>
        Task SaveErrorsAsync(List<ProcessError> errors);

        /// <summary>
        /// エラー情報を更新します。
        /// </summary>
        /// <param name="errors">更新するエラー情報のリスト。</param>
        Task UpdateErrorsAsync(List<ProcessError> errors);

        /// <summary>
        /// エラーテーブルを削除します。
        /// </summary>
        Task DeleteErrorTableAsync();

        #endregion

        #region MnemonicDevice

        /// <summary>
        /// 指定されたPLC IDに紐づくニーモニックデバイス情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>ニーモニックデバイス情報のリスト。</returns>
        Task<List<MnemonicDevice>> GetMnemonicDevicesAsync(int plcId);

        /// <summary>
        /// 指定されたPLC IDとニーモニックIDに紐づくニーモニックデバイス情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="mnemonicId">ニーモニックID。</param>
        /// <returns>ニーモニックデバイス情報のリスト。</returns>
        Task<List<MnemonicDevice>> GetMnemonicDevicesByMnemonicAsync(int plcId, int mnemonicId);

        /// <summary>
        /// ニーモニックデバイス情報を保存または更新します。
        /// </summary>
        /// <param name="device">保存または更新するニーモニックデバイス情報。</param>
        Task SaveOrUpdateMnemonicDeviceAsync(MnemonicDevice device);

        /// <summary>
        /// 指定されたニーモニックデバイス情報を削除します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="mnemonicId">ニーモニックID。</param>
        Task DeleteMnemonicDeviceAsync(int plcId, int mnemonicId);

        /// <summary>
        /// 全てのニーモニックデバイス情報を削除します。
        /// </summary>
        Task DeleteAllMnemonicDevicesAsync();

        #endregion

        #region MnemonicSpeedDevice

        /// <summary>
        /// 指定されたPLC IDに紐づくニーモニック速度デバイス情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>ニーモニック速度デバイス情報のリスト。</returns>
        Task<List<MnemonicSpeedDevice>> GetMnemonicSpeedDevicesAsync(int plcId);

        /// <summary>
        /// ニーモニック速度デバイス情報を保存または更新します。
        /// </summary>
        /// <param name="device">保存または更新するニーモニック速度デバイス情報。</param>
        Task SaveOrUpdateMnemonicSpeedDeviceAsync(MnemonicSpeedDevice device);

        /// <summary>
        /// 速度テーブルを削除します。
        /// </summary>
        Task DeleteSpeedTableAsync();

        #endregion

        #region ProsTime

        /// <summary>
        /// 指定されたPLC IDに紐づくProsTime情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>ProsTime情報のリスト。</returns>
        Task<List<ProsTime>> GetProsTimeByPlcIdAsync(int plcId);

        /// <summary>
        /// 指定されたPLC IDとニーモニックIDに紐づくProsTime情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <param name="mnemonicId">ニーモニックID。</param>
        /// <returns>ProsTime情報のリスト。</returns>
        Task<List<ProsTime>> GetProsTimeByMnemonicIdAsync(int plcId, int mnemonicId);

        /// <summary>
        /// ProsTime定義情報を取得します。
        /// </summary>
        /// <returns>ProsTime定義情報のリスト。</returns>
        Task<List<ProsTimeDefinitions>> GetProsTimeDefinitionsAsync();

        /// <summary>
        /// ProsTime情報を保存または更新します。
        /// </summary>
        /// <param name="prosTime">保存または更新するProsTime情報。</param>
        Task SaveOrUpdateProsTimeAsync(ProsTime prosTime);

        /// <summary>
        /// ProsTime情報をバッチで保存または更新します。
        /// </summary>
        /// <param name="prosTimes">保存または更新するProsTime情報のリスト。</param>
        Task SaveOrUpdateProsTimesBatchAsync(List<ProsTime> prosTimes);

        /// <summary>
        /// ProsTimeテーブルを削除します。
        /// </summary>
        Task DeleteProsTimeTableAsync();

        #endregion

        #region Memory

        /// <summary>
        /// 指定されたPLC IDに紐づくメモリ情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>メモリ情報のリスト。</returns>
        Task<List<Memory>> GetMemoriesAsync(int plcId);

        /// <summary>
        /// 全てのメモリカテゴリ情報を取得します。
        /// </summary>
        /// <returns>メモリカテゴリ情報のリスト。</returns>
        Task<List<MemoryCategory>> GetMemoryCategoriesAsync();

        /// <summary>
        /// メモリ情報を保存または更新します。
        /// </summary>
        /// <param name="memory">保存または更新するメモリ情報。</param>
        Task SaveOrUpdateMemoryAsync(Memory memory);

        /// <summary>
        /// メモリ情報をバッチで保存または更新します。
        /// </summary>
        /// <param name="memories">保存または更新するメモリ情報のリスト。</param>
        Task SaveOrUpdateMemoriesBatchAsync(List<Memory> memories);

        /// <summary>
        /// 指定されたPLC IDに紐づく全てのメモリ情報を削除します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        Task DeleteMemoriesByPlcIdAsync(int plcId);

        #endregion

        #region Definitions

        /// <summary>
        /// 指定されたカテゴリの定義情報を取得します。
        /// </summary>
        /// <param name="category">カテゴリ名。</param>
        /// <returns>定義情報のリスト。</returns>
        Task<List<Definitions>> GetDefinitionsAsync(string category);

        /// <summary>
        /// 指定されたカテゴリの単一定義情報を取得します。
        /// </summary>
        /// <param name="category">カテゴリ名。</param>
        /// <returns>定義情報。見つからない場合はnull。</returns>
        Task<Definitions?> GetDefinitionAsync(string category);

        #endregion

        #region Interlock

        /// <summary>
        /// 指定されたPLC IDに紐づくインターロック情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>インターロック情報のリスト。</returns>
        Task<List<Interlock>> GetInterlocksByPlcIdAsync(int plcId);

        /// <summary>
        /// 指定されたシリンダーIDに紐づくインターロック情報を取得します。
        /// </summary>
        /// <param name="cylinderId">シリンダーID。</param>
        /// <returns>インターロック情報のリスト。</returns>
        Task<List<Interlock>> GetInterlocksByCylindrIdAsync(int cylinderId);

        /// <summary>
        /// インターロック情報を挿入または更新します。
        /// </summary>
        /// <param name="interlock">挿入または更新するインターロック情報。</param>
        Task UpsertInterlockAsync(Interlock interlock);

        /// <summary>
        /// インターロック情報をバッチで挿入または更新します。
        /// </summary>
        /// <param name="interlocks">挿入または更新するインターロック情報のリスト。</param>
        Task UpsertInterlocksAsync(List<Interlock> interlocks);

        /// <summary>
        /// インターロック情報を削除します。
        /// </summary>
        /// <param name="interlock">削除するインターロック情報。</param>
        Task DeleteInterlockAsync(Interlock interlock);

        /// <summary>
        /// インターロック情報をバッチで削除します。
        /// </summary>
        /// <param name="interlocks">削除するインターロック情報のリスト。</param>
        Task DeleteInterlocksAsync(List<Interlock> interlocks);

        #endregion

        #region InterlockCondition

        /// <summary>
        /// 指定されたインターロックIDに紐づくインターロック条件を取得します。
        /// </summary>
        /// <param name="interlockId">インターロックID。</param>
        /// <returns>インターロック条件のリスト。</returns>
        Task<List<InterlockConditionDTO>> GetInterlockConditionsByInterlockIdAsync(int interlockId);

        /// <summary>
        /// インターロック条件を挿入または更新します。
        /// </summary>
        /// <param name="interlockCondition">挿入または更新するインターロック条件。</param>
        Task UpsertInterlockConditionAsync(InterlockConditionDTO interlockCondition);

        /// <summary>
        /// インターロック条件をバッチで挿入または更新します。
        /// </summary>
        /// <param name="interlockConditions">挿入または更新するインターロック条件のリスト。</param>
        Task UpsertInterlockConditionsAsync(List<InterlockConditionDTO> interlockConditions);

        /// <summary>
        /// インターロック条件を削除します。
        /// </summary>
        /// <param name="interlockCondition">削除するインターロック条件。</param>
        Task DeleteInterlockConditionAsync(InterlockConditionDTO interlockCondition);

        /// <summary>
        /// インターロック条件をバッチで削除します。
        /// </summary>
        /// <param name="interlockConditions">削除するインターロック条件のリスト。</param>
        Task DeleteInterlockConditionsAsync(List<InterlockConditionDTO> interlockConditions);

        /// <summary>
        /// 指定されたPLC IDに紐づくインターロック条件ビュー情報を取得します。
        /// </summary>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>インターロック条件ビュー情報のリスト。</returns>
        Task<List<ViewInterlockConditions>> GetViewInterlockConditionsByPlcIdAsync(int plcId);

        #endregion

        #region InterlockIO

        /// <summary>
        /// 指定されたインターロックIDに紐づくインターロックIO情報を取得します。
        /// </summary>
        /// <param name="interlockId">インターロックID。</param>
        /// <returns>インターロックIO情報のリスト。</returns>
        Task<List<InterlockIO>> GetInterlockIOsByInterlockIdAsync(int interlockId);

        /// <summary>
        /// 指定されたIOアドレスとPLC IDに紐づくインターロック情報を取得します。
        /// </summary>
        /// <param name="ioAddress">IOアドレス。</param>
        /// <param name="plcId">PLC ID。</param>
        /// <returns>インターロックIO情報のリスト。</returns>
        Task<List<InterlockIO>> GetIOInterlocksAsync(string ioAddress, int plcId);

        /// <summary>
        /// インターロックとIOの関連を追加します。
        /// </summary>
        /// <param name="interlockIO">追加するインターロックIO情報。</param>
        Task AddInterlockIOAssociationAsync(InterlockIO interlockIO);

        /// <summary>
        /// インターロックとIOの関連を削除します。
        /// </summary>
        /// <param name="interlockIO">削除するインターロックIO情報。</param>
        Task DeleteInterlockIOAsync(InterlockIO interlockIO);

        #endregion

        #region InterlockConditionType

        /// <summary>
        /// 全てのインターロック条件タイプを取得します。
        /// </summary>
        /// <returns>インターロック条件タイプのリスト。</returns>
        Task<List<InterlockConditionType>> GetInterlockConditionTypesAsync();

        #endregion

        #region InterlockPrecondition

        /// <summary>
        /// 全てのインターロック事前条件1を取得します。
        /// </summary>
        /// <returns>インターロック事前条件1のリスト。</returns>
        Task<List<InterlockPrecondition1>> GetInterlockPrecondition1ListAsync();

        /// <summary>
        /// インターロック事前条件1をバッチで挿入または更新します。
        /// </summary>
        /// <param name="preconditions">挿入または更新するインターロック事前条件1のリスト。</param>
        Task UpsertInterlockPrecondition1ListAsync(List<InterlockPrecondition1> preconditions);

        /// <summary>
        /// 全てのインターロック事前条件2を取得します。
        /// </summary>
        /// <returns>インターロック事前条件2のリスト。</returns>
        Task<List<InterlockPrecondition2>> GetInterlockPrecondition2ListAsync();

        /// <summary>
        /// インターロック事前条件2をバッチで挿入または更新します。
        /// </summary>
        /// <param name="preconditions">挿入または更新するインターロック事前条件2のリスト。</param>
        Task UpsertInterlockPrecondition2ListAsync(List<InterlockPrecondition2> preconditions);

        #endregion

        #region MemoryProfile

        /// <summary>
        /// 全てのメモリプロファイル情報を取得します。
        /// </summary>
        /// <returns>メモリプロファイル情報のリスト。</returns>
        Task<List<MemoryProfile>> GetMemoryProfilesAsync();

        /// <summary>
        /// 指定されたサイクルIDのメモリプロファイル情報を取得します。
        /// </summary>
        /// <param name="cycleId">サイクルID（主キー）。</param>
        /// <returns>メモリプロファイル情報。見つからない場合はnull。</returns>
        Task<MemoryProfile?> GetMemoryProfileByCycleIdAsync(int cycleId);

        /// <summary>
        /// 新しいメモリプロファイル情報を追加します。
        /// </summary>
        /// <param name="profile">追加するメモリプロファイル情報。</param>
        Task AddMemoryProfileAsync(MemoryProfile profile);

        /// <summary>
        /// メモリプロファイル情報を更新します。
        /// </summary>
        /// <param name="profile">更新するメモリプロファイル情報。</param>
        Task UpdateMemoryProfileAsync(MemoryProfile profile);

        /// <summary>
        /// メモリプロファイル情報を削除します。
        /// </summary>
        /// <param name="cycleId">削除するメモリプロファイルのサイクルID（主キー）。</param>
        Task DeleteMemoryProfileAsync(int cycleId);

        #endregion
    }
}
