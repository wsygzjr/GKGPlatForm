
using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Mark
{
    /// <summary>
    /// Mark点-视图模型
    /// </summary>
    public class MarkPointViewModel : DataGridItemBaseViewModel<MarkPointInfo>
    {

        /// <summary>
        /// 点ID
        /// </summary>
        private Guid _pointID;
        public Guid PointID
        {
            get => _pointID;
            set => this.RaiseAndSetIfChanged(ref _pointID, value);
        }

        /// <summary>
        /// 点ID名称
        /// </summary>
        private string _pointName="";
        public string PointName
        {
            get => _pointName;
            set => this.RaiseAndSetIfChanged(ref _pointName, value);
        }

        /// <summary>
        ///Mark点项配置
        /// </summary>
        public MarkPointItemConfigViewModel MarkPointItemConfigViewModel { get; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public MarkPointViewModel()
        {
            MarkPointItemConfigViewModel = new MarkPointItemConfigViewModel();

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 复制
        /// </summary>
        public override void CopyFrom(MarkPointInfo markPointInfo)
        {
            base.CopyBasePropertiesFrom(markPointInfo);
            PointID = markPointInfo.PointID;
            PointName = markPointInfo.PointName;
            SerialNumber = markPointInfo.SerialNumber;
            MarkPointItemConfigViewModel.ItemsSource.Clear();
            foreach (var item in markPointInfo.MarkPointPositionInfoes)
            {
                var markPointViewModel = MarkPointItemConfigViewModel.AddItem();
                markPointViewModel.CopyFrom(item);
            }
        }

        /// <summary>
        /// 复制到指定对象
        /// </summary>
        public override void CopyTo(MarkPointInfo markPointInfo)
        {
            base.CopyBasePropertiesTo(markPointInfo);
            markPointInfo.PointID = PointID;
            markPointInfo.SerialNumber = SerialNumber;
            markPointInfo.PointName = PointName;
            MarkPointItemConfigViewModel.CopyTo(markPointInfo.MarkPointPositionInfoes);

        }
       
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public override void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            MarkPointItemConfigViewModel.SetViewReference(view);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            MarkPointItemConfigViewModel.AfterModified += onAfterModified;
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
    }
}
