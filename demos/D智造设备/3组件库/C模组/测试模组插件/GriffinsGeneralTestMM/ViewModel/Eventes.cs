using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI;
using Griffins.UI.General;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using static GriffinsGeneralTestMM.UctlCabilityEventesViewModel; 

namespace GriffinsGeneralTestMM
{
    // 普通事件控件的ViewModel
    public class Eventes : ReactiveObject
    {  
        #region 私有方法 
         
        /// <summary>
        /// 将eventParamDefInfoes 转换为 List<PropertyValueInfo>
        /// </summary>
        /// <param name="eventParamDefInfoes"></param>
        /// <returns></returns>
        public static  List<PropertyValueInfo> ConvertGFParamDefInfoListToPropertyValueInfoList(GFParamDefInfoList eventParamDefInfoes)
        {
            List<PropertyValueInfo> gropertyValueInfoList = new List<PropertyValueInfo>();
             
            foreach (GFParamDefInfo paramDefInfo in eventParamDefInfoes)
            {
                PropertyValueInfo gfBaseTypeParamValue = new PropertyValueInfo();
                gfBaseTypeParamValue.ObjPropID = paramDefInfo.ParamID;
                gfBaseTypeParamValue.ObjPropName = paramDefInfo.ParamName;
                object val = PropertyValueInfo.GetDefaultValueByDataType(paramDefInfo.DataType, paramDefInfo.ObjectID);
                gfBaseTypeParamValue.GriffinsBaseValue = new GriffinsBaseValue(val);
                gropertyValueInfoList.Add(gfBaseTypeParamValue);
            }
            return gropertyValueInfoList;
        } 
         
        #endregion
          
    }
}