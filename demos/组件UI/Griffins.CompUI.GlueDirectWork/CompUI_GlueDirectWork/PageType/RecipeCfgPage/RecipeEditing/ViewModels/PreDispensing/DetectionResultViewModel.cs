using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaVisionControl;
using DynamicData;
using GKG.UI;
using GKG.UI.General;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 检测结果界面-视图模型
    /// </summary>
    public class DetectionResultViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        private ScriptModelList _scriptModelList;

        private ScriptModel? _selectedScriptModelItem;

        /// <summary>
        /// 脚本计数器
        /// </summary>
        private int _scriptIndexCount = 0;

        #endregion

        #region 委托

        /// <summary>
        /// 获取最新的图像预处理参数配置
        /// </summary>
        public Func<ImagePreProcessCfgInfo>? GetLatestImagePreProcessCfg { get; set; }

        /// <summary>
        /// 获取最新的脚本参数配置
        /// </summary>
        public Func<ScriptParamCfgInfo>? GetLatestScriptParamCfg { get; set; }

        /// <summary>
        /// 获取最新的图像预处理参数配置
        /// </summary>
        public Func<PassConditionCfgInfo>? GetLatestPassConditionCfg { get; set; }

        /// <summary>
        /// 获取最新的脚本参数配置
        /// </summary>
        public Func<DetectionResultCfgInfo>? GetLatestDetectionResultCfg { get; set; }

        /// <summary>
        /// 加载选中的脚本配置到编辑界面
        /// </summary>
        public Action<ImagePreProcessCfgInfo?, ScriptParamCfgInfo?, PassConditionCfgInfo?>? LoadSelectedScriptCfg { get; set; }

        public Action? ResetScroptParamViewModel { get; set; }

        /// <summary>
        /// 保存配置
        /// </summary>
        public Action? SaveCfg { get; set; }

        #endregion

        #region UI组件模组

        /// <summary>
        /// 检测逻辑-下拉框视图模型
        /// </summary>
        public ComboxViewModel DetectionLogicViewModel { get; set; }

        #endregion

        #region 属性

        /// <summary>
        /// 当前编辑的脚本 ff:后续可能要修改
        /// </summary>
        public ScriptModelList ScriptModelList
        {
            get => _scriptModelList;
            set
            {
                this.RaiseAndSetIfChanged(ref _scriptModelList, value);
                UpdateScriptIndexCount();
            }
        }

        /// <summary>
        /// 当前选中的脚本模型项
        /// </summary>
        public ScriptModel? SelectedScriptModelItem
        {
            get => _selectedScriptModelItem;
            set
            {
                //保存当前选中的配置模型
                SaveCurrentConfigToScript(_selectedScriptModelItem);

                //重置脚本参数界面的参数配置
                ResetScroptParamViewModel?.Invoke();

                this.RaiseAndSetIfChanged(ref _selectedScriptModelItem, value);

                // 加载新选中项的配置到界面
                if (_selectedScriptModelItem != null)
                {
                    LoadSelectedScriptCfg?.Invoke(_selectedScriptModelItem.ImagePreProcessCfgInfo, _selectedScriptModelItem.ScriptParamCfgInfo, _selectedScriptModelItem.PassConditionCfgInfo);
                    DetectionLogicType = _selectedScriptModelItem.DetectionLogicType;
                }
            }
        }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 响应属性

        /// <summary>
        /// 检测逻辑
        /// </summary>
        public DetectionLogicType DetectionLogicType
        {
            get => (DetectionLogicType)((DetectionLogicViewModel.SelectedItem as ComBoxItem)?.Value ?? DetectionLogicType.AllPass);
            set
            {
                if (DetectionLogicViewModel.ItemsSource != null)
                {
                    var targetItem = DetectionLogicViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (DetectionLogicType)o.Value == value);
                    if (targetItem != null)
                    {
                        DetectionLogicViewModel.SelectedItem = targetItem;
                    }
                    this.RaisePropertyChanged(nameof(DetectionLogicType));
                }
            }
        }



        #endregion

        #region 命令
        /// <summary>
        /// 相加命令 ff:后续可能要修改
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }
        /// <summary>
        /// 相减命令 ff:后续可能要修改
        /// </summary>
        public ReactiveCommand<Unit, Unit> SubtractCommand { get; set; }
        /// <summary>
        /// 正方形ROI按键命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddSquareRoiCommand { get; }
        /// <summary>
        /// 圆形ROI按键命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCircleRoiCommand { get; }
        /// <summary>
        /// 多边形ROI按键命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddPolygonRoiCommand { get; }
        /// <summary>
        /// 删除选中ROI命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> DeleteSelectedRoiCommand { get; }
        /// <summary>
        /// 清除所有ROI命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClearAllRoisCommand { get; }
        /// <summary>
        /// 测试当前script
        /// </summary>
        public ReactiveCommand<Unit, Unit> TestCurrentScriptCommand { get; }
        /// <summary>
        /// 测试所有script
        /// </summary>
        public ReactiveCommand<Unit, Unit> TestAllScriptsCommand { get; }
        /// <summary>
        /// 新建
        /// </summary>
        public ReactiveCommand<Unit, Unit> NewScriptCommand { get; }
        /// <summary>
        /// 删除
        /// </summary>
        public ReactiveCommand<Unit, Unit> DeleteScriptCommand { get; }
        /// <summary>
        /// 清空
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClearAllScriptsCommand { get; }

        /// <summary>
        /// 保存
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        #endregion

        /// <summary>
        /// 构造方法（初始化组件、命令、事件订阅）
        /// </summary>
        public DetectionResultViewModel()
        {
            //初始化数据模型
            _scriptModelList = new();//初期为空列表

            //初始化命令绑定
            AddCommand = ReactiveCommand.CreateFromTask(Add);
            SubtractCommand = ReactiveCommand.CreateFromTask(Subtract);
            AddSquareRoiCommand = ReactiveCommand.CreateFromTask(AddSquareRoi);
            AddCircleRoiCommand = ReactiveCommand.CreateFromTask(AddCircleRoi);
            AddPolygonRoiCommand = ReactiveCommand.CreateFromTask(AddPolygonRoi);
            DeleteSelectedRoiCommand = ReactiveCommand.CreateFromTask(DeleteSelectedRoi);
            ClearAllRoisCommand = ReactiveCommand.CreateFromTask(ClearAllRois);
            TestCurrentScriptCommand = ReactiveCommand.Create(TestCurrentScript);
            TestAllScriptsCommand = ReactiveCommand.Create(TestAllScripts);
            NewScriptCommand = ReactiveCommand.Create(NewScript);
            DeleteScriptCommand = ReactiveCommand.Create(DeleteScript);
            ClearAllScriptsCommand = ReactiveCommand.Create(ClearAllScripts);
            SaveCommand = ReactiveCommand.Create(Save);
            // 初始化UI组件模型
            DetectionLogicViewModel = new();

            var detectionLogicDisplayNames = new Dictionary<DetectionLogicType, string>
            {
                { DetectionLogicType.AllPass, "全部脚本通过则OK" },
                { DetectionLogicType.AnyPass, "有一个脚本通过则OK" }
            };

            DetectionLogicViewModel.ItemsSource = EnumExtensions.ToEnumItems(detectionLogicDisplayNames);
            DetectionLogicViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            // 订阅值改变事件
            subscribeValueChanges();
        }

        #region 命令初始化与执行逻辑
        /// <summary>
        /// 相加指令
        /// </summary>
        private async Task Add()
        {
            //try
            //{
            //    throw new NotImplementedException();
            //}
            //catch (Exception ex)
            //{
            //    await MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            //}
        }
        /// <summary>
        /// 相减指令
        /// </summary>
        private async Task Subtract()
        {
            //try
            //{
            //    throw new NotImplementedException();
            //}
            //catch (Exception ex)
            //{
            //    await MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            //}
        }
        /// <summary>
        /// 正方形指令
        /// </summary>
        private Task AddSquareRoi()
        {
            beginRoiDraw("Square");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 圆形指令
        /// </summary>
        private Task AddCircleRoi()
        {
            beginRoiDraw("Circle");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 多边形指令
        /// </summary>
        private Task AddPolygonRoi()
        {
            beginRoiDraw("Polygon");
            return Task.CompletedTask;
        }

        private static void beginRoiDraw(string drawMapType)
        {
            var cameraVm = GlobalVisionViewModel.CameraShowViewModel;
            if (cameraVm == null)
            {
                return;
            }

            cameraVm.DrawMapType = drawMapType;
            cameraVm.IsDrawMap = true;
        }

        /// <summary>
        /// 删除选中ROI指令
        /// </summary>
        private async Task DeleteSelectedRoi()
        {
            if (!GlobalVisionViewModel.CurActiveViewImageControl.IsFocused)
            {
                GlobalVisionViewModel.CurActiveViewImageControl.Focus();
            }
            var backspaceArgs = new KeyEventArgs
            {
                RoutedEvent = InputElement.KeyDownEvent,
                Key = Key.Delete,
                Source = GlobalVisionViewModel.CurActiveViewImageControl
            };

            GlobalVisionViewModel.CurActiveViewImageControl.RaiseEvent(backspaceArgs);
        }

        /// <summary>
        /// 清除所有ROI指令
        /// </summary>
        private async Task ClearAllRois()
        {
            ImageControlHelper.ClearPaintElements(GlobalVisionViewModel.CurActiveViewImageControl);
        }

        /// <summary>
        /// 测试当前脚本
        /// </summary>
        private void TestCurrentScript()
        {

        }

        /// <summary>
        /// 测试所有脚本
        /// </summary>
        private void TestAllScripts()
        {

        }

        /// <summary>
        /// 新建脚本
        /// </summary>
        private void NewScript()
        {
            try
            {
                // 获取最新配置
                var imgCfg = new ImagePreProcessCfgInfo();
                var scriptCfg = new ScriptParamCfgInfo();

                //在列表中添加一个新的脚本模型
                var newScript = new ScriptModel()
                {
                    ScriptName = "script" + _scriptIndexCount++,
                    ScriptResult = ScriptResultType.Default,
                    ImagePreProcessCfgInfo = imgCfg,
                    ScriptParamCfgInfo = scriptCfg
                };

                ScriptModelList.Add(newScript);
            }
            catch (Exception ex)
            {

                MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            }
        }

        /// <summary>
        /// 删除脚本
        /// </summary>
        private void DeleteScript()
        {
            try
            {
                if (SelectedScriptModelItem != null && ScriptModelList.Contains(SelectedScriptModelItem))
                {
                    ScriptModelList.Remove(SelectedScriptModelItem);
                }
            }
            catch (Exception ex)
            {

                MessageBox.ShowErrorDialog("错误", ex.Message, _viewReference);
            }
        }

        /// <summary>
        /// 清空脚本列表
        /// </summary>
        private void ClearAllScripts()
        {
            ScriptModelList.Clear();
            _scriptIndexCount = 0;
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void Save()
        {
            //保存当前选中想到配置模型
            SaveCurrentConfigToScript(SelectedScriptModelItem);
            SaveCfg?.Invoke();
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel ff:疑问
        /// </summary>
        public void CopyFrom(DetectionResultCfgInfo model)
        {
            // 深拷贝
            var newList = new ScriptModelList();
            if (model.ScriptModelList != null)
            {
                var jArray = model.ScriptModelList.ToJArray();
                newList.FromJObject(jArray);
            }

            ScriptModelList = newList;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(DetectionResultCfgInfo model)
        {
            // 深拷贝
            var newList = new ScriptModelList();
            if (ScriptModelList != null)
            {
                var jArray = ScriptModelList.ToJArray();
                newList.FromJObject(jArray);
            }

            model.ScriptModelList = newList;
        }

        /// <summary>
        /// 保存当前界面的配置到指定的脚本模型中
        /// </summary>
        /// <param name="targetScript">目标脚本模型</param>
        private void SaveCurrentConfigToScript(ScriptModel? targetScript)
        {
            if (targetScript == null) return;

            var itemInList = ScriptModelList.FirstOrDefault(s => s == targetScript);
            // 获取界面上最新的配置
            var currentImgCfg = GetLatestImagePreProcessCfg?.Invoke();
            var currentScriptCfg = GetLatestScriptParamCfg?.Invoke();
            var currentPassConditionCfg = GetLatestPassConditionCfg?.Invoke();

            if (itemInList != null)
            {
                // 更新列表项的数据
                if (currentImgCfg != null)
                    itemInList.ImagePreProcessCfgInfo = currentImgCfg;

                if (currentScriptCfg != null)
                    itemInList.ScriptParamCfgInfo = currentScriptCfg;

                if (currentPassConditionCfg != null)
                    itemInList.PassConditionCfgInfo = currentPassConditionCfg;

                itemInList.DetectionLogicType = DetectionLogicType;
            }
        }

        /// <summary>
        /// 更新脚本计数器，根据现有脚本名称的最大索引设置起始值
        /// </summary>
        private void UpdateScriptIndexCount()
        {
            _scriptIndexCount = 0;
            if (_scriptModelList != null)
            {
                foreach (var item in _scriptModelList)
                {
                    if (!string.IsNullOrEmpty(item.ScriptName) && item.ScriptName.StartsWith("script"))
                    {
                        if (int.TryParse(item.ScriptName.Substring(6), out int index))
                        {
                            if (index >= _scriptIndexCount)
                            {
                                _scriptIndexCount = index + 1;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 值改变事件订阅与处理

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            DetectionLogicViewModel.ValueChanged += onAfterModified;

            this.WhenAnyValue(x => x.ScriptModelList)
                .Where(list => list != null)
                .Select(list => Observable
                .FromEventPattern<System.Collections.Specialized.NotifyCollectionChangedEventHandler, System.Collections.Specialized.NotifyCollectionChangedEventArgs>(
 h => list.CollectionChanged += h, h => list.CollectionChanged -= h))
                .Switch()
                .Subscribe(_ => onAfterModified(this, EventArgs.Empty));
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        private void onAfterModified(object? sender, EventArgs e)
        {
            if (sender == DetectionLogicViewModel && SelectedScriptModelItem != null)
            {
                SelectedScriptModelItem.DetectionLogicType = DetectionLogicType;
            }

            AfterModified?.Invoke(sender, e);
        }

        #endregion
    }
}
