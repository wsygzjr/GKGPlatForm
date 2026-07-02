using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    namespace SubMM
    {
        public class VisionSubMachineModulesConst
        {
            public const string SubMMName = "视觉分析";

            public const string SubMMModelStr = "Vision";

            public static readonly SubMMModel SubMMModel = SubMMModel.Parse(SubMMModelStr);
            public static readonly SubMMObjInfoList SubMMObjInfos = new SubMMObjInfoList
            {
                new SubMMObjInfo()
                {
                    SubMMObjID = new Guid("{6CB3CF76-8B68-4B1B-AD7B-E28C3D817244}"),
                    SubMMObjName = "通用视觉"
                },
            };
            /// <summary>
            /// 开始能力方法(异步)
            /// </summary>
            public const string SearchMarkMethodID = "SearchMark";

            /// <summary>
            /// 开始完成能力事件ID
            /// </summary>
            public static readonly string SearchMarkFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(SearchMarkMethodID);

            /// <summary>
            /// 开始能力方法(异步)
            /// </summary>
            public const string TWODDetectMethodID = "TWODDetect";

            /// <summary>
            /// 开始完成能力事件ID
            /// </summary>
            public static readonly string TWODDetectMethodIDFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(TWODDetectMethodID);

            /// <summary>
            /// 开始能力方法(异步)
            /// </summary>
            public const string ScanCodeMethodID = "ScanCode";

            /// <summary>
            /// 开始完成能力事件ID
            /// </summary>
            public static readonly string ScanCodeMethodIDFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(ScanCodeMethodID);

            /// <summary>
            /// 开始能力方法(异步)
            /// </summary>
            public const string SearchEdgeMethodID = "SearchEdge";

            /// <summary>
            /// 开始完成能力事件ID
            /// </summary>
            public static readonly string SearchEdgeMethodIDFinishedEventID = ImeCompMethodDefInfo.GetFinishedEventID(SearchEdgeMethodID);


            public const string CreateModelMethodID = "CreateModel";
            /// <summary>
            /// 子机械模组能力方法定义信息列表
            /// 约定能力方法产生的能力事件，用ImeCompMethodDefInfo.GetFinishedEventID()产生
            /// </summary>
            /// 

            /// <summary>
            /// 控制命令ID，获取插件名称
            /// </summary>
            public const string RunTimeCtlCmdGetPluginName = "GetPluginName";
            public const string RunTimeCtlCmdMachineMove = "MachineMove";
            public const string RunTimeCtlGetAxisInfos = "GetAxisInfos";
            public const string RunTimeCtlGetIOStateInfos = "GetIOStateInfos";
            public static readonly ImeCompMethodDefInfoList Methods = new ImeCompMethodDefInfoList()
        {
            new ImeCompMethodDefInfo(SearchMarkMethodID,"搜索Mark",new GFParamDefInfoList(),new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(TWODDetectMethodID,"2D检测",new GFParamDefInfoList(),new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(ScanCodeMethodID,"2维码扫描",new GFParamDefInfoList(),new GFParamDefInfoList(), false),
            new ImeCompMethodDefInfo(SearchEdgeMethodID,"抓边",new GFParamDefInfoList(),new GFParamDefInfoList(), false),
        };

            /// <summary>
            /// 子机械模组能力事件定义信息列表
            /// 不是和能力方法匹配的其他能力事件
            /// </summary>
            public static readonly ImeCompEventDefInfoList Events = new ImeCompEventDefInfoList()
            {
                // new ImeCompEventDefInfo(...),
            };

            public static readonly ImeCompMethodDefInfoList normalMethodDefInfos = new ImeCompMethodDefInfoList()
            {
                new ImeCompMethodDefInfo(SearchMarkMethodID, "搜索Mark", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(TWODDetectMethodID, "2D检测", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(ScanCodeMethodID, "2维码扫描", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(CreateModelMethodID, "创建模板", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
                new ImeCompMethodDefInfo(SearchEdgeMethodID, "抓边", new GFParamDefInfoList(), new GFParamDefInfoList(), false),
            };
        }
    }
}