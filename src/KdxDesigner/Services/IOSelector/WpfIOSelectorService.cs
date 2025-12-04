using Kdx.Contracts.DTOs;
using Kdx.Contracts.Interfaces;
using KdxDesigner.ViewModels;
using KdxDesigner.Views;

// Views名前空間エイリアス
using ViewsCommon = KdxDesigner.Views.Common;

namespace KdxDesigner.Services.IOSelector
{
    /// <summary>
    /// WPFのIO選択ビューを使って IIOSelectorService を実装します。
    /// </summary>
    public class WpfIOSelectorService : IIOSelectorService
    {
        public IO? SelectIoFromMultiple(string ioText, List<IO> candidates, string recordName, int? recordId)
        {
            var viewModel = new IOSelectViewModel(ioText, candidates, recordName, recordId);
            var view = new ViewsCommon.IOSelectView(viewModel)
            {
                Title = $"複数候補の選択: '{ioText}' (対象: {recordName})"
            };

            // ShowDialog()がtrueを返した場合（＝OKが押された場合）
            if (view.ShowDialog() == true)
            {
                return viewModel.SelectedIO; // ViewModelから選択されたIOオブジェクトを取得
            }

            return null; // キャンセルされた場合はnull
        }
    }
}
