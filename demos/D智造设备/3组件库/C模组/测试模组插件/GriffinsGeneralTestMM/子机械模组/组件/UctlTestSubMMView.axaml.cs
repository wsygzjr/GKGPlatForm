using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI2;
using System;

namespace GriffinsGeneralTestMM
{
    public partial class UctlTestSubMMView : UserControl
    { 
        public UctlTestSubMMViewModel ViewModel  ;

        // 事件转发（和原控件一致）
        public event ImeCabilityEventHandler? CabilityEvent
        {
            add => ViewModel.CabilityEvent += value;
            remove => ViewModel.CabilityEvent -= value;
        }

        public event ImeGenNormalEventHandler? GenNormalEvent
        {
            add => ViewModel.GenNormalEvent += value;
            remove => ViewModel.GenNormalEvent -= value;
        }
       
        /// <summary>
        /// 带路径的属性ID的值改变
        /// </summary>
        public event GFBaseTypeObjPropPathValueChangedEventHandler? AfterPropValueChanged
        {
            add => ViewModel.AfterPropValueChanged += value;
            remove => ViewModel.AfterPropValueChanged -= value;
        }

        // 只读属性转发
        public bool ReadOnly
        {
            get => ViewModel.ReadOnly;
            set => ViewModel.ReadOnly = value;
        }

        public UctlTestSubMMView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // 外部接口转发
        public void Init(GenSubMMInfo genSubMMInfo, SubMMAlias alias)
        {
            ViewModel.Init(genSubMMInfo, alias);
        }
        public void SetInstanceName(string instanceName) => ViewModel.SetInstanceName(instanceName);
        public void AdjustCurDelyTime(int execPercent) => ViewModel.AdjustCurDelyTime(execPercent);
        public GriffinsBaseValue GetUIDataObjPropValue(MPPropertyID propertyID) => ViewModel.GetUIDataObjPropValue(propertyID);
        public void SetUIDataObjPropValue(MPPropertyID propertyID, GriffinsBaseValue value) => ViewModel.SetUIDataObjPropValue(propertyID, value);
        public GFBaseTypePropValueList GetAllUIDataObjPropValues() => ViewModel.GetAllUIDataObjPropValues();
        public void SetWorkState(string workState) => ViewModel.SetWorkState(workState);
        public void ShowExecCount() => ViewModel.ShowExecCount();
        public void SetExecMethod(bool execing, string curMethod = "") => ViewModel.SetExecMethod(execing, curMethod);
        public void ShowMessage(string msg) => ViewModel.ShowMessage(msg);
        public void ShowErrorMessage(string msg) => ViewModel.ShowErrorMessage(msg);

		/// <summary>
		/// 设置子机械模组回调接口实例
		/// </summary>
		public void SetSubMMCmdExecutorCallBack(ISubMMCmdExecutorCallBack callBack) => ViewModel.SetSubMMCmdExecutorCallBack(callBack);
	}
}