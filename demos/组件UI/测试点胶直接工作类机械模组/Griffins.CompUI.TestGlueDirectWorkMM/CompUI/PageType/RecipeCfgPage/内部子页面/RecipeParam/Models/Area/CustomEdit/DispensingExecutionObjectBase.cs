using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area
{
    /// <summary>
    /// 点胶执行对象基类
    /// </summary>
    public class DispensingExecutionObjectBase
    {
        /// <summary>
        /// 执行对象实例ID
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public virtual void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            InstanceId = Guid.TryParse(jObject["InstanceId"]?.Value<string>(), out var cmdGuid) ? cmdGuid : Guid.NewGuid();
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public virtual JObject ToJObject()
        {
            return new JObject
            {
                { "InstanceId", InstanceId.ToString() },
            };
        }

        public DispensingExecutionObjectBase() { }
    }

    #region 点胶基础指令执行对象

    /// <summary>
    /// 点胶基础指令执行对象
    /// </summary>
    public class BasicDispensingCommandExecutionObject : DispensingExecutionObjectBase
    {
        /// <summary>
        /// 点胶指令类型（决定指令具体实现）
        /// </summary>
        public DispensingCommandType DispensingCommandType { get; set; }

        /// <summary>
        /// 指令序列基础实例（根据指令类型动态创建）
        /// </summary>
        public CommandSequenceBase CommandSequence { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            base.FromJObject(jObject);
            DispensingCommandType = jObject["DispensingCommandType"] != null
                ? (DispensingCommandType)jObject["DispensingCommandType"].Value<int>()
                : DispensingCommandType.SubDispensing;

            // 利用工厂方法创建对应指令实例并反序列化
            if (jObject["CommandSequence"] is JObject cmdObj)
            {
                CommandSequence = CommandSequenceBase.Create(DispensingCommandType);
                CommandSequence.FromJObject(cmdObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject=base.ToJObject();
            jObject["DispensingCommandType"] = (int)DispensingCommandType;
            jObject["CommandSequence"] = CommandSequence?.ToJObject() ?? new JObject();
            return jObject;
        }

        public BasicDispensingCommandExecutionObject() : base() 
        {
            CommandSequence = new CleanCommandSequence();
        }
    }

    ///// <summary>
    ///// 点胶基础指令类型枚举
    ///// </summary>
    //public enum BasicDispensingCommandType
    //{
    //    /// <summary>
    //    /// 抬针
    //    /// </summary>
    //    LiftNeedle,

    //    /// <summary>
    //    /// 延时
    //    /// </summary>
    //    Delay,

    //    /// <summary>
    //    /// 清胶
    //    /// </summary>
    //    CleanGlue
    //}
    #endregion

    #region 点胶指令模板实例执行对象

    /// <summary>
    /// 点胶指令模板实例执行对象
    /// </summary>
    public class DispensingTemplateInstanceExecutionObject : DispensingExecutionObjectBase
    {
        /// <summary>
        /// 序号（仅显示，不存储）
        /// </summary>
        public int SerialNumber { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 点胶指令模板实例
        /// </summary>
        public DispensingCommandTemplateInstance TemplateInstance { get; set; } = new DispensingCommandTemplateInstance();

        public override void FromJObject(JObject? jObject)
        {
            base.FromJObject(jObject);
            if (jObject == null) return;

            IsEnabled = jObject["IsEnabled"]?.Value<bool>() ?? true;

            if (jObject["TemplateInstance"] is JObject templateObj)
            {
                TemplateInstance = new DispensingCommandTemplateInstance();
                TemplateInstance.FromJObject(templateObj);
            }
        }

        public override JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["IsEnabled"] = IsEnabled;
            jObject["TemplateInstance"] = TemplateInstance?.ToJObject() ?? new JObject();
            return jObject;
        }

        public DispensingTemplateInstanceExecutionObject() : base() { }
    }

    /// <summary>
    /// 点胶指令模板实例类
    /// </summary>
    public class DispensingCommandTemplateInstance
    {
        /// <summary>
        /// 所属的点胶指令模板ID
        /// </summary>
        public Guid TemplateId { get; set; } 

        /// <summary>
        /// 基准坐标
        /// </summary>
        public ThreeDimensionalCoordinate BaseCoordinate { get; set; } = new ThreeDimensionalCoordinate();

        /// <summary>
        /// Mark偏移量
        /// </summary>
        public AngleCoordinate MarkOffset { get; set; } = new AngleCoordinate();

        ///// <summary>
        ///// 指令列表
        ///// </summary>
        //public List<CommandSequenceBaseCfgInfo> CommandList { get; set; } = new List<CommandSequenceBaseCfgInfo>();

        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            TemplateId = Guid.TryParse(jObject["TemplateId"]?.Value<string>(), out var cmdGuid) ? cmdGuid : Guid.NewGuid();

            if (jObject["BaseCoordinate"] is JObject baseCoordObj)
            {
                BaseCoordinate = new ThreeDimensionalCoordinate();
                BaseCoordinate.FromJObject(baseCoordObj);
            }

            if (jObject["MarkOffset"] is JObject markOffsetObj)
            {
                MarkOffset = new AngleCoordinate();
                MarkOffset.FromJObject(markOffsetObj);
            }

            //if (jObject["CommandList"] is JArray cmdArray)
            //{
            //    CommandList = new List<CommandSequenceBaseCfgInfo>();
            //    foreach (var cmdObj in cmdArray)
            //    {
            //        if (cmdObj is JObject cmdJObj)
            //        {
            //            var cmdInfo = new CommandSequenceBaseCfgInfo();
            //            cmdInfo.FromJObject(cmdJObj);
            //            CommandList.Add(cmdInfo);
            //        }
            //    }
            //}
        }

        public JObject ToJObject()
        {
            var jObject = new JObject
            {
                { "TemplateId", TemplateId.ToString() },
                { "BaseCoordinate", BaseCoordinate?.ToJObject() ?? new JObject() },
                { "MarkOffset", MarkOffset?.ToJObject() ?? new JObject() }
            };

            //if (CommandList != null && CommandList.Count > 0)
            //{
            //    var cmdArray = new JArray();
            //    foreach (var cmd in CommandList)
            //    {
            //        cmdArray.Add(cmd.ToJObject());
            //    }
            //    jObject["CommandList"] = cmdArray;
            //}
            //else
            //{
            //    jObject["CommandList"] = new JArray();
            //}

            return jObject;
        }
    }


    /// <summary>
    /// 三维坐标类
    /// </summary>
    public class ThreeDimensionalCoordinate
    {
        /// <summary>
        /// X轴坐标
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Y轴坐标
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Z轴坐标
        /// </summary>
        public decimal Z { get; set; }

        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            X = jObject["X"]?.Value<decimal>() ?? 0;
            Y = jObject["Y"]?.Value<decimal>() ?? 0;
            Z = jObject["Z"]?.Value<decimal>() ?? 0;
        }

        public JObject ToJObject()
        {
            return new JObject
            {
                { "X", X },
                { "Y", Y },
                { "Z", Z }
            };
        }
    }


    /// <summary>
    /// 三维坐标类
    /// </summary>
    public class AngleCoordinate
    {
        /// <summary>
        /// X轴坐标
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Y轴坐标
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// 角度
        /// </summary>
        public decimal Angle { get; set; }

        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            X = jObject["X"]?.Value<decimal>() ?? 0;
            Y = jObject["Y"]?.Value<decimal>() ?? 0;
            Angle = jObject["Angle"]?.Value<decimal>() ?? 0;
        }

        public JObject ToJObject()
        {
            return new JObject
            {
                { "X", X },
                { "Y", Y },
                { "Angle", Angle }
            };
        }
    }
    #endregion
}