using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    ///  属性信息
    /// </summary>
    internal class GenPropertyInfo : Griffins.GFParamDefInfo
    {
        /// <summary>
        /// 创建 GenPropertyInfo 新实例
        /// </summary>
        /// <param name="paramID">参数ID</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="dataType">参数数据类型</param>
        /// <param name="labelWidth">属性标签宽度，0表示缺省值</param>
        public GenPropertyInfo(string paramID, string paramName, GriffinsBaseDataType dataType, int labelWidth = 0)
            : base(paramID, paramName, dataType)
        {
            this.LabelWidth = labelWidth;
        }
        /// <summary>
        /// 属性标签宽度，0表示缺省值
        /// </summary>
        public int LabelWidth { get; set; }
    }

    /// <summary>
    ///  属性信息列表
    /// </summary>
    internal class GenPropertyInfoList : List<GenPropertyInfo>
    {
        public static GenPropertyInfoList CreateGenPropertyInfoes(GenParamObjectDefInfo genObjectDefInfo)
        {
            var genPropertyInfoes = new GenPropertyInfoList();
            foreach (var item in genObjectDefInfo.ParamInfoes)
            {
                var genPropertyInfo = new GenPropertyInfo(item.ParamID, item.ParamName, item.DataType, genObjectDefInfo.LabelWidth);
                genPropertyInfoes.Add(genPropertyInfo);
            }
            return genPropertyInfoes;
        }
    }

    /// <summary>
    /// 通用界面数据对象属性定义信息
    /// </summary>
    public class GenUIDataObjPropDefInfo
    {
        /// <summary>
        /// 属性标签宽度，0表示缺省值
        /// </summary>
        public int LabelWidth { get; set; }

        /// <summary>
        /// 界面数据对象属性定义信息列表
        /// </summary>
        public GFUIPropDefInfoList Props { get; set; }
        /// <summary>
        /// 转成智造设备组件属性定义信息列表
        /// </summary>
        /// <returns></returns>
        internal ImeCompPropDefInfoList ToImeCompPropDefInfoes()
        {
            ImeCompPropDefInfoList imeCompPropDefInfos = new ImeCompPropDefInfoList();
            if(this.Props!=null)
            {
                foreach (var item in this.Props)
                {
                    ImeCompPropDefInfo imeCompPropDefInfo = new ImeCompPropDefInfo();
                    imeCompPropDefInfo.CopyFrom(item);
                    imeCompPropDefInfo.ReadWrite = item.ReadWrite;
                    imeCompPropDefInfos.Add(imeCompPropDefInfo);
                }
            }
            return imeCompPropDefInfos;
        }

        internal GenParamObjectDefInfo ToGenParamObjectDefInfo()
        {
            var genParamObjectDefInfo = new GenParamObjectDefInfo();
            genParamObjectDefInfo.LabelWidth = LabelWidth;
            genParamObjectDefInfo.ParamInfoes = new Griffins.GFParamDefInfoList();
            foreach (var item in this.Props)
            {
                genParamObjectDefInfo.ParamInfoes.Add(new Griffins.GFParamDefInfo
                {
                    ParamID = item.PropertyID.ToString(),
                    ParamName = item.PropertyName,
                    DataType = item.DataType,
                });
            }
            return genParamObjectDefInfo;
        }
      
    }

    ///// <summary>
    ///// 界面数据对象属性定义信息
    ///// </summary>
    //public class UIImeCompPropDefInfo : GFPropDefInfo
    //{
    //    /// <summary>
    //    /// 子机械部件类型属性编辑方式
    //    /// </summary>
    //    public GFPropertyEditKind EditKind { get; set; }
    //    /// <summary>
    //    /// 属性值取值范围列表
    //    /// </summary>
    //    public GriffinsValueRange ValueRange { get; set; }
    //    /// 值含义对照表
    //    /// </summary>
    //    public GriffinsValueNamePairList ValueNamePairs { get; set; }
    //    /// <summary>
    //    /// 创建UIImeCompPropDefInfo新实例
    //    /// </summary>
    //    /// <param name="propertyID">管理点属性ID</param>
    //    /// <param name="propertyName">管理点属性名称</param>
    //    /// <param name="readWrite">读写种类</param>
    //    /// <param name="dataType">管理点属性数据类型</param>
    //    public UIImeCompPropDefInfo(string propertyID, string propertyName, GfPropReadWrite readWrite, GriffinsBaseDataType dataType)
    //        : this(propertyID, propertyName, readWrite, dataType, Guid.Empty)
    //    {

    //    }
    //    /// <summary>
    //    /// 创建UIImeCompPropDefInfo新实例
    //    /// </summary>
    //    /// <param name="propertyID">管理点属性ID</param>
    //    /// <param name="propertyName">管理点属性名称</param>
    //    /// <param name="readWrite">读写种类</param>
    //    /// <param name="dataType">管理点属性数据类型</param>
    //    /// <param name="objectID">管理点属性数据对象标识，只有DataType的类型为Object才有用,否则为Guid.Empty</param>
    //    public UIImeCompPropDefInfo(string propertyID, string propertyName, GfPropReadWrite readWrite, GriffinsBaseDataType dataType, Guid objectID)
    //        : this(MPPropertyID.Parse(propertyID), propertyName, readWrite, dataType, objectID)
    //    {
    //    }
    //    /// <summary>
    //    /// 创建UIImeCompPropDefInfo新实例
    //    /// </summary>
    //    /// <param name="propertyID">管理点属性ID</param>
    //    /// <param name="propertyName">管理点属性名称</param>
    //    /// <param name="readWrite">读写种类</param>
    //    /// <param name="dataType">管理点属性数据类型</param>
    //    public UIImeCompPropDefInfo(MPPropertyID propertyID, string propertyName, GfPropReadWrite readWrite, GriffinsBaseDataType dataType)
    //        : this(propertyID, propertyName, readWrite, dataType, Guid.Empty)
    //    {
    //    }
    //    /// <summary>
    //    /// 创建UIImeCompPropDefInfo新实例
    //    /// </summary>
    //    /// <param name="propertyID">管理点属性ID</param>
    //    /// <param name="propertyName">管理点属性名称</param>
    //    /// <param name="readWrite">读写种类</param>
    //    /// <param name="dataType">管理点属性数据类型</param>
    //    /// <param name="objectID">管理点属性数据对象标识，只有DataType的类型为Object才有用,否则为Guid.Empty</param>
    //    public UIImeCompPropDefInfo(MPPropertyID propertyID, string propertyName, GfPropReadWrite readWrite, GriffinsBaseDataType dataType, Guid objectID)
    //        : base(propertyID, propertyName, readWrite, dataType, objectID)
    //    {
    //    }
    //}
    ///// <summary>
    ///// 界面数据对象属性定义信息列表
    ///// </summary>
    //[Serializable]
    //public class UIImeCompPropDefInfoList : List<UIImeCompPropDefInfo>
    //{
    //    /// <summary>
    //    ///  查找能力属性ID对应的界面数据对象属性定义信息
    //    /// </summary>
    //    /// <param name="propertyID">属性ID</param>
    //    /// <returns>界面数据对象属性定义信息</returns>
    //    public UIImeCompPropDefInfo Find(MPPropertyID propertyID)
    //    {
    //        foreach (UIImeCompPropDefInfo e in this)
    //        {
    //            if (e.PropertyID == propertyID)
    //                return e;
    //        }
    //        return null;
    //    }
    //    /// <summary>
    //    /// 转成智造设备组件属性定义信息列表
    //    /// </summary>
    //    /// <returns></returns>
    //    internal ImeCompPropDefInfoList ToImeCompPropDefInfoes()
    //    {
    //        ImeCompPropDefInfoList imeCompPropDefInfos = new ImeCompPropDefInfoList();
    //        foreach (var item in this)
    //        {
    //            //imeCompPropDefInfos.Add(item);
    //        }
    //        return imeCompPropDefInfos;
    //    }
    //}
    /// <summary>
    ///  通用参数对象定义信息
    /// </summary>
    public class GenParamObjectDefInfo
    {
        /// <summary>
        /// 属性标签宽度，0表示缺省值
        /// </summary>
        public int LabelWidth { get; set; }
        /// <summary>
        /// 参数信息列表
        /// </summary>
        public Griffins.GFParamDefInfoList ParamInfoes { get; set; }

        internal GenPropertyInfoList ToGenPropertyInfoes()
        {
            var genPropertyInfoes = new GenPropertyInfoList();
            foreach (var item in this.ParamInfoes)
            {
                var genPropertyInfo = new GenPropertyInfo(item.ParamID, item.ParamName, item.DataType, this.LabelWidth);
                genPropertyInfoes.Add(genPropertyInfo);
            }
            return genPropertyInfoes;
        }
    }

    /// <summary>
    /// 通用配置界面参数对象定义信息
    /// </summary>
    public class GenCfgViewParamObjectDefInfo : GenParamObjectDefInfo
    {
        /// <summary>
        /// 界面ID
        /// </summary>
        public string ViewID { get; set; }
        /// <summary>
        /// 界面名称
        /// </summary>
        public string ViewName { get; set; }
    }

    /// <summary>
    /// 通用配置界面参数对象定义信息列表
    /// </summary>
    public class GenCfgViewParamObjectDefInfoList : List<GenCfgViewParamObjectDefInfo>
    {
        public GenCfgViewParamObjectDefInfo Find(string viewID)
        {
            foreach (var item in this)
            {
                if (string.Compare(item.ViewID, viewID, true) == 0)
                    return item;
            }
            return null;
        }
    }

    /// <summary>
    /// 能力事件定义信息
    /// </summary>
    [Serializable]
    public class GenCabilityEventDefInfo
    {
        /// <summary>
        /// 创建 GenCabilityEventDefInfo 新实例
        /// </summary>
        public GenCabilityEventDefInfo()
        {
        }
        /// <summary>
        /// 创建 GenCabilityEventDefInfo 新实例
        /// </summary>
        /// <param name="eventID">事件ID</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="paramObjDefInfo">参数对象定义信息</param>
        /// <param name="memo">备注</param>
        public GenCabilityEventDefInfo(string eventID, string eventName, GenParamObjectDefInfo paramObjDefInfo=null, string memo = null)
        {
            this.EventID = eventID;
            this.EventName = eventName;
            this.ParamObjDefInfo = paramObjDefInfo;
            this.Memo = memo;
        }
        /// <summary>
        ///  事件ID
        /// </summary>
        public string EventID { get; set; }
        /// <summary>
        ///  事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        ///  参数对象定义信息
        /// </summary>
        public GenParamObjectDefInfo ParamObjDefInfo { get; set; }
        ///<summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 获取事件参数属性列表
        /// </summary>
        /// <returns>事件参数属性列表</returns>
        public Griffins.GFParamDefInfoList GetEventParamObjPropes()
        {
            if (this.ParamObjDefInfo == null)
                return null;
            return this.ParamObjDefInfo.ParamInfoes;
        }

        /// <summary>
        /// 转换为ImeCompEventDefInfo
        /// </summary>
        /// <returns></returns>
        public ImeCompEventDefInfo ToImeCabilityEventDefInfo()
        {
            var imeCabilityEventDefInfo = new ImeCompEventDefInfo
            {
                EventID = this.EventID,
                EventName = this.EventName,
                Memo = this.Memo,
                EventParamDefInfoes = this.GetEventParamObjPropes(),
            };
            return imeCabilityEventDefInfo;
        }
    }

    /// <summary>
    /// 能力事件定义信息列表
    /// </summary>
    [Serializable]
    public class GenCabilityEventDefInfoList : List<GenCabilityEventDefInfo>
    {
        /// <summary>
        ///  查找能力事件ID对应的普通事件定义信息
        /// </summary>
        /// <param name="eventID">能力事件ID</param>
        /// <returns>能力事件ID对应的普通事件定义信息</returns>
        public GenCabilityEventDefInfo Find(string eventID)
        {
            foreach (var item in this)
            {
                if (string.Compare(item.EventID, eventID, true) == 0)
                    return item;
            }
            return null;
        }
        /// <summary>
        ///  查找能力事件ID对应的普通事件定义信息索引值
        /// </summary>
        /// <param name="eventID">能力事件ID</param>
        /// <returns>能力事件ID对应的普通事件定义信息索引值</returns>
        public int Indexof(string eventID)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (string.Compare(this[i].EventID, eventID, true) == 0)
                    return i;
            }
            return -1;
        }

        /// <summary>
        ///  转换为 ImeCompEventDefInfoList
        /// </summary>
        /// <returns></returns>
        public ImeCompEventDefInfoList ToImeCabilityEventDefInfoList()
        {
            ImeCompEventDefInfoList imeCabilityEventDefInfoes = new ImeCompEventDefInfoList();
            foreach (GenCabilityEventDefInfo item in this)
            {
                imeCabilityEventDefInfoes.Add(item.ToImeCabilityEventDefInfo());
            }
            return imeCabilityEventDefInfoes;
        }
    }

    /// <summary>
    /// 普通事件定义信息
    /// </summary>
    [Serializable]
    public class GenNormalEventDefInfo
    {
        /// <summary>
        /// 创建 GenNormalEventDefInfo 新实例
        /// </summary>
        public GenNormalEventDefInfo()
        {
        }
        /// <summary>
        /// 创建 GenNormalEventDefInfo 新实例
        /// </summary>
        /// <param name="eventKind">事件种类</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="paramObjDefInfo">参数对象定义信息</param>
        /// <param name="memo">备注</param>
        public GenNormalEventDefInfo(int eventKind, string eventName, GenParamObjectDefInfo paramObjDefInfo, string memo = null)
        {
            this.EventKind = eventKind;
            this.EventName = eventName;
            this.ParamObjDefInfo = paramObjDefInfo;
            this.Memo = memo;
        }
        /// <summary>
        ///  事件种类
        /// </summary>
        public int EventKind { get; set; }
        /// <summary>
        ///  事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        ///  参数对象定义信息
        /// </summary>
        public GenParamObjectDefInfo ParamObjDefInfo { get; set; }
        ///<summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 获取事件参数属性列表
        /// </summary>
        /// <returns>事件参数属性列表</returns>
        public Griffins.GFParamDefInfoList GetEventParamObjPropes()
        {
            if (this.ParamObjDefInfo == null)
                return null;
            return this.ParamObjDefInfo.ParamInfoes;
        }
    }

    /// <summary>
    /// 普通事件定义信息
    /// </summary>
    [Serializable]
    public class GenNormalEventDefInfoList : List<GenNormalEventDefInfo>
    {
        /// <summary>
        ///  查找普通事件ID对应的普通事件定义信息
        /// </summary>
        /// <param name="eventID">普通事件ID</param>
        /// <returns>普通事件ID对应的普通事件定义信息</returns>
        public GenNormalEventDefInfo Find(int eventID)
        {
            foreach (var item in this)
            {
                if (item.EventKind == eventID)
                    return item;
            }
            return null;
        }
        /// <summary>
        ///  查找普通事件ID对应的普通事件定义信息索引值
        /// </summary>
        /// <param name="eventID">普通事件ID</param>
        /// <returns>普通事件ID对应的普通事件定义信息索引值</returns>
        public int Indexof(int eventID)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].EventKind == eventID)
                    return i;
            }
            return -1;
        }
    }

    /// <summary>
    /// 方法定义信息
    /// </summary>
    [Serializable]
    public class GenMethodDefInfo
    {
        /// <summary>
        /// 创建 GenMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="isAsyn">是否异步执行，true:异步，false：同步</param>
        /// <param name="retValDefInfoes">方法返回值参数定义信息列表</param>
        /// <param name="paramConvertDele">方法参数转换为方法返回值委托类型</param>
        public GenMethodDefInfo(string methodID, string methodName, int delyTime, Griffins.GFParamDefInfoList paramDefInfoes,
            bool isAsyn = false, Griffins.GFParamDefInfoList retValDefInfoes = null, ConvertInputParamToOutputParam paramConvertDele = null)
        {
            this.MethodID = methodID;
            this.MethodName = methodName;
            this.DelyTime = delyTime;
            this.ParamDefInfoes = paramDefInfoes;
            this.IsAsyn = isAsyn;
            this.RetValDefInfoes = retValDefInfoes;
            this.ParamConvertDele = paramConvertDele;
        }

        /// <summary>
        ///  方法ID
        /// </summary>
        public string MethodID { get; set; }
        /// <summary>
        ///  方法名称
        /// </summary>
        public string MethodName { get; set; }

        ///<summary>
        /// 执行延迟时间（毫秒）
        /// </summary>
        public int DelyTime { get; set; }
        ///<summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        ///<summary>
        /// 是否异步执行，true:异步，false：同步
        /// </summary>
        public bool IsAsyn { get; set; }

        ///<summary>
        /// 方法参数转换为方法执行完成事件参数委托类型
        /// </summary>
        public ConvertInputParamToOutputParam ParamConvertDele { get; set; }

        ///<summary>
        /// 当前执行延迟时间（毫秒）,用于执行过程中
        /// </summary>
        public int CurDelyTime { get; set; }

        /// <summary>
        ///  方法参数定义信息列表
        /// </summary>
        public Griffins.GFParamDefInfoList ParamDefInfoes { get; set; }

        /// <summary>
        ///  方法返回值参数定义信息列表
        /// </summary>
        public Griffins.GFParamDefInfoList RetValDefInfoes { get; set; }

        /// <summary>
        /// 转换为ImeCompMethodDefInfo
        /// </summary>
        /// <returns></returns>
        public ImeCompMethodDefInfo ToImeCabilityMethodDefInfo()
        {
            var imeCabilityMethodDefInfo = new ImeCompMethodDefInfo
            {
                MethodID = this.MethodID,
                MethodName = this.MethodName,
                Memo = this.Memo,
                ParamDefInfoes = this.ParamDefInfoes,
                RetValDefInfoes = this.RetValDefInfoes,
            };

            return imeCabilityMethodDefInfo;
        }
    }

    /// <summary>
    /// 方法定义信息列表
    /// </summary>
    [Serializable]
    public class GenMethodDefInfoList : List<GenMethodDefInfo>
    {
        /// <summary>
        ///  查找方法ID对应的方法定义信息
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的方法定义信息</returns>
        public GenMethodDefInfo Find(string methodID)
        {
            foreach (GenMethodDefInfo item in this)
            {
                if (item.MethodID == methodID)
                    return item;
            }
            return null;
        }
        /// <summary>
        ///  查找方法ID对应的方法定义信息索引值
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的方法定义信息索引值</returns>
        public int Indexof(string methodID)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].MethodID == methodID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        ///  转换为 ImeCompMethodDefInfoList
        /// </summary>
        /// <returns></returns>
        public ImeCompMethodDefInfoList ToImeCabilityMethodDefInfoList()
        {
            ImeCompMethodDefInfoList imeCabilityMethodDefInfoes = new ImeCompMethodDefInfoList();
            foreach (GenMethodDefInfo item in this)
            {
                imeCabilityMethodDefInfoes.Add(item.ToImeCabilityMethodDefInfo());
            }
            return imeCabilityMethodDefInfoes;
        }
    }

    /// <summary>
    ///  输入参数转换为输出参数委托类型
    /// </summary>
    /// <param name="inputParam"> 输入参数</param>
    /// <returns>输出参数</returns>
    public delegate GFBaseTypeParamValueList ConvertInputParamToOutputParam(GFBaseTypeParamValueList inputParam);

    /// <summary>
    /// 普通方法定义信息
    /// </summary>
    [Serializable]
    public class GenNormalMethodDefInfo : GenMethodDefInfo
    {
        /// <summary>
        /// 创建 GenNormalMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="isAsyn">是否异步执行，true:异步，false：同步</param>
        public GenNormalMethodDefInfo(string methodID, string methodName, int delyTime,
            Griffins.GFParamDefInfoList paramDefInfoes, bool isAsyn = false)
            : base(methodID, methodName, delyTime, paramDefInfoes, isAsyn, null, null)
        {
        }
        /// <summary>
        /// 创建 GenNormalMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="isAsyn">是否异步执行，true:异步，false：同步</param>
        public GenNormalMethodDefInfo(string methodID, string methodName, int delyTime,
            Griffins.GFParamDefInfoList paramDefInfoes, bool isAsyn,
            Griffins.GFParamDefInfoList retValDefInfoes,
             ConvertInputParamToOutputParam paramConvertDele)
            : base(methodID, methodName, delyTime, paramDefInfoes, isAsyn, retValDefInfoes, paramConvertDele)
        {
        }
    }

    /// <summary>
    /// 普通方法定义信息列表
    /// </summary>
    [Serializable]
    public class GenNormalMethodDefInfoList : List<GenNormalMethodDefInfo>
    {
        /// <summary>
        ///  查找方法ID对应的普通方法定义信息
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的普通方法定义信息</returns>
        public GenNormalMethodDefInfo Find(string methodID)
        {
            foreach (var item in this)
            {
                if (item.MethodID == methodID)
                    return item;
            }
            return null;
        }
        /// <summary>
        ///  查找方法ID对应的普通方法定义信息索引值
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的普通方法定义信息索引值</returns>
        public int Indexof(string methodID)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].MethodID == methodID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        ///  转换为 GenMethodDefInfoList
        /// </summary>
        /// <returns></returns>
        public GenMethodDefInfoList ToGenMethodDefInfoList()
        {
            GenMethodDefInfoList genMethodDefInfoList = new GenMethodDefInfoList();
            foreach (var item in this)
            {
                genMethodDefInfoList.Add(item);
            }
            return genMethodDefInfoList;
        }
        /// <summary>
        ///  转换为 ImeCompMethodDefInfoList
        /// </summary>
        /// <returns></returns>
        public ImeCompMethodDefInfoList ToImeCabilityMethodDefInfoList()
        {
            ImeCompMethodDefInfoList imeCabilityMethodDefInfoes = new ImeCompMethodDefInfoList();
            foreach (var item in this)
            {
                imeCabilityMethodDefInfoes.Add(item.ToImeCabilityMethodDefInfo());
            }
            return imeCabilityMethodDefInfoes;
        }
    }

    /// <summary>
    /// 能力方法定义信息
    /// </summary>
    [Serializable]
    public class GenCabilityMethodDefInfo : GenMethodDefInfo
    {
        /// <summary>
        /// 创建 GenCabilityMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="isAsyn">是否异步执行，true:异步，false：同步</param>
        /// <param name="finishedEventParamType">对应的异步方法执行完成事件参数类型</param>
        /// <param name="paramConvertDele">方法参数转换为方法执行完成事件参数委托类型</param>
        public GenCabilityMethodDefInfo(string methodID, string methodName, int delyTime, Griffins.GFParamDefInfoList paramDefInfoes,
              Griffins.GFParamDefInfoList retValDefInfoes = null, ConvertInputParamToOutputParam paramConvertDele = null)
             : base(methodID, methodName, delyTime, paramDefInfoes, true, retValDefInfoes, paramConvertDele)
        {
        }
    }

    /// <summary>
    /// 能力方法定义信息列表
    /// </summary>
    [Serializable]
    public class GenCabilityMethodDefInfoList : List<GenCabilityMethodDefInfo>
    {
        /// <summary>
        ///  转换为 GenMethodDefInfoList
        /// </summary>
        /// <returns></returns>
        public GenMethodDefInfoList ToGenMethodDefInfoList()
        {
            GenMethodDefInfoList genMethodDefInfoList = new GenMethodDefInfoList();
            foreach (var item in this)
            {
                genMethodDefInfoList.Add(item);
            }
            return genMethodDefInfoList;
        }
        /// <summary>
        ///  转换为 ImeCompMethodDefInfoList
        /// </summary>
        /// <returns></returns>
        public ImeCompMethodDefInfoList ToImeCabilityMethodDefInfoList()
        {
            ImeCompMethodDefInfoList imeCabilityMethodDefInfoes = new ImeCompMethodDefInfoList();
            foreach (var item in this)
            {
                imeCabilityMethodDefInfoes.Add(item.ToImeCabilityMethodDefInfo());
            }
            return imeCabilityMethodDefInfoes;
        }
        /// <summary>
        ///  查找方法ID对应的能力方法定义信息
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的能力方法定义信息</returns>
        public GenCabilityMethodDefInfo Find(string methodID)
        {
            foreach (var item in this)
            {
                if (item.MethodID == methodID)
                    return item;
            }
            return null;
        }
        /// <summary>
        ///  查找方法ID对应的能力方法定义信息索引值
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的能力方法定义信息索引值</returns>
        public int Indexof(string methodID)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].MethodID == methodID)
                    return i;
            }
            return -1;
        }
    }

    /// <summary>
	///  界面数据对象属性值改变事件类型
	/// </summary>
	public delegate void UIDataObjPropValChangedEventHandler(object sender, UIDataObjPropValChangedEventArgs e);

    ///<summary>
    /// 界面数据对象属性值改变事件参数类型
    /// </summary>
    public class UIDataObjPropValChangedEventArgs : EventArgs
    {
        /// <summary>
        ///  UIDataObjPropValChangedEventArgs
        /// </summary>
        /// <param name="propVals">界面数据对象属性值</param>
        public UIDataObjPropValChangedEventArgs(GFBaseTypeObjPropPathValue propVal)
        {
            this.PropVal = propVal;
        }
        /// <summary>
        ///  界面数据对象属性值
        /// </summary>
        public GFBaseTypeObjPropPathValue PropVal { get; }
    }

    /// <summary>
    /// 控制面板定义信息
    /// 控制面板的描述信息:控制面板抬头、按钮列表（按钮名称、执行的命令）、窗体大小
    /// </summary>
    [Serializable]
    public class ControlPanelDefInfo
    {
        /// <summary>
        ///  控制面板ID
        /// </summary>
        public string ControlPanelID { get; set; }
        /// <summary>
        ///  控制面板名称
        /// </summary>
        public string ControlPanelName { get; set; }
        ///<summary>
        /// 控制面板尺寸
        /// 为空则使用默认尺寸
        /// </summary>
        public ControlPanelWinSize ControlPanelWinSize { get; set; }

        /// <summary>
        /// 控制面板执行的命令(按钮)列表
        /// </summary>
        public GFMethodDefInfoList Commands { get; set; }
        public ControlPanelDefInfo()
        {

        }
        /// <summary>
        /// 创建 GenCabilityMethodDefInfo 新实例
        /// </summary>
        /// <param name="controlPanelID">控制面板ID</param>
        /// <param name="controlPanelName">控制面板名称</param>
        /// <param name="controlPanelWinSize">控制面板尺寸</param>
        /// <param name="commands">控制面板执行的命令列表</param>
        public ControlPanelDefInfo(
            string controlPanelID, 
            string controlPanelName, 
            ControlPanelWinSize controlPanelWinSize,
            GFMethodDefInfoList commands)
        {
            this.ControlPanelID = controlPanelID;
            this.ControlPanelName = controlPanelName;
            this.ControlPanelWinSize = controlPanelWinSize;
            this.Commands = commands;
        }
    }
    /// <summary>
    /// 控制面板定义信息列表
    /// </summary>
    [Serializable]
    public class ControlPanelDefInfoList : List<ControlPanelDefInfo>
    {
    }

    /// <summary>
    /// 控制面板尺寸
    /// </summary>
    [Serializable]
    public class ControlPanelWinSize
    {
        ///<summary>
        ///控制面板窗口的宽度
        /// </summary>
        public int Width { get; set; }
        ///<summary>
        ///控制面板窗口的高度
        /// </summary>
        public int Height { get; set; }
    }
}
