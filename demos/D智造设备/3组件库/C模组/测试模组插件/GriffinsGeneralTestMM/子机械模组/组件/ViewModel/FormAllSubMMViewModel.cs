
using ReactiveUI;
using System.Collections.ObjectModel;

namespace GriffinsGeneralTestMM
{
    public class FormAllSubMMViewModel : ReactiveObject
    {
        // 子控件列表（绑定到WrapPanel）
        private ObservableCollection<UctlTestSubMMViewModel> _uctlTestSubMMList = new();
        private int _execPercent;

        public ObservableCollection<UctlTestSubMMViewModel> UctlTestSubMMList
        {
            get => _uctlTestSubMMList;
            set
            {
                this.RaiseAndSetIfChanged(ref _uctlTestSubMMList, value);
            } 
        }

        public FormAllSubMMViewModel(int execPercent)
        {
            _execPercent = execPercent;
        }

        // 1. 添加无参构造函数（满足XAML解析要求）
        public FormAllSubMMViewModel()
        {
            _execPercent = 0; // 默认值
        }

        /// <summary>
        /// 添加子模组控件
        /// </summary>
        public void AddTestSubMM(UctlTestSubMMViewModel uctlTestSubMM)
        {
            UctlTestSubMMList.Add(uctlTestSubMM);
            uctlTestSubMM.AdjustCurDelyTime(_execPercent);
        }

        /// <summary>
        /// 调整所有子控件的执行延迟
        /// </summary>
        public void AdjustCurExecPercent(int execPercent)
        {
            _execPercent = execPercent;
            foreach (var vm in UctlTestSubMMList)
                vm.AdjustCurDelyTime(execPercent);
        }
    }
}