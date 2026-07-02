using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.PropertyGrid.Controls;
using DynamicData;
using Griffins;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.UI;
using Griffins.UI2;
using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    class CompUIRunTime : PageTypeRunTimeCompUIBase
    {
        private PageTypeID pageTypeID;
        private GenCfgViewParamObjectDefInfoList cfgViewParamObjectDefInfoes;
        private readonly Dictionary<string, CfgView> cfgViewDict;
        public CompUIRunTime(PageTypeID pageTypeID, GenCfgViewParamObjectDefInfoList cfgViewParamObjectDefInfoes)
        {
            this.pageTypeID = pageTypeID;
            this.cfgViewParamObjectDefInfoes = cfgViewParamObjectDefInfoes;
            this.cfgViewDict = new Dictionary<string, CfgView>();
            foreach (var item in cfgViewParamObjectDefInfoes)
            {
                var cfgView = new CfgView(item);
                cfgView.AfterModified += doAfterModified;
                cfgView.Init(this.CallBack);
                this.cfgViewDict[item.ViewID] = cfgView;
            }
        }
        /// <summary>
        /// 所属的页面类型ID
        /// </summary>
        protected override PageTypeID _GetPageTypeID(){ return this.pageTypeID; }

        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (this.cfgViewDict.ContainsKey(viewID))
                return this.cfgViewDict[viewID];
            return null;
        }
        /// <summary>
        /// 设置数据信息
        /// </summary>
        /// <param name="data">数据信息，null表示缺省值</param>
        protected override void _SetData(byte[] data)
        {
            foreach(var cfgView in this.cfgViewDict.Values)
                cfgView.SetCfgInfo(data);
        }
        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        protected override byte[] _GetData()
        {
            if(this.cfgViewDict.Values.Count > 0)
            {
                foreach (var cfgView in this.cfgViewDict.Values)
                    return cfgView.GetCfgInfo();
            } 
            
            return [];

        }
        /// <summary>
        /// 检测数据合法性
        /// </summary>
        /// <param name="inValidMsg">不合法时的描述信息列表</param>
        /// <returns>是否合法 true:合法 false 不合法</returns>
        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = null;
            return true;
        }
        /// <summary>
        ///  执行界面命令
        ///  说明：主要用于内部子页面和组件界面插件之间的数据交互，如：标定子页面为内部子页面，它需要从所有组件界面插件得到标定项，
        ///  包括：标定项名称、对应的界面ID，然后自动产生对应的子页面。
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数（和命令ID对应的Json字符串）</param>
        /// <returns>返回结果（和命令ID对应的Json字符串）</returns>
        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            switch (cmdID)
            {
                default:
                    return null;
            }
        }
       
        private void doAfterModified(object sender, EventArgs e)
        {
            AfterDataModified?.Invoke(this, e);
        }
        #region 配置界面

        private class CfgView : IPageTypeRunTimeCompUIView
        {
            protected GenParamObjectDefInfo _GenObjectDefInfo;
            /// <summary>
            /// 配置属性值列表
            /// </summary>
            private UIPropValueListViewModel propValueListViewModel;
            private UIPropValueListView uIPropValueListView;
            //private UctlTestMMCfgView uctlCompCfg;
            public CfgView(GenParamObjectDefInfo genObjectDefInfo)
            {
                this._GenObjectDefInfo = genObjectDefInfo;
                propValueListViewModel = new UIPropValueListViewModel();
                propValueListViewModel.CanEdit = false;
                propValueListViewModel.AfterPropValueChanged += onAfterPropValueChanged;
                uIPropValueListView = new UIPropValueListView();
                uIPropValueListView.DataContext = propValueListViewModel;
                var mainWindow = Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                  ? desktop.MainWindow
                  : null;
                propValueListViewModel.SetViewReference(mainWindow);

                //uctlCompCfg = new UctlTestMMCfgView();
                //uctlCompCfg.AfterParamValModified += doAfterModified;

            }

           

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="callBack">通过命令方式的机械模组配置接口回调接口</param>
            public void Init(ICompUIRunTimeCallBack callBack)
            {
                //uctlCompCfg.Init(_GenObjectDefInfo);
            }
            /// <summary>
            /// 编辑查看参数界面，应该从Control继承
            /// </summary>
            object IPageTypeRunTimeCompUIView.View
            {
                get
                {
                    //uctlCompCfg.AfterParamValModified -= doAfterModified;
                    //uctlCompCfg = new UctlTestMMCfgView();
                    //uctlCompCfg.AfterParamValModified += doAfterModified;
                    //uctlCompCfg.Init(_GenObjectDefInfo);

                    uIPropValueListView = new UIPropValueListView();
                    uIPropValueListView.DataContext = propValueListViewModel;
                    propValueListViewModel.SetViewReference(uIPropValueListView);
                    return uIPropValueListView; 
                }
            }
            /// <summary>
            /// 编辑权限所需的操作管理单元ID列表。通过判断用户权限中是否有该操作管理单元ID决定在界面是否可以进行编辑，
            /// 只要包含其中一个就认为该用户具有该功能的操作权限，否则只有只读权限。null或个数为0表示不控制编辑权限。
            /// </summary>
            OpMngCellID[] IPageTypeRunTimeCompUIView.EditFuncMngCellIDs
            {
                get { return null; }
            }
            /// <summary>
            /// 信息修改事件
            /// </summary>
            public event EventHandler AfterModified;
            /// <summary>
            /// 界面数据对象属性值改变事件
            /// </summary>
            public event GFBaseTypeObjPropPathValueChangedEventHandler? AfterPropValueChanged;
          
            /// <summary>
            /// 带路径的属性ID的值改变
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void onAfterPropValueChanged(object sender, GFBaseTypeObjPropPathValueChangedEventArgs e)
            {
                AfterPropValueChanged?.Invoke(this, e);
            }
            /// <summary>
            ///  设置是否只读
            /// </summary>
            /// <param name="readOnly">true:只读，false:读写</param>
            void IPageTypeRunTimeCompUIView.SetReadOnly(bool readOnly)
            {
                //uctlCompCfg.ReadOnly = readOnly;
            }

            public  void SetCfgInfo(byte[] cfgInfo)
            {
                var paramValues = new GFBaseTypeParamValueList();
                paramValues.FromBytes(cfgInfo);
                
                propValueListViewModel.Init(convertToGFBaseTypePropValueList(paramValues), ConverterObj.ConvertToGFUIPropDefInfoList(_GenObjectDefInfo.ParamInfoes));

                //uctlCompCfg.CfgInfo = cfgInfo;
            }
          
            public  byte[] GetCfgInfo()
            {
                //return uctlCompCfg.CfgInfo;
                GFBaseTypePropValueList gFBaseTypePropValues=propValueListViewModel.GetUIPropValues();
                GFBaseTypeParamValueList gFBaseTypeParamValues = convertToGFBaseTypeParamValueList(gFBaseTypePropValues);
                return gFBaseTypeParamValues.ToBytes();
            }
            
            private GFBaseTypePropValueList convertToGFBaseTypePropValueList(GFBaseTypeParamValueList gFBaseTypeParamValues)
            {
                GFBaseTypePropValueList gFBaseTypePropValues = new GFBaseTypePropValueList();
                foreach (var gFBaseTypeParamValue in gFBaseTypeParamValues)
                {
                    gFBaseTypePropValues.Add(new GFBaseTypePropValue()
                    {
                        PropertyID = new MPPropertyID(gFBaseTypeParamValue.ID),
                        Value = gFBaseTypeParamValue.Value
                    });
                }
                return gFBaseTypePropValues;
            }
            private GFBaseTypeParamValueList convertToGFBaseTypeParamValueList(GFBaseTypePropValueList gFBaseTypePropValues)
            {
                GFBaseTypeParamValueList gFBaseTypeParamValues = new();
                foreach (var gFBaseTypePropValue in gFBaseTypePropValues)
                {
                    gFBaseTypeParamValues.Add(new GFBaseTypeParamValue()
                    {
                        ID = gFBaseTypePropValue.PropertyID.ToString(),
                        Value = gFBaseTypePropValue.Value
                    });
                }
                return gFBaseTypeParamValues;
            }
        }

        #endregion
    }

   internal static class ConverterObj
    {
        /// <summary>
        /// 参数信息转属性信息
        /// </summary>
        /// <param name="paramInfoes"></param>
        /// <returns></returns>
        public static GFUIPropDefInfoList ConvertToGFUIPropDefInfoList(GFParamDefInfoList paramInfoes)
        {
            GFUIPropDefInfoList uIPropDefInfoes = new GFUIPropDefInfoList();
            foreach (var paramInfo in paramInfoes)
            {
                uIPropDefInfoes.Add(new GFUIPropDefInfo()
                {
                    PropertyID = new MPPropertyID(paramInfo.ParamID),
                    PropertyName = paramInfo.ParamName,
                    DataType = paramInfo.DataType,
                    ObjectID = paramInfo.ObjectID,
                });
            }
            return uIPropDefInfoes;
        }
    }
}
