
 using GF_Gereric;
 using Griffins;
 using Griffins.Map;
 
 namespace GKG.Map.UIDataObj.StatusInfo
 {
     /// <summary>
     /// 状态信息功能图元对应的界面数据对象
     /// 用于在前端/后端之间以统一的属性列表形式交换数据
     /// </summary>
     public class UIDataObjStatusInfo : GFPropObjBase
     {
         /// <summary>
         /// 是否双阀模式
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool IsDualValve { get; set; }
 
         /// <summary>
         /// 左阀定时称重状态
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool LeftValveGlueMonitorState { get; set; }
 
         /// <summary>
         /// 左阀定时定量称重状态
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool LeftValveQuantitativeGlueMonitorState { get; set; }
 
         /// <summary>
         /// 左阀余量监控状态
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool LeftValveRemainingMonitorState { get; set; }
 
         /// <summary>
         /// 左压电阀总次数报警状态
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool LeftPressureCyclesAlarmState { get; set; }
 
         /// <summary>
         /// 右阀定时称重状态
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool RightValveGlueMonitorState { get; set; }
 
         /// <summary>
         /// 右阀定时定量称重状态
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool RightValveQuantitativeGlueMonitorState { get; set; }
 
         /// <summary>
         /// 右阀余量监控状态
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool RightValveRemainingMonitorState { get; set; }
 
         /// <summary>
         /// 右压电阀总次数报警状态
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public bool RightPressureCyclesAlarmState { get; set; }
 
         /// <summary>
         /// A 轨待补加热时间（m:s）
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public string? AWaitingAddGlueTime { get; set; }
 
         /// <summary>
         /// B 轨待补加热时间（m:s）
         /// </summary>
         [GFProp(GfPropReadWrite.ReadWrite)]
         public string? BWaitingAddGlueTime { get; set; }
     }
 }
