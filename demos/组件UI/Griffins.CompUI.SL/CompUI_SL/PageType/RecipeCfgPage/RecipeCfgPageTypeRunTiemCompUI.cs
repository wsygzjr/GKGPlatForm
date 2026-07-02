using Griffins.CompUI.SL.RecipeCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.SL.RecipeCfgPage
{
    internal class RecipeCfgPageTypeRunTiemCompUI : IPageTypeRunTimeCompUI
    {
        private ICompUIRunTimeCallBack callBack;

        private IPageTypeRunTimeCompUIView motorSpeedPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView trackWidening‌PageTypeRunTimeCompUIView;

        private RecipeCfgPageTypeData recipeCfgPageTypeData;

        private event EventHandler afterDataModified;

        /// <summary>
        /// 页面类型ID
        /// </summary>
        PageTypeID IPageTypeRunTimeCompUI.PageTypeID { get => PageTypeID.Parse("RecipeCfgPage"); }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public RecipeCfgPageTypeRunTiemCompUI(ICompUIRunTimeCallBack callBack)
        {
            recipeCfgPageTypeData = new();
            this.callBack = callBack;
            this.motorSpeedPageTypeRunTimeCompUIView = new MotorSpeedPageTypeRunTimeCompUIView(this.callBack);
            this.trackWidening‌PageTypeRunTimeCompUIView = new TrackWideningPageTypeRunTimeCompUIView(this.callBack);

            // 事件绑定
            motorSpeedPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
            trackWidening‌PageTypeRunTimeCompUIView.AfterModified += OnAfterModified;
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
                RecipeCfgPageTypeConst.ViewID_MotorSpeed => motorSpeedPageTypeRunTimeCompUIView,
                RecipeCfgPageTypeConst.ViewID_TrackWidening => trackWidening‌PageTypeRunTimeCompUIView,
                _ => null,
            };
        }

        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        byte[] IPageTypeRunTimeCompUI.GetData()
        {
            return GF_Gereric.JsonObjConvert.ToJSonBytes(recipeCfgPageTypeData);
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

            recipeCfgPageTypeData = GF_Gereric.JsonObjConvert.FromJSonBytes<RecipeCfgPageTypeData>(data);
            if (motorSpeedPageTypeRunTimeCompUIView is MotorSpeedPageTypeRunTimeCompUIView motorSpeedCompUIView)
            {
                motorSpeedCompUIView.SetData(recipeCfgPageTypeData.MotorSpeedCompUIModel‌);
            }
            else if(trackWidening‌PageTypeRunTimeCompUIView is TrackWideningPageTypeRunTimeCompUIView trackWideningCompUIView)
            {
                trackWideningCompUIView.SetData(recipeCfgPageTypeData.TrackWideningCompUIModel‌);
            }


        }

        /// <summary>
        /// 修改事件处理
        /// </summary>
        private void OnAfterModified(object sender, EventArgs e)
        {
            if (motorSpeedPageTypeRunTimeCompUIView is MotorSpeedPageTypeRunTimeCompUIView motorSpeedCompUIView)
            {
                recipeCfgPageTypeData.MotorSpeedCompUIModel‌ = motorSpeedCompUIView.GetData();
                afterDataModified?.Invoke(sender, e);
            }
            else if (trackWidening‌PageTypeRunTimeCompUIView is TrackWideningPageTypeRunTimeCompUIView trackWideningCompUIView)
            {
                recipeCfgPageTypeData.TrackWideningCompUIModel‌ = trackWideningCompUIView.GetData();
                afterDataModified?.Invoke(sender, e);
            }

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
        private class RecipeCfgPageTypeData
        {
            public MotorSpeedCompUIModel‌ MotorSpeedCompUIModel‌ { get; set; }

            public TrackWideningCompUIModel‌ TrackWideningCompUIModel‌ { get; set; }
        }
    }
}
