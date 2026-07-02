using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GriffinsGeneralTestMM
{
    // 所有TestMM的容器ViewModel
    public class FormAllMMViewModel : ReactiveObject
    {
        // 存储多个TestMMViewModel
        private ObservableCollection<UctlTestMMViewModel> _testMMViewModels = new ObservableCollection<UctlTestMMViewModel>();
        private int _execPercent;

        // 公开属性：TestMM视图模型列表（绑定到View的ItemsControl）
        public ObservableCollection<UctlTestMMViewModel> TestMMViewModels
        {
            get => _testMMViewModels;
            set
            {
                this.RaiseAndSetIfChanged(ref _testMMViewModels, value); 
            } 
        }

        // 执行延迟百分比
        public int ExecPercent
        {
            get => _execPercent;
            set
            {
                this.RaiseAndSetIfChanged(ref _execPercent, value); 
            } 
        }

        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }


        #region 构造函数
        public FormAllMMViewModel(int execPercent)
        {
            ExecPercent = execPercent;
        }
        #endregion

        #region 核心业务逻辑
        // 添加TestMM
        public void AddTestMM(UctlTestMMViewModel testMMVM)
        { 
            TestMMViewModels.Add(testMMVM);
            testMMVM.AdjustCurDelyTime(ExecPercent);
        }

        // 调整所有TestMM的执行延迟百分比
        public void AdjustCurExecPercent(int execPercent)
        {
            ExecPercent = execPercent;
            foreach (var vm in TestMMViewModels)
            {
                vm.AdjustCurDelyTime(execPercent); 
            }
        }
        #endregion
    }
}