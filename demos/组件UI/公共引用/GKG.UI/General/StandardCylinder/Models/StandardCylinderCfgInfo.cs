using Newtonsoft.JsonG.Linq;
using System;

namespace GKG.UI.General
{
    /// <summary>
    /// 标准气缸配置模型
    /// </summary>
    public class StandardCylinderCfgInfo
    {
        /// <summary>
        /// 气缸页面类型行配置（固定区）
        /// </summary>
        public CylinderPageTypeRowCfgInfo PageTypeRow { get; set; } = new();

        /// <summary>
        /// 当前模式
        /// </summary>
        public StandardCylinderMode Mode { get; set; } = StandardCylinderMode.SingleControlSingleLimit;

        /// <summary>
        /// 单控单限位配置
        /// </summary>
        public SingleControlSingleLimitCfgInfo SingleControlSingleLimit { get; set; } = new();

        /// <summary>
        /// 单控双限位配置
        /// </summary>
        public SingleControlDoubleLimitCfgInfo SingleControlDoubleLimit { get; set; } = new();

        /// <summary>
        /// 双控双限位配置
        /// </summary>
        public DoubleControlDoubleLimitCfgInfo DoubleControlDoubleLimit { get; set; } = new();

        /// <summary>
        /// 双控单限位配置
        /// </summary>
        public DoubleControlSingleLimitCfgInfo DoubleControlSingleLimit { get; set; } = new();

        private static void ReadHorizontalModelFromJObject(HorizontalControlCardStateInitCfgInfo target, JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            target.ControlCardID = jObject["ControlCardID"]?.Value<string>() ?? target.ControlCardID;
            target.IOChannel = jObject["IOChannel"] != null
                ? (IOChannelType)jObject["IOChannel"]!.Value<int>()
                : target.IOChannel;
        }

        private static JObject WriteHorizontalModelToJObject(HorizontalControlCardStateInitCfgInfo? model)
        {
            if (model == null)
            {
                return new JObject();
            }

            return new JObject
            {
                { "ControlCardID", model.ControlCardID },
                { "IOChannel", (int)model.IOChannel },
            };
        }

        /// <summary>
        /// 从另一个实例复制数据
        /// </summary>
        public void CopyFrom(StandardCylinderCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            PageTypeRow.CopyFrom(src.PageTypeRow);
            Mode = src.Mode;
            SingleControlSingleLimit.CopyFrom(src.SingleControlSingleLimit);
            SingleControlDoubleLimit.CopyFrom(src.SingleControlDoubleLimit);
            DoubleControlDoubleLimit.CopyFrom(src.DoubleControlDoubleLimit);
            DoubleControlSingleLimit.CopyFrom(src.DoubleControlSingleLimit);
        }

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            if (jObject["Mode"] != null)
            {
                var rawMode = jObject["Mode"]!.Value<int>();
                Mode = Enum.IsDefined(typeof(StandardCylinderMode), rawMode)
                    ? (StandardCylinderMode)rawMode
                    : Mode;
            }

            if (jObject["PageTypeRow"] is JObject ptr)
            {
                PageTypeRow.FromJObject(ptr);
            }

            if (jObject["SingleControlSingleLimit"] is JObject scslObj)
            {
                var controlObj = scslObj["ControlModel"] as JObject;
                var limitObj = scslObj["LimitModel"] as JObject;
                SingleControlSingleLimit.ControlModel ??= new();
                SingleControlSingleLimit.LimitModel ??= new();
                ReadHorizontalModelFromJObject(SingleControlSingleLimit.ControlModel, controlObj);
                ReadHorizontalModelFromJObject(SingleControlSingleLimit.LimitModel, limitObj);
            }

            if (jObject["SingleControlDoubleLimit"] is JObject scdlObj)
            {
                var controlObj = scdlObj["ControlModel"] as JObject;
                var firstObj = scdlObj["FirstLimitModel"] as JObject;
                var secondObj = scdlObj["SecondLimitModel"] as JObject;
                SingleControlDoubleLimit.ControlModel ??= new();
                SingleControlDoubleLimit.FirstLimitModel ??= new();
                SingleControlDoubleLimit.SecondLimitModel ??= new();
                ReadHorizontalModelFromJObject(SingleControlDoubleLimit.ControlModel, controlObj);
                ReadHorizontalModelFromJObject(SingleControlDoubleLimit.FirstLimitModel, firstObj);
                ReadHorizontalModelFromJObject(SingleControlDoubleLimit.SecondLimitModel, secondObj);
            }

            if (jObject["DoubleControlSingleLimit"] is JObject dcslObj)
            {
                var firstControlObj = dcslObj["FirstControlModel"] as JObject;
                var secondControlObj = dcslObj["SecondControlModel"] as JObject;
                var limitObj = dcslObj["LimitModel"] as JObject;
                DoubleControlSingleLimit.FirstControlModel ??= new();
                DoubleControlSingleLimit.SecondControlModel ??= new();
                DoubleControlSingleLimit.LimitModel ??= new();
                ReadHorizontalModelFromJObject(DoubleControlSingleLimit.FirstControlModel, firstControlObj);
                ReadHorizontalModelFromJObject(DoubleControlSingleLimit.SecondControlModel, secondControlObj);
                ReadHorizontalModelFromJObject(DoubleControlSingleLimit.LimitModel, limitObj);
            }

            if (jObject["DoubleControlDoubleLimit"] is JObject dcdlObj)
            {
                var firstControlObj = dcdlObj["FirstControlModel"] as JObject;
                var secondControlObj = dcdlObj["SecondControlModel"] as JObject;
                var firstLimitObj = dcdlObj["FirstLimitModel"] as JObject;
                var secondLimitObj = dcdlObj["SecondLimitModel"] as JObject;
                DoubleControlDoubleLimit.FirstControlModel ??= new();
                DoubleControlDoubleLimit.SecondControlModel ??= new();
                DoubleControlDoubleLimit.FirstLimitModel ??= new();
                DoubleControlDoubleLimit.SecondLimitModel ??= new();
                ReadHorizontalModelFromJObject(DoubleControlDoubleLimit.FirstControlModel, firstControlObj);
                ReadHorizontalModelFromJObject(DoubleControlDoubleLimit.SecondControlModel, secondControlObj);
                ReadHorizontalModelFromJObject(DoubleControlDoubleLimit.FirstLimitModel, firstLimitObj);
                ReadHorizontalModelFromJObject(DoubleControlDoubleLimit.SecondLimitModel, secondLimitObj);
            }
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "PageTypeRow", PageTypeRow.ToJObject() },
                { "Mode", (int)Mode },
                {
                    "SingleControlSingleLimit",
                    new JObject
                    {
                        { "ControlModel", WriteHorizontalModelToJObject(SingleControlSingleLimit.ControlModel) },
                        { "LimitModel", WriteHorizontalModelToJObject(SingleControlSingleLimit.LimitModel) },
                    }
                },
                {
                    "SingleControlDoubleLimit",
                    new JObject
                    {
                        { "ControlModel", WriteHorizontalModelToJObject(SingleControlDoubleLimit.ControlModel) },
                        { "FirstLimitModel", WriteHorizontalModelToJObject(SingleControlDoubleLimit.FirstLimitModel) },
                        { "SecondLimitModel", WriteHorizontalModelToJObject(SingleControlDoubleLimit.SecondLimitModel) },
                    }
                },
                {
                    "DoubleControlSingleLimit",
                    new JObject
                    {
                        { "FirstControlModel", WriteHorizontalModelToJObject(DoubleControlSingleLimit.FirstControlModel) },
                        { "SecondControlModel", WriteHorizontalModelToJObject(DoubleControlSingleLimit.SecondControlModel) },
                        { "LimitModel", WriteHorizontalModelToJObject(DoubleControlSingleLimit.LimitModel) },
                    }
                },
                {
                    "DoubleControlDoubleLimit",
                    new JObject
                    {
                        { "FirstControlModel", WriteHorizontalModelToJObject(DoubleControlDoubleLimit.FirstControlModel) },
                        { "SecondControlModel", WriteHorizontalModelToJObject(DoubleControlDoubleLimit.SecondControlModel) },
                        { "FirstLimitModel", WriteHorizontalModelToJObject(DoubleControlDoubleLimit.FirstLimitModel) },
                        { "SecondLimitModel", WriteHorizontalModelToJObject(DoubleControlDoubleLimit.SecondLimitModel) },
                    }
                }
            };
        }
    }
}
