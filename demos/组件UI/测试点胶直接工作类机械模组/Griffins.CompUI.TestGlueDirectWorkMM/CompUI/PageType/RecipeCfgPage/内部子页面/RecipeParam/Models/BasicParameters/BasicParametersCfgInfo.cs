using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;
using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.BasicParameters
{
    /// <summary>
    /// 基本参数配置信息
    /// </summary>
    public class BasicParametersCfgInfo
    {
        /// <summary>
        /// 切换配方时是否弹窗向导校准页面
        /// </summary>
        public bool IsShowCalibrationPageWhenSwitchingFormulas { get; set; }

        /// <summary>
        /// 暂停后恢复是否排胶称重
        /// </summary>
        public bool IsDischargeGlueAndWeighWhenResumePause { get; set; }

        /// <summary>
        /// 启用拐角平滑
        /// </summary>
        public bool EnableCornerSmoothing { get; set; }

        /// <summary>
        /// 暂停后回待机位
        /// </summary>
        public bool IsReturnToStandbyPositionAfterPause { get; set; }

        /// <summary>
        /// 定时提醒校准时间间隔（单位：小时?）
        /// </summary>
        public int CalibrationReminderInterval { get; set; }

        /// <summary>
        /// 胶量报警后继续点胶板数
        /// </summary>
        public int ContinueDispensingBoardsAfterGlueAlarm { get; set; }

        /// <summary>
        /// 拐角平滑
        /// </summary>
        public decimal CornerSmoothingCoefficient { get; set; }

        /// <summary>
        /// 点胶头待机位置
        /// </summary>
        public BasePositionInfo GlueDspensingPositionInfo { get; set; }
       


        public BasicParametersCfgInfo()
        {
            GlueDspensingPositionInfo = new BasePositionInfo();
        }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 布尔类型
            IsShowCalibrationPageWhenSwitchingFormulas = jObject["IsShowCalibrationPageWhenSwitchingFormulas"]?.Value<bool>() ?? false;
            IsDischargeGlueAndWeighWhenResumePause = jObject["IsDischargeGlueAndWeighWhenResumePause"]?.Value<bool>() ?? false;
            EnableCornerSmoothing = jObject["EnableCornerSmoothing"]?.Value<bool>() ?? false;
            IsReturnToStandbyPositionAfterPause = jObject["IsReturnToStandbyPositionAfterPause"]?.Value<bool>() ?? false;

            // 整数类型
            CalibrationReminderInterval = jObject["CalibrationReminderInterval"]?.Value<int>() ?? 0;
            ContinueDispensingBoardsAfterGlueAlarm = jObject["ContinueDispensingBoardsAfterGlueAlarm"]?.Value<int>() ?? 0;

            // 小数类型
            CornerSmoothingCoefficient = jObject["CornerSmoothingCoefficient"]?.Value<decimal>() ?? 0;

            // 嵌套对象：点胶头待机位置
            if (jObject["GlueDspensingPositionInfo"] is JObject posObj)
            {
                GlueDspensingPositionInfo ??= new BasePositionInfo();
                GlueDspensingPositionInfo.FromJObject(posObj);
            }

        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject();

            // 布尔类型
            jObject["IsShowCalibrationPageWhenSwitchingFormulas"] = IsShowCalibrationPageWhenSwitchingFormulas;
            jObject["IsDischargeGlueAndWeighWhenResumePause"] = IsDischargeGlueAndWeighWhenResumePause;
            jObject["EnableCornerSmoothing"] = EnableCornerSmoothing;
            jObject["IsReturnToStandbyPositionAfterPause"] = IsReturnToStandbyPositionAfterPause;

            // 整数类型
            jObject["CalibrationReminderInterval"] = CalibrationReminderInterval;
            jObject["ContinueDispensingBoardsAfterGlueAlarm"] = ContinueDispensingBoardsAfterGlueAlarm;

            // 小数类型
            jObject["CornerSmoothingCoefficient"] = CornerSmoothingCoefficient;

            // 嵌套对象：点胶头待机位置
            jObject["GlueDspensingPositionInfo"] = GlueDspensingPositionInfo?.ToJObject() ?? new JObject();

            return jObject;
        }
    }
}
