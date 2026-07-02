using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins;
using Griffins.UI;
using Griffins.UI.General;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Intrinsics.Arm; 
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 事件参数窗口的ViewModel 
    /// </summary>
    public class FormEventParamWindowViewModel : ReactiveObject
    { 
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
        /// <summary>
        /// 确认命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }
        /// <summary>
        /// 取消修改命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }


        #region 绑定属性 

        /// <summary>
        ///  属性项列表
        /// </summary>
        public PropertyValueListViewModel ParamValueViewModel { private set; get; }
          
        private bool? _dialogResult;
        /// <summary>
        /// 对话框结果（true:保存，false:取消，null:未操作）
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>    
        public FormEventParamWindowViewModel(List<PropertyValueInfo> propertyValueInfoes , Avalonia.Controls.Control? viewReference)
        {
            ConfirmCommand = ReactiveCommand.CreateFromTask(OnConfirmClickedAsync);
            CancelCommand = ReactiveCommand.CreateFromTask(OnCancelClicked);

            this.ParamValueViewModel = new PropertyValueListViewModel(); 
            if (propertyValueInfoes != null && propertyValueInfoes.Count >0)
                this.ParamValueViewModel.CopyFrom(propertyValueInfoes, viewReference);
             
            this.ParamValueViewModel.IsEdit = true;

            //  监听 DialogResult 变化 → 自动关闭窗口
            this.WhenAnyValue(x => x.DialogResult)
                .Where(x => x.HasValue)
                .Subscribe(_ =>
                {
                    if (_viewReference is Window w)
                        w.Close(DialogResult);
                });
        }
        


        #region 核心业务属性（ 原WinForm的EventParam）
        /// <summary>
        /// 原业务对象：事件参数列表
        /// </summary>
        public GFBaseTypeParamValueList? EventParam { get; private set; }
        #endregion

        #region 视图回调事件（ViewModel不依赖View，通过事件通知View执行窗口操作）
        /// <summary>
        /// 确认请求事件（View订阅此事件，执行窗口关闭并返回OK）
        /// </summary>
        public event EventHandler? OnConfirmRequested;

        /// <summary>
        /// 取消请求事件（View订阅此事件，执行窗口关闭并返回Cancel）
        /// </summary>
        public event EventHandler? OnCancelRequested;
        #endregion

        #region 业务逻辑方法（ 原Form的逻辑，命令执行/初始化/类型转换）
    
        /// <summary>
        /// 确认按钮点击逻辑 
        /// </summary>
        private async Task OnConfirmClickedAsync()
        {
            try
            {
                if (ParamValueViewModel == null || ParamValueViewModel.ItemsSource.Count == 0)
                    throw new Exception("参数不能为空");
                EventParam = ParseParamTextToGFBaseType(ParamValueViewModel);
                // 把赋值移到try内部：校验通过才执行保存，空值时不会执行
                DialogResult = true;
            }
            catch (Exception ex)
            {
                // 这里是真正的异步操作，必须await，无警告
                await MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            }
        }

        private GFBaseTypeParamValueList ParseParamTextToGFBaseType(PropertyValueListViewModel paramValueViewModel)
        {
            GFBaseTypeParamValueList paramValueList = new GFBaseTypeParamValueList();
             
            try
            { 
                foreach (PropertyValueItemViewModel paramPair in paramValueViewModel.ItemsSource)
                {
                    // 3. 转换原始值为GriffinsBaseValue（核心：自动识别值类型）
                    GriffinsBaseValue griffinsValue = new GriffinsBaseValue( paramPair.GriffinsBaseDataTypeViewModel.Val);
                     
                    // 4. 创建参数对象并添加到列表
                    GFBaseTypeParamValue paramValue = new GFBaseTypeParamValue
                    {
                        ID = paramPair.ObjPropID,
                        Value = griffinsValue ,
                    };
                    paramValueList.Add(paramValue);
                }
            }
            catch (Exception ex)
            {
                // 异常时返回空列表（也可根据业务需求抛出异常）
                return new GFBaseTypeParamValueList();
            }

            return paramValueList;
        } 

        /// <summary>
        /// 取消逻辑 
        /// </summary>
        private async Task OnCancelClicked()
        {
            DialogResult = false; 
        } 

        #endregion
    }
}