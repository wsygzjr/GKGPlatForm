using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark
{
    /// <summary>
    /// Mark配置信息 
    /// </summary>
    public class MarkConfigInfo
    {
        /// <summary>
        /// Mark基本配置信息
        /// </summary>
        public MarkBaseConfigInfo MarkBaseConfigInfo { get; set; } = new MarkBaseConfigInfo();

        /// <summary>
        /// Mark点列表
        /// </summary>
        public MarkPointInfoList MarkPointInfoes { get; set; } = new MarkPointInfoList();

        public MarkConfigInfo()
        {
            MarkBaseConfigInfo = new MarkBaseConfigInfo();
            MarkPointInfoes = new MarkPointInfoList();
        }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["MarkBaseConfigInfo"] is JObject baseCfgObj)
            {
                MarkBaseConfigInfo.FromJObject(baseCfgObj);
            }

            if (jObject["MarkPointInfoes"] is JArray markPointArray)
            {
                MarkPointInfoes.FromJObject(markPointArray);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                ["MarkBaseConfigInfo"] = MarkBaseConfigInfo.ToJObject(),
                ["MarkPointInfoes"] = MarkPointInfoes.ToJArray()
            };
        }
    }

    /// <summary>
    /// Mark基本配置信息
    /// </summary>
    public class MarkBaseConfigInfo
    {
        /// <summary>
        /// Mark方式
        /// </summary>
        public MarkMode MarkMode { get; set; }

        /// <summary>
        /// 未找到mark时自动跳过
        /// </summary>
        public bool AutoSkipWhenNotFound { get; set; } = false;

        /// <summary>
        /// 是否分离
        /// </summary>
        public bool IsSeparated { get; set; } = false;

        /// <summary>
        /// 偏差值报警
        /// </summary>
        public decimal DeviationAlarmValue { get; set; } = 0.0m;

        /// <summary>
        /// 是否为放置基准
        /// </summary>
        public bool IsPlacementReference { get; set; } = false;

        /// <summary>
        /// 备份Mark数量
        /// </summary>
        public int BackupMarkCount { get; set; } = 0;

        /// <summary>
        /// Mark防抖次数
        /// </summary>
        public int MarkdebounceCount { get; set; } = 1;

        /// <summary>
        /// Mark失败后重试次数
        /// </summary>
        public int MarkRetryAfterFailureCount { get; set; } = 1;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            MarkMode = jObject["MarkMode"] != null
                ? (MarkMode)jObject["MarkMode"].Value<int>()
                : MarkMode.PositionCompensation;
            AutoSkipWhenNotFound = jObject["AutoSkipWhenNotFound"]?.Value<bool>() ?? false;
            IsSeparated = jObject["IsSeparated"]?.Value<bool>() ?? false;
            IsPlacementReference = jObject["IsPlacementReference"]?.Value<bool>() ?? false;

            DeviationAlarmValue = jObject["DeviationAlarmValue"]?.Value<decimal>() ?? 0.0m;
            BackupMarkCount = jObject["BackupMarkCount"]?.Value<int>() ?? 0;
            MarkdebounceCount = jObject["MarkdebounceCount"]?.Value<int>() ?? 1;
            MarkRetryAfterFailureCount = jObject["MarkRetryAfterFailureCount"]?.Value<int>() ?? 1;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                ["MarkMode"] = (int)MarkMode,
                ["AutoSkipWhenNotFound"] = AutoSkipWhenNotFound,
                ["IsSeparated"] = IsSeparated,
                ["DeviationAlarmValue"] = DeviationAlarmValue,
                ["IsPlacementReference"] = IsPlacementReference,
                ["BackupMarkCount"] = BackupMarkCount,
                ["MarkdebounceCount"] = MarkdebounceCount,
                ["MarkRetryAfterFailureCount"] = MarkRetryAfterFailureCount
            };
        }
    }

   
}