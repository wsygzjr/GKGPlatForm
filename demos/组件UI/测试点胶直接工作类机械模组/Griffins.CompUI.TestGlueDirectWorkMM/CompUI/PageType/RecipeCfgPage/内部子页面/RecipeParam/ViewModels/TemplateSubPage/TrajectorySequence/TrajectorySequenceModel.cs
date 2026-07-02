using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 轨迹序列配置模型
    /// </summary>
    public class TrajectorySequenceModel : TrajectorySequenceBaseModel
    {
        private EventHandler<PointChangedEventArgs>? _pointChanged;
        /// <summary>
        /// 点工作区视图实例
        /// </summary>
        private PointWorkAreaCfgView? _pointWorkAreaCfgView;
        /// <summary>
        /// 点计算轨迹信息-视图模型
        /// </summary>
        public PointTrajectoryWorkAreaViewModel? PointTrajectoryCfgViewModel { get; set; }

        /// <summary>
        /// 线工作区视图实例
        /// </summary>
        private LineWorkAreaCfgView? _lineWorkAreaCfgView;
        /// <summary>
        /// 线计算轨迹信息-视图模型
        /// </summary>
        public LineWorkAreaViewModel? LineWorkAreaViewModel { get; set; }

        /// <summary>
        /// 工作区的视图实例
        /// 创建对象时就决定是工作区的view，所以不需要通知UI处理
        /// </summary>
        public object? WorkAreaView { get; set; }

        /// <summary>
        /// 工艺参数
        /// </summary>
        public object ProcessParameters { get; set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public TrajectorySequenceModel() :base()
        {
            ProcessParameters = new object();
            base.CanEdit = true;
            // 订阅值变更事件
            subscribeValueChanges();
        }
       
      
        /// <summary>
        /// 根据轨迹类型更新相关控件
        /// </summary>
        protected override void _onUpdateTrajectoryByCalculateTrajectoryType()
        {
            base._onUpdateTrajectoryByCalculateTrajectoryType();
            switch (CalculateTrajectoryType)
            {
                case CalculateTrajectoryType.Point:
                    SelectedStyleModel.ItemsSource = getStyleSource(CalculateTrajectoryType.Point);
                    PointTrajectoryCfgViewModel = new PointTrajectoryWorkAreaViewModel();
                    PointTrajectoryCfgViewModel.SubscribPointChanged(_pointChanged);
                    PointTrajectoryCfgViewModel.CopyFrom(new PointCalculateTrajectory());
                    PointTrajectoryCfgViewModel.AfterModified += onAfterModified;

                    _pointWorkAreaCfgView = new PointWorkAreaCfgView() { DataContext = this };
                    WorkAreaView = _pointWorkAreaCfgView;
                    break;
                case CalculateTrajectoryType.Line:
                    //线轨迹工作区
                    SelectedStyleModel.ItemsSource = getStyleSource(CalculateTrajectoryType.Line);
                    LineWorkAreaViewModel = new LineWorkAreaViewModel();
                    LineWorkAreaViewModel.CopyFrom(new LineCalculateTrajectory());
                    LineWorkAreaViewModel.SetViewReference(_viewReference!);
                    LineWorkAreaViewModel.SubscribPointChanged(_pointChanged);
                    LineWorkAreaViewModel.AfterModified += onAfterModified;

                    _lineWorkAreaCfgView = new LineWorkAreaCfgView() { DataContext = this };
                    WorkAreaView = _lineWorkAreaCfgView;
                    break;
            }
        }

        /// <summary>
        /// 从信息对象复制数据
        /// </summary>
        /// <param name="info"></param>
        public new void CopyFrom(TrajectorySequenceCfgInfo info)
        {
            if (info == null) return;
            base.CopyFrom(info);
          
            if(CalculateTrajectoryType== CalculateTrajectoryType.Point)
            {
                if (PointTrajectoryCfgViewModel == null)
                    throw new Exception("点信息视图模型为空");
                PointTrajectoryCfgViewModel.CopyFrom((PointCalculateTrajectory)info.CalculateTrajectory);

            }
            else if (CalculateTrajectoryType == CalculateTrajectoryType.Line)
            {
                if (LineWorkAreaViewModel == null)
                    throw new Exception("线信息视图模型为空");
                LineWorkAreaViewModel.CopyFrom((LineCalculateTrajectory)info.CalculateTrajectory);
            }
            ProcessParameters = info.ProcessParameters;
        }

        /// <summary>
        /// 复制到信息对象
        /// </summary>
        /// <param name="targetInfo"></param>
        public new void CopyTo(TrajectorySequenceCfgInfo targetInfo)
        {
            if (targetInfo == null) return;
            base.CopyFrom(targetInfo);
           
            if (CalculateTrajectoryType == CalculateTrajectoryType.Point)
            {
                if (PointTrajectoryCfgViewModel == null)
                    throw new Exception("点信息视图模型为空");
                var pointCalculateTrajectory = new PointCalculateTrajectory();
                PointTrajectoryCfgViewModel.CopyTo(pointCalculateTrajectory);
                targetInfo.CalculateTrajectory = pointCalculateTrajectory;
            }
            else if (CalculateTrajectoryType == CalculateTrajectoryType.Line)
            {
                if (LineWorkAreaViewModel == null)
                    throw new Exception("线信息视图模型为空");
                var lineCalculateTrajectory = new LineCalculateTrajectory();
                LineWorkAreaViewModel.CopyTo(lineCalculateTrajectory);
                targetInfo.CalculateTrajectory = lineCalculateTrajectory;
            }

            targetInfo.ProcessParameters = ProcessParameters;
        }
        /// <summary>
        /// 设置点坐标
        /// </summary>
        public void SetPointWhenEditChanged(decimal x, decimal y, decimal z)
        {
            if (CalculateTrajectoryType == CalculateTrajectoryType.Point)
            {
                if (PointTrajectoryCfgViewModel == null)
                    throw new Exception("点信息视图模型为空");
                PointTrajectoryCfgViewModel.SetPointWhenEditChanged(x,y,z);
            }
            else if (CalculateTrajectoryType == CalculateTrajectoryType.Line)
            {
                if (LineWorkAreaViewModel == null)
                    throw new Exception("线信息视图模型为空");
                LineWorkAreaViewModel.SetPointWhenEditChanged(x,y,z);
            }

        }
        /// <summary>
        /// 订阅点改变事件
        /// </summary>
        /// <param name="styleItemChanged"></param>
        public  void SubscribPointChanged(EventHandler<PointChangedEventArgs>? pointChanged)
        {
            _pointChanged= pointChanged;
        }
        /// <summary>
        /// 平移 
        /// </summary>
        /// <param name="translationParam"></param>
        public void Translation(TranslationParam translationParam)
        {
            if (CalculateTrajectoryType == CalculateTrajectoryType.Point)
            {
                if (PointTrajectoryCfgViewModel == null)
                    throw new Exception("点信息视图模型为空");
                PointTrajectoryCfgViewModel.Translation(translationParam);
            }
            else if (CalculateTrajectoryType == CalculateTrajectoryType.Line)
            {
                if (LineWorkAreaViewModel == null)
                    throw new Exception("线信息视图模型为空");
                LineWorkAreaViewModel.Translation(translationParam);
            }
        }
        /// <summary>
        /// 获取样式数据源
        /// </summary>
        /// <returns></returns>
        private List<ComBoxItem> getStyleSource(CalculateTrajectoryType calculateTrajectoryType)
        {
            List<DispensingStyleCfgBaseInfo> dispensingStyleCfgBaseInfos = new List<DispensingStyleCfgBaseInfo>();
           DispensingStyleCfgInfo dispensingStyleCfgInfo=CacheDataExchange.GetDispensingStyleCfgInfo();
            switch (calculateTrajectoryType)
            {
                case CalculateTrajectoryType.Point:
                    //取点的点胶前和点胶后的样式
                    foreach (var item in dispensingStyleCfgInfo.DispensingPointStyleCfgInfo.DispensingBeforePointStyleCfgInfoes)
                    {
                        dispensingStyleCfgBaseInfos.Add(item);
                    }
                    foreach (var item in dispensingStyleCfgInfo.DispensingPointStyleCfgInfo.DispensingAfterPointStyleCfgInfoes)
                    {
                        dispensingStyleCfgBaseInfos.Add(item);
                    }
                    break;
                case CalculateTrajectoryType.Line:
                    //取线点胶前和点胶后的样式
                    foreach (var item in dispensingStyleCfgInfo.DispensingLineStyleCfgInfo.DispensingBeforeAfterLineStyleCfgInfoes)
                    {
                        dispensingStyleCfgBaseInfos.Add(item);
                    }
                    break;
                default:
                    break;
            }
            List<ComBoxItem> items = new List<ComBoxItem>();
            foreach (var item in dispensingStyleCfgBaseInfos)
            {
                items.Add(new ComBoxItem()
                {
                    Value = item.StyleID,
                    DisplayName = item.StyleName
                });
            }
            return items;
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
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }
}