using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 通过条件配置信息
    /// </summary>
    public class PassConditionCfgInfo
    {
        /// <summary>
        /// 通过条件（脚本参数类型）
        /// </summary>
        public ScriptParamType PassCondition { get; set; } = ScriptParamType.MissingGlueDetection;

        /// <summary>
        /// 条件
        /// </summary>
        public ConditionType Condition { get; set; } = ConditionType.Equal;

        /// <summary>
        /// 数值调节方式
        /// </summary>
        public ValueAdjustModeType ValueAdjustMode { get; set; } = ValueAdjustModeType.Value;


        /// <summary>
        /// 选中的数值
        /// </summary>
        public decimal SelectedValue { get; set; } = 0;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            PassCondition = (ScriptParamType)(jObject["PassCondition"]?.Value<int>() ?? (int)ScriptParamType.MissingGlueDetection);
            Condition = (ConditionType)(jObject["Condition"]?.Value<int>() ?? (int)ConditionType.Equal);
            ValueAdjustMode = (ValueAdjustModeType)(jObject["ValueAdjustMode"]?.Value<int>() ?? (int)ValueAdjustModeType.Value);
            SelectedValue = jObject["SelectedValue"]?.Value<decimal>() ?? 0;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "PassCondition", (int)PassCondition },
                { "Condition", (int)Condition },
                { "ValueAdjustMode", (int)ValueAdjustMode },
                { "SelectedValue", SelectedValue }
            };
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyFrom(PassConditionCfgInfo source)
        {
            if (source == null) return;

            PassCondition = source.PassCondition;
            Condition = source.Condition;
            ValueAdjustMode = source.ValueAdjustMode;
            SelectedValue = source.SelectedValue;
        }
    }
}