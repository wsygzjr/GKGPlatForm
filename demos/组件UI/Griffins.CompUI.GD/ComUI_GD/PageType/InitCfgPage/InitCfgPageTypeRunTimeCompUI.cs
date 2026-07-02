using Griffins.CompUI.GD.ComUI_GD.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.GD.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : IPageTypeRunTimeCompUI
    {
        private ICompUIRunTimeCallBack callBack;

        private IPageTypeRunTimeCompUIView singleTrackStationView;

        private InitCfgPageTypeData initCfgPageTypeData;

        private event EventHandler? afterDataModified;

        /// <summary>
        /// 页面类型ID
        /// </summary>
        PageTypeID IPageTypeRunTimeCompUI.PageTypeID => PageTypeID.Parse("InitCfgPage");

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public InitCfgPageTypeRunTimeCompUI(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
            initCfgPageTypeData = new InitCfgPageTypeData();
            singleTrackStationView = new SingleTrackStationPageTypeRunTimeCompUIView(callBack);
            singleTrackStationView.AfterModified += OnViewAfterModified;
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
        IPageTypeRunTimeCompUIView? IPageTypeRunTimeCompUI.GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                InitCfgPageTypeConst.ViewID_SingleTrackStation => singleTrackStationView,
                _ => null,
            };
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
            if (singleTrackStationView is SingleTrackStationPageTypeRunTimeCompUIView compView)
            {
                compView.SetData(initCfgPageTypeData.SingleTrackStationCompUIModel!);
            }
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
        /// 修改事件处理
        /// </summary>
        private void OnViewAfterModified(object? sender, EventArgs e)
        {
            if (singleTrackStationView is SingleTrackStationPageTypeRunTimeCompUIView view)
            {
                initCfgPageTypeData.SingleTrackStationCompUIModel = view.GetData();
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
        /// 初始化配置页面类型数据
        /// </summary>
        private class InitCfgPageTypeData
        {
            public SingleTrackStationCompUIModel? SingleTrackStationCompUIModel { get; set; }
        }
    }
}
