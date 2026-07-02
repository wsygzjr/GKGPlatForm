using Avalonia.Controls;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using Griffins.UI.General;
using Griffins.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.DatumPoint
{
    /// <summary>
    /// 基准点参数配置-视图模型
    /// </summary>
    public class DatumPointConfigInfoViewModel : ReactiveObject
    {
        private Control? _viewReference;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        #region 响应式控件ViewModel
        /// <summary>
        /// 基准点位置教导对象列表
        /// </summary>
        public ObservableCollection<CamreaPositionViewModel> CamreaPositionViewModels { get; }

        /// <summary>
        /// 当前选中项
        /// </summary>
        private CamreaPositionViewModel? _selectedItem;
        public CamreaPositionViewModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null || value == _selectedItem) return;
                this.RaiseAndSetIfChanged(ref _selectedItem, value);
            }
        }
        #endregion

        #region 命令定义
        /// <summary>
        /// 新增项命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        /// <summary>
        /// 单条删除命令
        /// </summary>
        public ReactiveCommand<CamreaPositionViewModel, Unit> DeleteCommand { get; }

        ///// <summary>
        ///// 导入CSV命令(页面级按钮)
        ///// </summary>
        //public ReactiveCommand<Unit, Unit> ImportCommand { get; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public DatumPointConfigInfoViewModel()
        {
            CamreaPositionViewModels = new ObservableCollection<CamreaPositionViewModel>();

            // 初始化命令
            AddCommand = ReactiveCommand.Create(addItem);
            DeleteCommand = ReactiveCommand.Create<CamreaPositionViewModel>(deleteItem);
            //ImportCommand = ReactiveCommand.CreateFromTask(importFromCsv);
            // 订阅值变更事件
            subscribeValueChanges();
        }

        #region 数据同步方法
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="model">基准点配置数据模型</param>
        public void CopyFrom(DatumPointConfigInfo model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "数据模型不能为空");

            CamreaPositionViewModels.Clear();
            foreach (var item in model.Points)
            {
                var camreaPositionViewModel = new CamreaPositionViewModel()
                {
                    X = (decimal)item.X,
                    Y = (decimal)item.Y
                };
                camreaPositionViewModel.SetViewReference(_viewReference!);
                camreaPositionViewModel.AfterModified += onAfterModified;

                CamreaPositionViewModels.Add(camreaPositionViewModel);
            }
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="model">基准点配置数据模型</param>
        public void CopyTo(DatumPointConfigInfo model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "数据模型不能为空");

            model.Points.Clear();
            //foreach (var item in CamreaPositionViewModels)
            //{
            //    model.Points.Add(new Point2D()
            //    {
            //        X = (double)item.X,
            //        Y = (double)item.Y
            //    });
            //}
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 新增项
        /// </summary>
        private void addItem()
        {
            var camreaPositionViewModel = new CamreaPositionViewModel();
            camreaPositionViewModel.SetViewReference(_viewReference!);
            camreaPositionViewModel.AfterModified += onAfterModified;
            CamreaPositionViewModels.Add(camreaPositionViewModel);
            onAfterModified(this, new EventArgs());
        }

        /// <summary>
        /// 单条删除项
        /// </summary>
        private async void deleteItem(CamreaPositionViewModel item)
        {
            try
            {
                if (item == null) return;

                var confirmResult = await MessageBox.ShowConfirmDialog(
                    "删除确认",
                    $"确定要删除吗？删除后不可恢复。",
                    _viewReference
                );

                if (confirmResult == ButtonResult.Yes)
                {
                    CamreaPositionViewModels.Remove(item);
                    SelectedItem = CamreaPositionViewModels.Count > 0 ? CamreaPositionViewModels[0] : null;
                    onAfterModified(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", $"{ex.Message}", _viewReference);
            }


        }

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
           
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
        ///// <summary>
        ///// 从CSV文件导入基准点数据
        ///// </summary>
        //private async Task importFromCsv()
        //{
        //    try
        //    {
        //        // 1. 弹出文件选择框，筛选CSV文件
        //        var fileDialog = new OpenFileDialog
        //        {
        //            Title = "选择基准点CSV文件",
        //            Filters = new List<FileDialogFilter>
        //            {
        //                new FileDialogFilter { Name = "CSV文件", Extensions = { "csv" } },
        //                new FileDialogFilter { Name = "所有文件", Extensions = { "*" } }
        //            },
        //            AllowMultiple = false 
        //        };

        //        // 2. 获取选择的文件路径
        //        var parentWindow = _viewReference?.GetVisualRoot() as Window;
        //        var selectedFiles = await fileDialog.ShowAsync(parentWindow);
        //        if (selectedFiles == null || selectedFiles.Length == 0)
        //            return; // 用户取消选择

        //        string csvPath = selectedFiles[0];

        //        // 3. 读取并解析CSV文件
        //        var importPoints = await parseCsvFile(csvPath);
        //        if (importPoints.Count == 0)
        //        {
        //            await MessageBox.ShowInfoDialog("提示", "CSV文件中未解析到有效数据", _viewReference);
        //            return;
        //        }

        //        // 4. 将解析后的数据插入到集合中
        //        foreach (var point in importPoints)
        //        {
        //            var vm = new CamreaPositionViewModel
        //            {
        //                X = point.X,
        //                Y = point.Y
        //            };
        //            vm.SetViewReference(_viewReference!);
        //            CamreaPositionViewModels.Add(vm);
        //        }

        //        // 5. 导入成功提示
        //        await MessageBox.ShowInfoDialog(
        //            "导入成功",
        //            $"共导入 {importPoints.Count} 个基准点数据",
        //            _viewReference
        //        );

        //        // 6. 选中第一个新增项
        //        if (importPoints.Count > 0)
        //        {
        //            SelectedItem = CamreaPositionViewModels[CamreaPositionViewModels.Count - importPoints.Count];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await MessageBox.ShowErrorDialog("导入失败", $"CSV导入出错：{ex.Message}", _viewReference);
        //    }
        //}

        ///// <summary>
        ///// 解析CSV文件，提取X/Y坐标
        ///// </summary>
        ///// <param name="csvPath"></param>
        ///// <returns></returns>
        //private async Task<List<(decimal X, decimal Y)>> parseCsvFile(string csvPath)
        //{
        //    var result = new List<(decimal X, decimal Y)>();

        //    return result;
        //}
        #endregion
    }
}