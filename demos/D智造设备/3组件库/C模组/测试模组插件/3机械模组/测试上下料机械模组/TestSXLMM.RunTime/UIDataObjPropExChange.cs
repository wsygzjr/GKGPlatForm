using Griffins;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using Griffins.ImeIOT;

namespace TestSXLMM.RunTime
{
    public class UIDataObjPropExChange : IUIDataObjPropExChange
    {
        private TestSXLMMRunTimeMain genTestMMRunTime;

        public UIDataObjPropExChange(TestSXLMMRunTimeMain genTestMMRunTimeBase)
        {
            this.genTestMMRunTime = genTestMMRunTimeBase;
        }
        /// <summary>
        /// 在定义时获取子界面数据对象项名称信息字典
        /// </summary>
        /// <param name="objInstPropPath">界面数据对象属性路径</param>
        /// <param name="callBack">定义服务的回调</param>
        /// <returns></returns>
        Dictionary<string, string> IUIDataObjPropExChange.GetMMSubUIProObjItemNamesOfDefSvr(ObjInstPropPath objInstPropPath, IMachineModulesDefSvrCallBack callBack)
        {
            return genTestMMRunTime.GetMMSubUIProObjItemNamesOfDefSvr(objInstPropPath, callBack);
        }
        /// <summary>
        /// 子机械模组在定义时获取子界面数据对象项名称信息字典
        /// </summary>
        /// <param name="objInstPropPath">界面数据对象属性路径</param>
        /// <param name="callBack">定义服务的回调</param>
        /// <returns></returns>
        Dictionary<string, string> IUIDataObjPropExChange.GetSubMMSubUIProObjItemNamesOfDefSvr(ObjInstPropPath objInstPropPath, ISubMachineModulesDefSvrCallBack callBack)
        {
            return null;
        }
        /// <summary>
        /// 在运行时获取子界面数据对象项名称信息字典
        /// </summary>
        /// <param name="proIDPath">属性路径对象</param>
        /// <returns></returns>
        Dictionary<string, string> IUIDataObjPropExChange.GetSubUIProObjItemNamesOfRunTime(ObjInstPropPath objInstPropPath)
        {
            return genTestMMRunTime.GetSubUIProObjItemNames(objInstPropPath);
          
        }
    }
}
