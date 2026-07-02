using GF_Gereric;
//using GKG.Station;
//using GKG.Dispense;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.Map.Cmd;
using Griffins.PF;
using System.Text.Json;
using GKG.Map.UIDataObj.ProductionInfo;

namespace GKG.Map.UIDataObjProcess.ProductionInfo
{
    [UIDataObjProcess_ObjKind("ProductionInfo")]
    internal class UIDataObjProcessProductionInfo : GriffinsPluginMngClass, IUIDataObjProcess
    {
        private IUIDataObjProcessCallBack callBack = null!;
        private UIDataObjProcessProductionInfoCfgInfo cfgInfo = null!;
        private ProductionInfoData productionInfoData = null!;

        #region IUIDataObjProcess 成员

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginFileName">插件文件名（包括所在的路径）</param>
        /// <param name="cfgData">自定义界面数据对象实例配置参数</param>
        /// <param name="iUIInterfaceCallBack">界面接口插件接口回调接口</param>
        void IUIDataObjProcess.Init(string pluginFileName, byte[] cfgData, IUIDataObjProcessCallBack iUIInterfaceCallBack)
        {
            this.callBack = iUIInterfaceCallBack;
            this.cfgInfo = new UIDataObjProcessProductionInfoCfgInfo();
            this.cfgInfo.FromBytes(cfgData);

            productionInfoData = new ProductionInfoData();

            // 配置模组实列别名
            //this.cfgInfo.DeviceManager_Alias = new MMAlias("DevMngMM1");

            //自定义界面数据对象处理插件的数据来源可以有以下几种
            //第一：模组的界面数据对象属性值改变
            //第二：模组的自定义事件
            //第三：自定义的服务插件
            //订阅机械模组界面数据对象属性值改变处理
            this.callBack.RegisterInformInfoProcessDelegate(InformInfo_MMUIDataObjPropValChangedInfo.InfoKindID, doMMUIDataObjPropValChangedInfo);
            //订阅子机械模组界面数据对象属性值改变处理
            this.callBack.RegisterInformInfoProcessDelegate(InformInfo_SubMMUIDataObjPropValChangedInfo.InfoKindID, doSubMMUIDataObjPropValChangedInfo);
            //订阅机械模组自定义事件信息处理
            this.callBack.RegisterInformInfoProcessDelegate(InformInfo_MMCustomEventInfo.InfoKindID, doReceiveMMCustomEventInfo);
            //订阅子机械模组自定义事件信息处理
            this.callBack.RegisterInformInfoProcessDelegate(InformInfo_SubMMCustomEventInfo.InfoKindID, doReceiveSubCustomMMEventInfo);
        }

        /// <summary>
        /// 获取指定实例别名的界面数据对象属性值
        /// </summary>
        /// <param name="projectID">项目ID，如果用于网关，该项无效</param>
        /// <param name="alias">界面数据对象实例别名</param>
        /// <param name="objKind">界面数据对象种类</param>
        /// <param name="category">界面数据对象类别</param>
        /// <returns>指定实例别名的界面数据对象属性值列表</returns>
        GFBaseTypeObjPropPathValueList IUIDataObjProcess.GetUIDataObjPropPathValues(string projectID, string alias, ManagingPointKind objKind, MPCategoryID category)
        {
            var objInstPropPaths = new ObjInstPropPath[]
            {
                ObjInstPropPath.Parse("Datas.CurrentRecipeName")
            };

            return ((IUIDataObjProcess)this).GetUIDataObjPropPathValues(projectID, alias, objInstPropPaths, objKind, category);
        }

        /// <summary>
        /// 获取指定实例别名列表的界面数据对象属性值
        /// </summary>
        /// <param name="projectID">项目ID，如果用于网关，该项无效</param>
        /// <param name="aliases">界面数据对象实例别名列表</param>
        /// <param name="objKind">界面数据对象种类</param>
        /// <param name="category">界面数据对象类别</param>
        /// <returns>指定实例别名列表的界面数据对象属性值列表</returns>
        UIDataObjPropPathValueList IUIDataObjProcess.GetUIDataObjPropPathValues(string projectID, string[] aliases, ManagingPointKind objKind, MPCategoryID category)
        {
            // 自定义界面数据对象一般不支持获取多个界面数据对象实例的属性值
            throw new NotSupportedException();
        }

        /// <summary>
        /// 获取指定实例别名的界面数据对象属性当前值
        /// </summary>
        /// <param name="projectID">项目ID，如果用于网关，该项无效</param>
        /// <param name="alias">界面数据对象实例别名</param>
        /// <param name="objInstPropPath">界面数据对象属性路径</param>
        /// <param name="objKind">界面数据对象种类</param>
        /// <param name="category">界面数据对象类别</param>
        /// <returns>指定实例别名的界面数据对象属性当前值</returns>
        GriffinsBaseValue IUIDataObjProcess.GetUIDataObjPropPathValue(string projectID, string alias, ObjInstPropPath objInstPropPath, ManagingPointKind objKind, MPCategoryID category)
        {
            if (objInstPropPath.ToString() == "Datas.CurrentRecipeName")
            {
                SvrImeIOTCallSvrProvider svrProvider = new SvrImeIOTCallSvrProvider();
                IImeCompUIDataObjPropValRW iImeCompUIDataObjPropValRW = svrProvider.GetImeCompUIDataObjPropValRW();

                string deviceManagerAlias = this.cfgInfo.DeviceManager_Alias.ToString();
                ObjInstPropPath objPath = ObjInstPropPath.Parse("CurFormulaNumber");

                var baseValue = iImeCompUIDataObjPropValRW.GetUIDataObjPropPathValue(
                    projectID,
                    deviceManagerAlias,
                    objPath,
                    UIDataObjProcessProductionInfoCfgInfo.DeviceManager_MPKind,
                    ImeIOTConst.UIDataObjCategory_MM);

                if (baseValue != null)
                {
                    convert(objPath, baseValue);
                }
            }

            return productionInfoData.ToGFBaseTypePropValues().ToGriffinsBaseValue();
        }

        /// <summary>
        /// 获取指定实例别名一组属性当前值
        /// </summary>
        /// <param name="projectID">项目ID，如果用于网关，该项无效</param>
        /// <param name="alias">界面数据对象实例别名</param>
        /// <param name="mPPropertyIDs">界面数据对象属性ID列表</param>
        /// <param name="objKind">界面数据对象种类</param>
        /// <param name="category">界面数据对象类别</param>
        /// <returns>指定实例别名一组属性当前值列表</returns>
        GFBaseTypeObjPropPathValueList IUIDataObjProcess.GetUIDataObjPropPathValues(string projectID, string alias, ObjInstPropPath[] objInstPropPaths, ManagingPointKind objKind, MPCategoryID category)
        {
            GFBaseTypeObjPropPathValueList gFBaseTypeObjPropPathValueList = new GFBaseTypeObjPropPathValueList();

            foreach (ObjInstPropPath objInstPropPath in objInstPropPaths)
            {
                var propValue = ((IUIDataObjProcess)this).GetUIDataObjPropPathValue(projectID, alias, objInstPropPath, objKind, category);

                if (propValue != null)
                {
                    gFBaseTypeObjPropPathValueList.Add(new GFBaseTypeObjPropPathValue(objInstPropPath, propValue));
                }
            }

            return gFBaseTypeObjPropPathValueList;
        }

        /// <summary>
        /// 设置界面数据对象属性值
        /// </summary>
        /// <param name="projectID">项目ID，如果用于网关，该项无效</param>
        /// <param name="uiDataObjPropValue">界面数据对象属性值</param>
        /// <param name="objKind">界面数据对象种类</param>
        /// <param name="category">界面数据对象类别</param>
        void IUIDataObjProcess.SetUIDataObjPropPathValue(string projectID, UIDataObjPropPathValue uIDataObjPropPathValue, ManagingPointKind objKind, MPCategoryID category)
        {
        }

        /// <summary>
        /// 设置一组界面数据对象属性值
        /// </summary>
        /// <param name="projectID">项目ID，如果用于网关，该项无效</param>
        /// <param name="uiDataObjPropValues">界面数据对象属性值列表</param>
        /// <param name="objKind">界面数据对象种类</param>
        /// <param name="category">界面数据对象类别</param>
        void IUIDataObjProcess.SetUIDataObjPropPathValues(string projectID, UIDataObjPropPathValueList uIDataObjPropPathValues, ManagingPointKind objKind, MPCategoryID category)
        {
        }

        /// <summary>
        ///  执行界面数据对象命令
        /// </summary>
        /// <param name="projectID">项目ID，如果用于网关，该项无效</param>
        /// <param name="alias">组件界面数据对象实例别名</param>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <param name="objKind">界面数据对象种类</param>
        /// <param name="category">界面数据对象类别</param>
        /// <returns>命令返回值</returns>
        GFBaseTypeParamValueList IUIDataObjProcess.ExecUIDataObjCommand(string projectID, string alias, string cmdID, GFBaseTypeParamValueList cmdParam, ManagingPointKind objKind, MPCategoryID category)
        {
            if (cmdID == "GetAllFormulaNumberes" || cmdID == "SetCurFormulaNumber")
            {
                var svrProvider = new SvrImeIOTCallSvrProvider();
                return svrProvider.GetImeCompUIDataObjPropValRW().ExecUIDataObjCommand(
                    projectID,
                    this.cfgInfo.DeviceManager_Alias.ToString(),
                    cmdID,
                    cmdParam,
                    UIDataObjProcessProductionInfoCfgInfo.DeviceManager_MPKind,
                    ImeIOTConst.UIDataObjCategory_MM);
            }
            return new GFBaseTypeParamValueList();
        }

        #endregion IUIDataObjProcess 成员

        #region 事件监听与推送

        /// <summary>
        /// 机械模组界面数据对象属性值改变信息
        /// </summary>
        /// <param name="infoKind">信息种类</param>
        /// <param name="infoNo">信息编号</param>
        /// <param name="info">信息内容对象</param>
        private void doMMUIDataObjPropValChangedInfo(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
            //把转发的数据转换回来
            if (info is InformInfo_MMUIDataObjPropValChangedInfo informInfo)
            {
                if (informInfo.MMAlias == this.cfgInfo.DeviceManager_Alias)
                {
                    if (informInfo.ObjInstPropPath == ObjInstPropPath.Parse("CurFormulaNumber"))
                    {
                        var uIDataObjPropPathValue = new UIDataObjPropPathValue(
                            this.callBack.Alias, 
                            ObjInstPropPath.Parse("Datas.CurrentRecipeName"), 
                            convert(informInfo.ObjInstPropPath, informInfo.Val)
                            );
                        this.callBack.AsynSendUIDataObjPropValueToMapTml("", uIDataObjPropPathValue);
                    }
                }
            }
        }

        /// <summary>
        /// 子机械模组界面数据对象属性值改变信息
        /// </summary>
        /// <param name="infoKind">信息种类</param>
        /// <param name="infoNo">信息编号</param>
        /// <param name="info">信息内容对象</param>
        private void doSubMMUIDataObjPropValChangedInfo(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
        }

        /// <summary>
        /// 机械模组自定义事件信息处理
        /// </summary>
        /// <param name="infoKind"></param>
        /// <param name="infoNo"></param>
        /// <param name="info"></param>
        private void doReceiveMMCustomEventInfo(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
        }

        /// <summary>
        /// 子机械模组自定义事件信息处理
        /// </summary>
        /// <param name="infoKind"></param>
        /// <param name="infoNo"></param>
        /// <param name="info"></param>
        private void doReceiveSubCustomMMEventInfo(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
        }

        #endregion

        /// <summary>
        /// 将底层各个模组零散的数据合并入聚合大对象
        /// </summary>
        private GriffinsBaseValue convert(ObjInstPropPath objInstPropPath, GriffinsBaseValue value)
        {
            if (value != null)
            {
                string pathStr = objInstPropPath.ToString();
                if (pathStr == "CurFormulaNumber") productionInfoData.CurrentRecipeName = value.ToPrimitiveValue<string>();
            }

            return productionInfoData.ToGFBaseTypePropValues().ToGriffinsBaseValue();
        }
    }

    /// <summary>
	/// 自定义界面数据对象配置信息
	/// </summary>
	internal class UIDataObjProcessProductionInfoCfgInfo
    {
        /// <summary>
        /// 设备管理界面数据对象种类
        /// </summary>
        public static readonly ManagingPointKind DeviceManager_MPKind = ManagingPointKind.Parse(ImeIOTConst.DevMngMMStr);

        /// <summary>
        /// 设备管理机械模组实列别名
        /// </summary>
        public MMAlias DeviceManager_Alias { get; set; }

        public byte[] ToBytes() => JsonObjConvert.ToJSonBytes(this);

        public void FromBytes(byte[] data)
        {
            if (data != null) JsonObjConvert.PopulateObject(data, this);
        }
    }
}
