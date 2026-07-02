using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI2;
using System;

namespace GriffinsGeneralTestMM
{
    public partial class UctlTestMMView : UserControl
    {
        // 暴露ViewModel供外部访问
        public UctlTestMMViewModel ViewModel  ;

        #region  原控件的外部事件
        /// <summary>
        /// 机械模组能力事件（转发ViewModel的CabilityEvent）
        /// </summary>
        public event ImeCabilityEventHandler? CabilityEvent
        {
            add => ViewModel.CabilityEvent += value;
            remove => ViewModel.CabilityEvent -= value;
        }

        /// <summary>
        /// 机械模组普通事件（转发ViewModel的GenNormalEvent）
        /// </summary>
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
        #endregion

        #region  原控件的外部属性
        ///// <summary>
        ///// 只读状态（转发ViewModel的ReadOnly属性）
        ///// </summary>
        //public bool ReadOnly
        //{
        //    get => ViewModel.ReadOnly;
        //    set => ViewModel.ReadOnly = value;
        //}
        #endregion

        public UctlTestMMView()
        {
            InitializeComponent(); 
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #region  原控件的外部方法 
        /// <summary>
        /// 设置实例名称
        /// </summary>
        public void SetInstanceName(string instanceName) => ViewModel.SetInstanceName(instanceName);

        /// <summary>
        /// 调整执行延迟时间
        /// </summary>
        public void AdjustCurDelyTime(int execPercent) => ViewModel.AdjustCurDelyTime(execPercent);

        /// <summary>
        /// 获取界面属性值
        /// </summary>
        public GriffinsBaseValue GetUIDataObjPropValue(MPPropertyID propertyID) => ViewModel.GetUIDataObjPropValue(propertyID);

        /// <summary>
        /// 设置界面属性值
        /// </summary>
        public void SetUIDataObjPropValue(MPPropertyID propertyID, GriffinsBaseValue value) => ViewModel.SetUIDataObjPropValue(propertyID, value);

        /// <summary>
        /// 获取所有属性值
        /// </summary>
        public GFBaseTypePropValueList GetAllUIDataObjPropValues() => ViewModel.GetAllUIDataObjPropValues();

        /// <summary>
        /// 设置工作状态
        /// </summary>
        public void SetWorkState(string workState) => ViewModel.SetWorkState(workState);

        /// <summary>
        /// 显示执行计数
        /// </summary>
        public void ShowExecCount() => ViewModel.ShowExecCount();

        /// <summary>
        /// 设置执行方法
        /// </summary>
        public void SetExecMethod(bool execing, string curMethod = "") => ViewModel.SetExecMethod(execing, curMethod);

        /// <summary>
        /// 显示普通消息
        /// </summary>
        public void ShowMessage(string msg) => ViewModel.ShowMessage(msg);

        /// <summary>
        /// 显示错误消息
        /// </summary>
        public void ShowErrorMessage(string msg) => ViewModel.ShowErrorMessage(msg);

		/// <summary>
		/// 设置机械模组回调接口实例
		/// </summary>
		public void SetMMCmdExecutorCallBack(IMMCmdExecutorCallBack callBack) => ViewModel.SetMMCmdExecutorCallBack(callBack);
		#endregion
	}
}