using Griffins.CompUI.SL.FactoryCfgPage.Models;
using Griffins.CompUI.SL.InitCfgPage;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.SL.FactoryCfgPage
{
    internal class FactoryCfgPageTypeRunTimeCompUI : IPageTypeRunTimeCompUI
    {
        private ICompUIRunTimeCallBack callBack;

        private IPageTypeRunTimeCompUIView trackWideningPageTypeRunTimeCompUIView;

        private FactoryCfgPageTypeData factoryCfgPageTypeData;

        private event EventHandler afterDataModified;

        /// <summary>
        /// 页面类型ID
        /// </summary>
        PageTypeID IPageTypeRunTimeCompUI.PageTypeID { get => PageTypeID.Parse("FactoryCfgPage"); }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public FactoryCfgPageTypeRunTimeCompUI(ICompUIRunTimeCallBack callBack)
        {
            factoryCfgPageTypeData = new();
            this.callBack = callBack;
            this.trackWideningPageTypeRunTimeCompUIView = new TrackWideningPageTypeRunTimeCompUIView(this.callBack);
            // 事件绑定
            trackWideningPageTypeRunTimeCompUIView.AfterModified += OnAfterModified;

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
                FactoryCfgPageTypeConst.ViewID_TrackWidening => trackWideningPageTypeRunTimeCompUIView,
                _ => null,
            };
        }

        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        byte[] IPageTypeRunTimeCompUI.GetData()
        {
            return GF_Gereric.JsonObjConvert.ToJSonBytes(factoryCfgPageTypeData);
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

            factoryCfgPageTypeData = GF_Gereric.JsonObjConvert.FromJSonBytes<FactoryCfgPageTypeData>(data);
            if (trackWideningPageTypeRunTimeCompUIView is TrackWideningPageTypeRunTimeCompUIView trackWideningCompUIView)
            {
                trackWideningCompUIView.SetData(factoryCfgPageTypeData.TrackWideningCompUIModel‌);
            }
            

        }

        /// <summary>
        /// 修改事件处理
        /// </summary>
        private void OnAfterModified(object sender, EventArgs e)
        {
            if (trackWideningPageTypeRunTimeCompUIView is TrackWideningPageTypeRunTimeCompUIView trackWideningCompUIView)
            {
                factoryCfgPageTypeData.TrackWideningCompUIModel‌ = trackWideningCompUIView.GetData();
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
        private class FactoryCfgPageTypeData
        {
            public TrackWideningCompUIModel‌ TrackWideningCompUIModel‌ { get; set; }

            
        }
    }
}
