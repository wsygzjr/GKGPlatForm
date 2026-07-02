using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command
{
    /// <summary>
    /// 指令序列配置信息
    /// </summary>
    public class CommandSequenceCfgInfo : CommandSequenceBaseCfgInfo
    {
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
            var jObject = base.ToJObject();
            jObject["CommandSequence"] = CommandSequence?.ToJObject() ?? new JObject();

            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandSequenceCfgInfo() : base()
        {
            CommandSequence = new CleanCommandSequence();
        }
    }

    /// <summary>
    /// 指令序列配置信息列表
    /// </summary>
    public class CommandSequenceCfgInfoList : List<CommandSequenceCfgInfo>
    {
        /// <summary>
        /// 从JArray反序列化
        /// </summary>
        /// <param name="jArray">JSON数组</param>
        public void FromJObject(JArray? jArray)
        {
            if (jArray == null) return;

            Clear();
            foreach (var itemObj in jArray)
            {
                if (itemObj is JObject itemJObject)
                {
                    var item = new CommandSequenceCfgInfo();
                    item.FromJObject(itemJObject);
                    Add(item);
                }
            }
        }

        /// <summary>
        /// 序列化为JArray
        /// </summary>
        /// <returns>JSON数组</returns>
        public JArray ToJArray()
        {
            var jArray = new JArray();
            foreach (var item in this)
            {
                jArray.Add(item.ToJObject());
            }
            return jArray;
        }
    }
}