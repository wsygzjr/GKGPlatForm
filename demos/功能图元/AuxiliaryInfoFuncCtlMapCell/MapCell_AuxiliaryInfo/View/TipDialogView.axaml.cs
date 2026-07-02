using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryInfoFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell.View
{
    public partial class TipDialogView : ReactiveWindow<TipDialogViewModel>
    {
        public TipDialogView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (ViewModel != null)
                {
                    // 将 ViewModel 的关闭请求与当前窗口的 Close 方法绑定
                    // 并把用户选择的枚举结果（Yes/No/Ok等）作为弹窗的最终返回值传出去
                    ViewModel.RequestClose = (result) =>
                    {
                        this.Close(result);
                    };
                }
            });
        }
    }
}