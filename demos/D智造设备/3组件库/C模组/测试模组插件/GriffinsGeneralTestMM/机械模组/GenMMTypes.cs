using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 机械模组方法执行子机械模组方法定义信息
    /// </summary>
    public class GenMMMMethodExecDefInfo 
    {
        /// <summary>
        /// 创建 GenMMMMethodExecDefInfo 新实例
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subMethodID">子机械模组能力方法ID</param>
        /// <param name="isCability">是否为子机械模组能力方法：true:能力方法，false：普通方法</param>
        /// <param name="subParamConvertDele">机械模组方法参数转换为子机械模组方法参数委托类型</param>
        public GenMMMMethodExecDefInfo(InnerAlias innerAlias, string subMethodID,  bool isCability, ConvertInputParamToOutputParam subParamConvertDele=null)
        {
            this.InnerAlias = innerAlias;
            this.SubMethodID = subMethodID;
            this.IsCability = isCability;
            this.SubParamConvertDele = subParamConvertDele;
        }

        /// <summary>
        /// 子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）
        /// </summary>
        public InnerAlias InnerAlias { get; set; }
        ///<summary>
        /// 子机械模组方法ID
        /// </summary>
        public string SubMethodID { get; set; }
        ///<summary>
        /// 是否为子机械模组能力方法：true:能力方法，false：普通方法
        /// </summary>
        public bool IsCability { get; set; }
        ///<summary>
        /// 机械模组方法参数转换为子机械模组方法参数委托类型
        /// </summary>
        public ConvertInputParamToOutputParam SubParamConvertDele { get; set; }
    }

    /// <summary>
    /// 机械模组方法执行子机械模组方法定义信息列表
    /// </summary>
    public class GenMMMMethodExecDefInfoList:List<GenMMMMethodExecDefInfo>
    {
    }

    /// <summary>
    /// 机械模组方法定义信息
    /// </summary>
    public class GenMMMMethodDefInfo: GenMethodDefInfo
    {
        /// <summary>
        /// 创建 GenMMMMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="isAsyn">是否异步执行，true:异步，false：同步</param>
        /// <param name="execDefInfoes">机械模组方法执行子机械模组方法定义信息列表</param>
        public GenMMMMethodDefInfo(string methodID, string methodName, int delyTime, GFParamDefInfoList paramDefInfoes, bool isAsyn, GenMMMMethodExecDefInfoList execDefInfoes = null)
           : this(methodID, methodName, delyTime, paramDefInfoes, isAsyn, null, null, execDefInfoes)
        {
        }

        /// <summary>
        /// 创建 GenMMMMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="retValDefInfoes">方法返回值参数定义信息列表</param>
        /// <param name="paramConvertDele">方法参数转换为方法执行完成事件参数委托类型</param>
        /// <param name="execDefInfoes">机械模组方法执行子机械模组方法定义信息列表</param>
        public GenMMMMethodDefInfo(string methodID, string methodName, int delyTime, GFParamDefInfoList paramDefInfoes, GFParamDefInfoList retValDefInfoes, ConvertInputParamToOutputParam paramConvertDele,
            GenMMMMethodExecDefInfoList execDefInfoes = null)
            : this(methodID, methodName, delyTime, paramDefInfoes, true, retValDefInfoes, paramConvertDele, execDefInfoes)
        {
        }

        /// <summary>
        /// 创建 GenMMMMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="isAsyn">是否异步执行，true:异步，false：同步</param>
        /// <param name="retValDefInfoes">方法返回值参数定义信息列表</param>
        /// <param name="paramConvertDele">方法参数转换为方法执行完成事件参数委托类型</param>
        /// <param name="execDefInfoes">机械模组方法执行子机械模组方法定义信息列表</param>
        public GenMMMMethodDefInfo(string methodID, string methodName, int delyTime, GFParamDefInfoList paramDefInfoes, bool isAsyn, GFParamDefInfoList retValDefInfoes, ConvertInputParamToOutputParam paramConvertDele,
            GenMMMMethodExecDefInfoList execDefInfoes = null)
            : base(methodID, methodName, delyTime, paramDefInfoes, isAsyn, retValDefInfoes, paramConvertDele)
        {
            this.ExecDefInfoes = execDefInfoes;
        }
        /// <summary>
        /// 机械模组方法执行子机械模组方法定义信息列表
        /// </summary>
        public GenMMMMethodExecDefInfoList ExecDefInfoes { get; set; }
    }

    /// <summary>
    /// 机械模组方法定义信息列表
    /// </summary>
    [Serializable]
    public class GenMMMMethodDefInfoList : List<GenMMMMethodDefInfo>
    {
        /// <summary>
        ///  查找方法ID对应的机械模组方法定义信息
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的机械模组方法定义信息</returns>
        public GenMMMMethodDefInfo Find(string methodID)
        {
            foreach (var item in this)
            {
                if (item.MethodID == methodID)
                    return item;
            }
            return null;
        }
        /// <summary>
        ///  查找方法ID对应的机械模组方法定义信息索引值
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的机械模组方法定义信息索引值</returns>
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
            foreach (var item in this)
            {
                imeCabilityMethodDefInfoes.Add(item.ToImeCabilityMethodDefInfo());
            }
            return imeCabilityMethodDefInfoes;
        }
    }

    /// <summary>
    /// 机械模组普通方法定义信息
    /// </summary>
    [Serializable]
    public class GenMMNormalMethodDefInfo : GenMMMMethodDefInfo
    {
        /// <summary>
        /// 创建 GenMMNormalMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="isAsyn">是否异步执行，true:异步，false：同步</param>
        public GenMMNormalMethodDefInfo(string methodID, string methodName, int delyTime, GFParamDefInfoList paramDefInfoes, bool isAsyn = false)
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
        public GenMMNormalMethodDefInfo(string methodID, string methodName, int delyTime,
            Griffins.GFParamDefInfoList paramDefInfoes, bool isAsyn,
            Griffins.GFParamDefInfoList retValDefInfoes,
             ConvertInputParamToOutputParam paramConvertDele)
            : base(methodID, methodName, delyTime, paramDefInfoes, isAsyn, retValDefInfoes, paramConvertDele)
        {
        }
    }

    /// <summary>
    /// 机械模组普通方法定义信息列表
    /// </summary>
    [Serializable]
    public class GenMMNormalMethodDefInfoList : List<GenMMNormalMethodDefInfo>
    {
        /// <summary>
        ///  查找方法ID对应的机械模组普通方法定义信息
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的机械模组普通方法定义信息</returns>
        public GenMMNormalMethodDefInfo Find(string methodID)
        {
            foreach (var item in this)
            {
                if (item.MethodID == methodID)
                    return item;
            }
            return null;
        }
        /// <summary>
        ///  查找方法ID对应的机械模组普通方法定义信息索引值
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的机械模组普通方法定义信息索引值</returns>
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
            foreach (var item in this)
            {
                imeCabilityMethodDefInfoes.Add(item.ToImeCabilityMethodDefInfo());
            }
            return imeCabilityMethodDefInfoes;
        }
    }

    /// <summary>
    /// 机械模组能力方法定义信息
    /// </summary>
    [Serializable]
    public class GenMMCabilityMethodDefInfo : GenMMMMethodDefInfo
    {
        /// <summary>
        /// 创建 GenMMCabilityMethodDefInfo 新实例
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="delyTime">执行延迟时间（毫秒）</param>
        /// <param name="paramDefInfoes">方法参数定义信息列表</param>
        /// <param name="retValDefInfoes">方法返回值参数定义信息列表</param>
        /// <param name="paramConvertDele">方法参数转换为方法执行完成事件参数委托类型</param>
        /// <param name="execDefInfoes">机械模组方法执行子机械模组方法定义信息列表</param>
        public GenMMCabilityMethodDefInfo(string methodID, string methodName, int delyTime, GFParamDefInfoList paramDefInfoes, GFParamDefInfoList retValDefInfoes, ConvertInputParamToOutputParam paramConvertDele,
            GenMMMMethodExecDefInfoList execDefInfoes = null)
            : base(methodID, methodName, delyTime, paramDefInfoes, true, retValDefInfoes, paramConvertDele, execDefInfoes)
        {
        }
    }

    /// <summary>
    /// 机械模组能力方法定义信息列表
    /// </summary>
    [Serializable]
    public class GenMMCabilityMethodDefInfoList : List<GenMMCabilityMethodDefInfo>
    {
        /// <summary>
        ///  查找方法ID对应的机械模组能力方法定义信息
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的机械模组能力方法定义信息</returns>
        public GenMMCabilityMethodDefInfo Find(string methodID)
        {
            foreach (var item in this)
            {
                if (item.MethodID == methodID)
                    return item;
            }
            return null;
        }
        /// <summary>
        ///  查找方法ID对应的机械模组能力方法定义信息索引值
        /// </summary>
        /// <param name="methodID">方法ID</param>
        /// <returns>方法ID对应的机械模组能力方法定义信息索引值</returns>
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
            foreach (var item in this)
            {
                imeCabilityMethodDefInfoes.Add(item.ToImeCabilityMethodDefInfo());
            }
            return imeCabilityMethodDefInfoes;
        }
    }


    /// <summary>
    /// 执行接收到所属的子机械模组事件定义信息
    /// </summary>
    [Serializable]
    public class GenExecSubEventDefInfo
    {
        /// <summary>
        /// 创建 GenExecSubMMEventDefInfo 新实例
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subEventID">子机械模组事件ID</param>
        /// <param name="mmEventID">触发的机械模组能力事件ID</param>
        /// <param name="paramConvertDele">子机械模组参数转换为机械模组参数委托类型</param>
        public GenExecSubEventDefInfo(InnerAlias innerAlias, int subEventID, string mmEventID,ConvertInputParamToOutputParam paramConvertDele)
        {
            this.InnerAlias = innerAlias;
            this.SubEventID = subEventID.ToString();
            this.IsSubCabilityEvent = false;
            this.IsImeCabilityEvent = true;
            this.MMEventID = mmEventID;
            this.ParamConvertDele = paramConvertDele;
        }
        /// <summary>
        /// 创建 GenExecSubMMEventDefInfo 新实例
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subEventID">子机械模组事件ID</param>
        /// <param name="mmEventID">触发的机械模组普通事件ID</param>
        /// <param name="paramConvertDele">子机械模组参数转换为机械模组参数委托类型</param>
        public GenExecSubEventDefInfo(InnerAlias innerAlias, int subEventID, int mmEventID, ConvertInputParamToOutputParam paramConvertDele)
        {
            this.InnerAlias = innerAlias;
            this.SubEventID = subEventID.ToString();
            this.IsSubCabilityEvent = false;
            this.IsImeCabilityEvent = false;
            this.MMEventID = mmEventID.ToString();
            this.ParamConvertDele = paramConvertDele;
        }

        /// <summary>
        /// 创建 GenExecSubMMEventDefInfo 新实例
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subEventID">子机械模组事件ID</param>
        /// <param name="mmEventID">触发的机械模组能力事件ID</param>
        /// <param name="paramConvertDele">子机械模组参数转换为机械模组参数委托类型</param>
        public GenExecSubEventDefInfo(InnerAlias innerAlias, string subEventID,string mmEventID, ConvertInputParamToOutputParam paramConvertDele)
        {
            this.InnerAlias = innerAlias;
            this.SubEventID = subEventID;
            this.IsSubCabilityEvent = true;
            this.IsImeCabilityEvent = true;
            this.MMEventID = mmEventID;
            this.ParamConvertDele = paramConvertDele;
        }
        /// <summary>
        /// 创建 GenExecSubMMEventDefInfo 新实例
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subEventID">子机械模组事件ID</param>
        /// <param name="mmEventID">触发的机械模组能力事件ID</param>
        /// <param name="paramConvertDele">子机械模组参数转换为机械模组参数委托类型</param>
        public GenExecSubEventDefInfo(InnerAlias innerAlias, string subEventID, int mmEventID, ConvertInputParamToOutputParam paramConvertDele)
        {
            this.InnerAlias = innerAlias;
            this.SubEventID = subEventID;
            this.IsSubCabilityEvent = true;
            this.IsImeCabilityEvent = false;
            this.MMEventID = mmEventID.ToString();
            this.ParamConvertDele = paramConvertDele;
        }

        /// <summary>
        /// 子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）
        /// </summary>
        public InnerAlias InnerAlias { get; set; }
        ///<summary>
        /// 子机械模组事件ID
        /// </summary>
        public string SubEventID { get; set; }
        ///<summary>
        /// 是否为子机械模组能力事件
        /// </summary>
        public bool IsSubCabilityEvent { get; set; }
        ///<summary>
        /// 是否为机械模组能力事件
        /// </summary>
        public bool IsImeCabilityEvent { get; set; }
        ///<summary>
        /// 触发的机械模组能力(普通)事件ID（如果是普通事件，该值为普通事件ID的字符串表示）
        /// </summary>
        public string MMEventID { get; set; }
        ///<summary>
        /// 子机械模组参数转换为机械模组参数委托类型
        /// </summary>
        public ConvertInputParamToOutputParam ParamConvertDele { get; set; }
    }
    /// <summary>
    /// 执行接收到所属的子机械模组事件定义信息列表
    /// </summary>
    [Serializable]
    public class GenExecSubEventDefInfoList : List<GenExecSubEventDefInfo>
    {
        /// <summary>
        ///  查找子机械模组事件对应的执行接收到所属的子机械模组事件定义信息
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subEventID">子机械模组事件ID</param>
        /// <param name="isCabilityEvent">是否为子机械模组能力事件</param>
        /// <returns>方法ID对应的复合机械模组普通方法</returns>
        public GenExecSubEventDefInfo Find(InnerAlias innerAlias, string subEventID, bool isCabilityEvent)
        {
            foreach (GenExecSubEventDefInfo item in this)
            {
                if ((item.InnerAlias == innerAlias) &&
                    (item.SubEventID == subEventID) &&
                    (item.IsSubCabilityEvent == isCabilityEvent))
                    return item;
            }
            return null;
        }
    }
         

    /// <summary>
    ///  机械模组信息
    /// </summary>
    public class GenMMInfo
    {
        /// <summary>
        /// 机械模组名称
        /// </summary>
        public string MMName { get; set; }
        /// <summary>
        ///  机械模组包含的子机械模组能力数据列表
        /// </summary>
        public CompContainSubMMDataList SubMMs { get; set; }
        /// <summary>
        ///  机械模组的机械模组能力事件列表
        /// </summary>
        public GenCabilityEventDefInfoList CabilityEvents { get; set; }
        /// <summary>
        ///  机械模组的机械模组能力方法列表
        /// </summary>
        public GenMMCabilityMethodDefInfoList CabilityMethods { get; set; }
        /// <summary>
        ///  机械模组界面数据对象属性定义信息列表
        /// </summary>
        public GenUIDataObjPropDefInfo UIDataObjProps { get; set; }
        /// <summary>
        /// 机械模组界面数据对象命令定义信息列表
        /// </summary>
        public ImeCompMethodDefInfoList UICommands { get; set; }
        /// <summary>
        ///  机械模组的机械模组普通事件列表
        /// </summary>
        public GenNormalEventDefInfoList NormalEvents { get; set; }
        /// <summary>
        ///  机械模组的机械模组普通方法列表
        /// </summary>
        public GenMMNormalMethodDefInfoList NormalMethods { get; set; }
        /// <summary>
        /// 执行接收到所属的子机械模组事件定义信息列表
        /// </summary>
        public GenExecSubEventDefInfoList ExecSubEventDefInfoes { get; set; }

        /// <summary>
        ///  控制面板定义信息列表
        /// </summary>
        public ControlPanelDefInfoList ControlPanels { get; set; }


		/// <summary>
		///  调整当前执行延迟时间
		/// </summary>
		/// <param name="execPercent">执行延迟时间百分比</param>
		public void AdjustCurDelyTime(int execPercent)
        {
            if (this.CabilityMethods != null)
            {
                foreach (var genMethodDefInfo in this.CabilityMethods)
                {
                    decimal curDelyTime = ((decimal)genMethodDefInfo.DelyTime * execPercent) / 100;
                    genMethodDefInfo.CurDelyTime = Convert.ToInt32(curDelyTime);
                }
            }
            if (this.NormalMethods != null)
            {
                foreach (var genMethodDefInfo in this.NormalMethods)
                {
                    decimal curDelyTime = ((decimal)genMethodDefInfo.DelyTime * execPercent) / 100;
                    genMethodDefInfo.CurDelyTime = Convert.ToInt32(curDelyTime);
                }
            }
        }

        /// <summary>
        /// 查找子机械模组普通事件对应的执行接收到所属的子机械模组事件定义信息
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subEventID">子机械模组事件ID</param>
        /// <returns>方法ID对应的复合机械模组普通方法</returns>
        public GenExecSubEventDefInfo FindGenExecSubEventDefInfo(InnerAlias innerAlias, int subEventID)
        {
            return findGenExecSubEventDefInfo(innerAlias, subEventID.ToString(), false);
        }

        /// <summary>
        /// 查找子机械模组能力事件对应的执行接收到所属的子机械模组事件定义信息
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subEventID">子机械模组事件ID</param>
        /// <returns>方法ID对应的复合机械模组普通方法</returns>
        public GenExecSubEventDefInfo FindGenExecSubEventDefInfo(InnerAlias innerAlias, string subEventID)
        {
            return findGenExecSubEventDefInfo(innerAlias, subEventID, true);
        }

        /// <summary>
        ///  查找方法ID对应的复合机械模组普通方法
        /// </summary>
        /// <param name="innerAlias">子机械模组的内部别名（在机械模组内唯一标识一个子机械模组）</param>
        /// <param name="subEventID">子机械模组事件ID</param>
        /// <param name="isSubCabilityEvent">是否为子机械模组能力事件</param>
        /// <returns>方法ID对应的复合机械模组普通方法</returns>
        private GenExecSubEventDefInfo findGenExecSubEventDefInfo(InnerAlias innerAlias, string subEventID, bool isSubCabilityEvent)
        {
            if (this.ExecSubEventDefInfoes == null)
                return null;
            return this.ExecSubEventDefInfoes.Find(innerAlias, subEventID, isSubCabilityEvent);
        }

        /// <summary>
        /// 查找指定的能力方法ID对应的方法定义信息
        /// </summary>
        /// <param name="methodID">能力方法ID</param>
        /// <returns>指定的能力方法ID对应的方法定义信息</returns>
        public GenMMCabilityMethodDefInfo FindCabilityMethodDefInfo(string methodID)
        {
            if (this.CabilityMethods == null)
                return null;
            return this.CabilityMethods.Find(methodID);
        }

        /// <summary>
        /// 查找指定的普通方法ID对应的方法定义信息
        /// </summary>
        /// <param name="methodID">普通方法ID</param>
        /// <returns>指定的普通方法ID对应的方法定义信息</returns>
        public GenMMNormalMethodDefInfo FindNormalMethodDefInfo(string methodID)
        {
            if (this.NormalMethods == null)
                return null;
            return this.NormalMethods.Find(methodID);
        }

        /// <summary>
        ///  获取 ImeCompMethodDefInfoList
        /// </summary>
        /// <returns></returns>
        public ImeCompMethodDefInfoList GetImeCabilityMethodDefInfoList()
        {
            if (this.CabilityMethods == null)
                return new ImeCompMethodDefInfoList();
            return this.CabilityMethods.ToImeCabilityMethodDefInfoList();
        }

        /// <summary>
        ///  获取 ImeCompEventDefInfoList
        /// </summary>
        /// <returns></returns>
        public ImeCompEventDefInfoList GetImeCabilityEventDefInfoList()
        {
            if (this.CabilityEvents == null)
                return new ImeCompEventDefInfoList();
            return this.CabilityEvents.ToImeCabilityEventDefInfoList();
        }
        /// <summary>
        /// 界面数据对象属性交互接口
        /// </summary>
        public IUIDataObjPropExChange IUIDataObjPropExChange;
    }
    /// <summary>
    /// 界面数据对象属性交互接口
    /// </summary>
    public interface IUIDataObjPropExChange
    {
        /// <summary>
        /// 机械模组在定义时获取子界面数据对象项名称信息字典
        /// </summary>
        /// <param name="objInstPropPath">界面数据对象属性路径</param>
        /// <param name="callBack">定义服务的回调</param>
        /// <returns></returns>
        Dictionary<string, string> GetMMSubUIProObjItemNamesOfDefSvr(ObjInstPropPath objInstPropPath, IMachineModulesDefSvrCallBack callBack);
        /// <summary>
        /// 子机械模组在定义时获取子界面数据对象项名称信息字典
        /// </summary>
        /// <param name="objInstPropPath">界面数据对象属性路径</param>
        /// <param name="callBack">定义服务的回调</param>
        /// <returns></returns>
        Dictionary<string, string> GetSubMMSubUIProObjItemNamesOfDefSvr(ObjInstPropPath objInstPropPath, ISubMachineModulesDefSvrCallBack callBack);
        /// <summary>
        /// 在运行时获取子界面数据对象项名称信息字典
        /// </summary>
        /// <param name="objInstPropPath">界面数据对象属性路径</param>
        /// <returns></returns>
        Dictionary<string, string> GetSubUIProObjItemNamesOfRunTime(ObjInstPropPath objInstPropPath);
    }

    /// <summary>
    ///  机械模组设计时信息
    /// </summary>
    public class GenMMDesignTimeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factoryCfgObjDefInfo">出厂配置参数对象定义信息</param>
        /// <param name="initCfgObjDefInfo">初始化参数对象定义信息</param>
        /// <param name="ppCfgObjDefInfo">配方参数对象定义信息</param>
        public GenMMDesignTimeInfo(GenCfgViewParamObjectDefInfoList factoryCfgObjDefInfo, 
            GenCfgViewParamObjectDefInfoList initCfgObjDefInfo, 
            GenCfgViewParamObjectDefInfoList ppCfgObjDefInfo, GenCfgViewParamObjectDefInfoList factoryCalibrationCfgObjDefInfo=null)
        {
            this.FactoryCfgObjDefInfo = factoryCfgObjDefInfo;
            this.InitCfgObjDefInfo = initCfgObjDefInfo;
            this.PPCfgObjDefInfo = ppCfgObjDefInfo;
        }
        /// <summary>
		/// 出厂配置参数对象定义信息
		/// </summary>
		public GenCfgViewParamObjectDefInfoList FactoryCfgObjDefInfo { get; }
        /// <summary>
		/// 初始化参数对象定义信息
		/// </summary>
		public GenCfgViewParamObjectDefInfoList InitCfgObjDefInfo { get; }
        /// <summary>
		/// 配方参数对象定义信息
		/// </summary>
		public GenCfgViewParamObjectDefInfoList PPCfgObjDefInfo { get; }
    }
}
