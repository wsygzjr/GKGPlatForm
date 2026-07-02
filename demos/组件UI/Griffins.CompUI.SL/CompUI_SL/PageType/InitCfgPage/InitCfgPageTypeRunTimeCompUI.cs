using System;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.SL.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : IPageTypeRunTimeCompUI
    {
        private ICompUIRunTimeCallBack callBack;

        private IPageTypeRunTimeCompUIView transportMotorPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView singleTrackStationPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView regularSingleTrackPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView pcCommunicationPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView trackBasicParamPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView trackWideningPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView standardDirectWorkPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView motorMechanismPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView cylinderDelayPageTypeRunTimeCompUIView;

        private InitCfgPageTypeData initCfgPageTypeData;

        private event EventHandler afterDataModified;

        /// <summary>
        /// 页面类型ID
        /// </summary>
        PageTypeID IPageTypeRunTimeCompUI.PageTypeID { get => PageTypeID.Parse("InitCfgPage"); }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public InitCfgPageTypeRunTimeCompUI(ICompUIRunTimeCallBack callBack)
        {
            initCfgPageTypeData = new();
            this.callBack = callBack;
            this.transportMotorPageTypeRunTimeCompUIView = new TransportMotorPageTypeRunTimeCompUIView(this.callBack);
            this.singleTrackStationPageTypeRunTimeCompUIView = new SingleTrackStationPageTypeRunTimeCompUIView(this.callBack);
            this.regularSingleTrackPageTypeRunTimeCompUIView = new RegularSingleTrackPageTypeRunTimeCompUIView(this.callBack);
            this.pcCommunicationPageTypeRunTimeCompUIView = new PcCommunicationPageTypeRunTimeCompUIView(this.callBack);
            this.trackBasicParamPageTypeRunTimeCompUIView = new TrackBasicParamPageTypeRunTimeCompUIView(this.callBack);
            this.trackWideningPageTypeRunTimeCompUIView = new TrackWideningPageTypeRunTimeCompUIView(this.callBack);
            this.standardDirectWorkPageTypeRunTimeCompUIView = new StandardDirectWorkPageTypeRunTimeCompUIView(this.callBack);
            this.motorMechanismPageTypeRunTimeCompUIView = new MotorMechanismPageTypeRunTimeCompUIView(this.callBack);
            this.cylinderDelayPageTypeRunTimeCompUIView = new CylinderDelayPageTypeRunTimeCompUIView(this.callBack);

            // 事件绑定
            transportMotorPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            singleTrackStationPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            regularSingleTrackPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            pcCommunicationPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            trackBasicParamPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            trackWideningPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            standardDirectWorkPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            motorMechanismPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            cylinderDelayPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
        }

        /// <summary>
        /// 运行时组件UI修改后事件
        /// </summary>
        event EventHandler IPageTypeRunTimeCompUI.AfterDataModified
        {
            add
            {
                afterDataModified += value;
            }
            remove
            {
                afterDataModified -= value;
            }
        }

        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
        IPageTypeRunTimeCompUIView IPageTypeRunTimeCompUI.GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_TransportMotor => transportMotorPageTypeRunTimeCompUIView,
                InitCfgPageTypeConst.ViewID_SingleTrackStation => singleTrackStationPageTypeRunTimeCompUIView,
                InitCfgPageTypeConst.ViewID_RegularSingleTrack => regularSingleTrackPageTypeRunTimeCompUIView,
                InitCfgPageTypeConst.ViewID_PcCommunication => pcCommunicationPageTypeRunTimeCompUIView,
                InitCfgPageTypeConst.ViewID_TrackBasicParam => trackBasicParamPageTypeRunTimeCompUIView,
                InitCfgPageTypeConst.ViewID_TrackWidening => trackWideningPageTypeRunTimeCompUIView,
                InitCfgPageTypeConst.ViewID_StandardDirectWork => standardDirectWorkPageTypeRunTimeCompUIView,
                InitCfgPageTypeConst.ViewID_MotorMechanism => motorMechanismPageTypeRunTimeCompUIView,
                InitCfgPageTypeConst.ViewID_CylinderDelay => cylinderDelayPageTypeRunTimeCompUIView,
                _ => null,
            };
        }

        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        byte[] IPageTypeRunTimeCompUI.GetData()
        {
            return GF_Gereric.JsonObjConvert.ToJSonBytes(initCfgPageTypeData);
        }
        /// <summary>
        ///  执行界面命令
        ///  说明：主要用于内部子页面和组件界面插件之间的数据交互，如：标定子页面为内部子页面，它需要从所有组件界面插件得到标定项，
        ///  包括：标定项名称、对应的界面ID，然后自动产生对应的子页面。
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数（和命令ID对应的Json字符串）</param>
        /// <returns>返回结果（和命令ID对应的Json字符串）</returns>
        string IPageTypeRunTimeCompUI.ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        /// <summary>
        /// 设置数据信息
        /// </summary>
        /// <param name="data">数据信息，null表示缺省值</param>
        void IPageTypeRunTimeCompUI.SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            initCfgPageTypeData = GF_Gereric.JsonObjConvert.FromJSonBytes<InitCfgPageTypeData>(data);
            if (transportMotorPageTypeRunTimeCompUIView is TransportMotorPageTypeRunTimeCompUIView transportMotorcompUIView)
            {
                transportMotorcompUIView.SetData(initCfgPageTypeData.TransportMotorCompUIModel);
            }
            if (regularSingleTrackPageTypeRunTimeCompUIView is RegularSingleTrackPageTypeRunTimeCompUIView regularSingleTrackCompUIView)
            {
                regularSingleTrackCompUIView.SetData(initCfgPageTypeData.RegularSingleTrackCompUIModel);
            }
            if (pcCommunicationPageTypeRunTimeCompUIView is PcCommunicationPageTypeRunTimeCompUIView pcCommunicationcompUIView)
            {
                pcCommunicationcompUIView.SetData(initCfgPageTypeData.PcCommunicationCompUIModel);
            }
            if (singleTrackStationPageTypeRunTimeCompUIView is SingleTrackStationPageTypeRunTimeCompUIView CompUIView)
            {
                CompUIView.SetData(initCfgPageTypeData.SingleTrackStationCompUIModel);
            }
            if (trackBasicParamPageTypeRunTimeCompUIView is TrackBasicParamPageTypeRunTimeCompUIView trackBasicParamCompUIView)
            {
                trackBasicParamCompUIView.SetData(initCfgPageTypeData.TrackBasicParamCompUIModel);
            }
            if (trackWideningPageTypeRunTimeCompUIView is TrackWideningPageTypeRunTimeCompUIView trackWideningCompUIView)
            {
                trackWideningCompUIView.SetData(initCfgPageTypeData.TrackWideningCompUIModel);
            }
            if (standardDirectWorkPageTypeRunTimeCompUIView is StandardDirectWorkPageTypeRunTimeCompUIView standardDirectWorkCompUIView)
            {
                standardDirectWorkCompUIView.SetData(initCfgPageTypeData.StandardDirectWorkCompUIModel);
            }
            if (motorMechanismPageTypeRunTimeCompUIView is MotorMechanismPageTypeRunTimeCompUIView motorMechanismCompUIView)
            {
                motorMechanismCompUIView.SetData(initCfgPageTypeData.MotorMechanismCompUIModel);
            }
            if (cylinderDelayPageTypeRunTimeCompUIView is CylinderDelayPageTypeRunTimeCompUIView cylinderDelayCompUIView)
            {
                cylinderDelayCompUIView.SetData(initCfgPageTypeData.CylinderDelayCompUIModel);
            }
        }

        /// <summary>
        /// 修改事件处理
        /// </summary>
        private void OnAfterModified(object sender, EventArgs e)
        {
            if (transportMotorPageTypeRunTimeCompUIView is TransportMotorPageTypeRunTimeCompUIView transportMotorcompUIView)
            {
                initCfgPageTypeData.TransportMotorCompUIModel = transportMotorcompUIView.GetData();
            }
            if (regularSingleTrackPageTypeRunTimeCompUIView is RegularSingleTrackPageTypeRunTimeCompUIView regularSingleTrackCompUIView)
            {
                initCfgPageTypeData.RegularSingleTrackCompUIModel = regularSingleTrackCompUIView.GetData();
            }
            if (pcCommunicationPageTypeRunTimeCompUIView is PcCommunicationPageTypeRunTimeCompUIView pcCommunicationcompUIView)
            {
                initCfgPageTypeData.PcCommunicationCompUIModel = pcCommunicationcompUIView.GetData();
            }
            if (singleTrackStationPageTypeRunTimeCompUIView is SingleTrackStationPageTypeRunTimeCompUIView CompUIView)
            {
                initCfgPageTypeData.SingleTrackStationCompUIModel = CompUIView.GetData();
            }
            if (trackBasicParamPageTypeRunTimeCompUIView is TrackBasicParamPageTypeRunTimeCompUIView trackBasicParamCompUIView)
            {
                initCfgPageTypeData.TrackBasicParamCompUIModel = trackBasicParamCompUIView.GetData();
            }
            if (trackWideningPageTypeRunTimeCompUIView is TrackWideningPageTypeRunTimeCompUIView trackWideningCompUIView)
            {
                initCfgPageTypeData.TrackWideningCompUIModel = trackWideningCompUIView.GetData();
            }
            if (standardDirectWorkPageTypeRunTimeCompUIView is StandardDirectWorkPageTypeRunTimeCompUIView standardDirectWorkCompUIView)
            {
                initCfgPageTypeData.StandardDirectWorkCompUIModel = standardDirectWorkCompUIView.GetData();
            }
            if (motorMechanismPageTypeRunTimeCompUIView is MotorMechanismPageTypeRunTimeCompUIView motorMechanismCompUIView)
            {
                initCfgPageTypeData.MotorMechanismCompUIModel = motorMechanismCompUIView.GetData();
            }
            if (cylinderDelayPageTypeRunTimeCompUIView is CylinderDelayPageTypeRunTimeCompUIView cylinderDelayCompUIView)
            {
                initCfgPageTypeData.CylinderDelayCompUIModel = cylinderDelayCompUIView.GetData();
            }
            afterDataModified?.Invoke(sender, e);
        }

        public ISubPageRunTime GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            throw new NotImplementedException();
        }

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ff:后续跟据模型修改
        /// </summary>
        private class InitCfgPageTypeData
        {
            public TransportMotorCompUIModel TransportMotorCompUIModel { get; set; }

            public SingleTrackStationCompUIModel SingleTrackStationCompUIModel { get; set; }

            public RegularSingleTrackCompUIModel RegularSingleTrackCompUIModel { get; set; }

            public PcCommunicationCompUIModel PcCommunicationCompUIModel { get; set; }

            public TrackBasicParamCompUIModel TrackBasicParamCompUIModel { get; set; }

            public TrackWideningCompUIModel TrackWideningCompUIModel { get; set; }

            public StandardDirectWorkCompUIModel StandardDirectWorkCompUIModel { get; set; }

            public MotorMechanismCompUIModel MotorMechanismCompUIModel { get; set; }

            public CylinderDelayCompUIModel CylinderDelayCompUIModel { get; set; }
        }
    }
}
