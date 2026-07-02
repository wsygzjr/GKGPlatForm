using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit
{
    /// <summary>
    /// 点胶执行对象
    /// </summary>
    public class DispensingExecutionObject : EntityBase
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string ShowName { get; set; } = "";
        /// <summary>
        /// 执行对象指令类型(显示)
        /// </summary>
        public ExecutionCommandType CommandType { get; set; } = ExecutionCommandType.LiftNeedle;
        /// <summary>
        /// 执行对象类型
        /// </summary>
        public ExecutionObjectType ObjectType { get; set; } = ExecutionObjectType.BasicCommand;

        /// <summary>
        /// 执行对象实例（根据类型动态创建）
        /// </summary>
        public DispensingExecutionObjectBase ExecutionObject { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            base.FromJObject(jObject);
            ShowName = jObject["ShowName"] != null
               ? jObject["ShowName"].Value<string>()
               : "";

            ObjectType = jObject["ObjectType"] != null
                ? (ExecutionObjectType)jObject["ObjectType"].Value<int>()
                : ExecutionObjectType.BasicCommand;

            CommandType = jObject["CommandType"] != null
                ? (ExecutionCommandType)jObject["CommandType"].Value<int>()
                : ExecutionCommandType.LiftNeedle;

            if (jObject["ExecutionObject"] is JObject execObj)
            {
                ExecutionObject = ObjectType switch
                {
                    ExecutionObjectType.BasicCommand => new BasicDispensingCommandExecutionObject(),
                    ExecutionObjectType.TemplateInstance => new DispensingTemplateInstanceExecutionObject(),
                    _ => new DispensingExecutionObjectBase()
                };
                ExecutionObject.FromJObject(execObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public new JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["ShowName"] = ShowName;
            jObject["ObjectType"] = (int)ObjectType;
            jObject["CommandType"] = (int)CommandType;
            jObject["ExecutionObject"] = ExecutionObject?.ToJObject() ?? new JObject();
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingExecutionObject()
        {
            ExecutionObject = new BasicDispensingCommandExecutionObject();
        }
    }
    /// <summary>
    /// 执行对象类型枚举
    /// </summary>
    public enum ExecutionObjectType
    {
        /// <summary>
        /// 点胶基础指令执行对象
        /// </summary>
        BasicCommand,

        /// <summary>
        /// 点胶指令模板实例执行对象
        /// </summary>
        TemplateInstance
    }

    /// <summary>
    /// 执行对象指令类型枚举
    /// </summary>
    public enum ExecutionCommandType
    {
        /// <summary>
        /// 模板
        /// </summary>
        Template,

        /// <summary>
        /// 抬针
        /// </summary>
        LiftNeedle,

        /// <summary>
        /// 延时
        /// </summary>
        Delay,

        /// <summary>
        /// 清胶
        /// </summary>
        CleanGlue
    }

}