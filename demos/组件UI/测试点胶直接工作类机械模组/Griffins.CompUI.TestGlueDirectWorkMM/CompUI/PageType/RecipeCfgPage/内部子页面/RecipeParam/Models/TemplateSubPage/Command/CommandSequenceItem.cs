using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command
{
    /// <summary>
    /// 指令序列基础信息
    /// </summary>
    public class CommandSequenceBaseCfgInfo : EntityBase
    {
        /// <summary>
        /// 指令ID（唯一标识）
        /// </summary>
        public Guid CommandID { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 描述信息（指令说明）
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// 点胶指令类型（决定指令具体实现）
        /// </summary>
        public DispensingCommandType DispensingCommandType { get; set; }


        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            base.FromJObject(jObject);
            CommandID = Guid.TryParse(jObject["CommandID"]?.Value<string>(), out var cmdGuid) ? cmdGuid : Guid.NewGuid();
            Enable = jObject["Enable"]?.Value<bool>() ?? true;
            Description = jObject["Description"]?.Value<string>() ?? "";
            DispensingCommandType = jObject["DispensingCommandType"] != null
                ? (DispensingCommandType)jObject["DispensingCommandType"].Value<int>()
                : DispensingCommandType.SubDispensing;

          
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public new JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["CommandID"] = CommandID.ToString();
            jObject["Enable"] = Enable;
            jObject["Description"] = Description;
            jObject["DispensingCommandType"] = (int)DispensingCommandType;
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandSequenceBaseCfgInfo()
        {
        }
    }
}