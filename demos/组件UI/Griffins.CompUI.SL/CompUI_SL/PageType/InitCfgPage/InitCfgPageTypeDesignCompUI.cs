using Griffins.CompUI.SL.InitCfgPage.Views;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : IPageTypeDesignCompUI
    {
        private ICompUIRunTimeCallBack callBack;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public InitCfgPageTypeDesignCompUI(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
        }

        /// <summary>
        /// 页面类型ID
        /// </summary>
        PageTypeID IPageTypeDesignCompUI.PageTypeID { get => PageTypeID.Parse("InitCfgPage"); }

        /// <summary>
        /// 获取组件实例所有的页面类型组件界面视图信息，null或个数为0表示没有对应的界面
        /// </summary>
        /// <returns>该机械模组所有的出厂配置参数配置界面信息列表</returns>
        PageTypeCompUIViewInfoList IPageTypeDesignCompUI.GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_TransportMotor, ViewName = ResourceA.ViewName_TransportMotor },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_SingleTrackStation, ViewName = ResourceA.ViewName_SingleTrackStation },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_RegularSingleTrack, ViewName = ResourceA.ViewName_RegularSingleTrack },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_PcCommunication, ViewName = ResourceA.ViewName_PcCommunication },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_TrackBasicParam, ViewName = ResourceA.ViewName_TrackBasicParam },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_TrackWidening, ViewName = ResourceA.ViewName_TrackWidening },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_StandardDirectWork, ViewName = ResourceA.ViewName_StandardDirectWork },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_MotorMechanism, ViewName = ResourceA.ViewName_MotorMechanism },
                new PageTypeCompUIViewInfo() { ViewID = InitCfgPageTypeConst.ViewID_CylinderDelay, ViewName = ResourceA.ViewName_CylinderDelay },
            };
        }

        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
        object IPageTypeDesignCompUI.GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_TransportMotor => new TransportMotorCompUIView(),
                InitCfgPageTypeConst.ViewID_SingleTrackStation => new SingleTrackStationCompUIView(),
                InitCfgPageTypeConst.ViewID_RegularSingleTrack => new RegularSingleTrackCompUIView(),
                InitCfgPageTypeConst.ViewID_PcCommunication => new PcCommunicationCompUIView(),
                InitCfgPageTypeConst.ViewID_TrackBasicParam => new TrackBasicParamCompUIView(),
                InitCfgPageTypeConst.ViewID_TrackWidening => new TrackWideningCompUIView(),
                InitCfgPageTypeConst.ViewID_StandardDirectWork => new StandardDirectWorkCompUIView(),
                InitCfgPageTypeConst.ViewID_MotorMechanism => new MotorMechanismCompUIView(),
                InitCfgPageTypeConst.ViewID_CylinderDelay => new CylinderDelayCompUIView(),
                _ => null,
            };
        }

        public InnerSubPageTypeInfoList GetInnerSubPageTypeInfoes()
        {
            throw new System.NotImplementedException();
        }

        public IInnerSubPageDesignTime CreateDesignTime(InnerSubPageTypeID innerSubPageTypeID)
        {
            throw new System.NotImplementedException();
        }

        public void Init(ICompUIDesignCallBack callBack)
        {
            throw new System.NotImplementedException();
        }
    }
}
