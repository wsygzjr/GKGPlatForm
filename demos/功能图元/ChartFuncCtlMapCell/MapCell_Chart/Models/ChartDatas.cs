using Griffins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GKG.Map.ChartFuncCtlMapCell.Models
{

    public class ChartData
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
    }

    /// <summary>
    /// 用于与底层 Griffins 框架交互的 ChartDataDto 包装类
    /// </summary>
    public class ChartDataDto : IGriffinsBaseValue
    {
        public static readonly Guid Object_ID = new Guid("{D7C2F8A1-4B9E-4125-A3E5-8C9B0F1D3E4A}");

        public List<ChartData> Data { get; set; } = new List<ChartData>();
        public double CPK { get; set; }
        public double CPU { get; set; }
        public double CPL { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Mean { get; set; }
        public double UpperLimit { get; set; }
        public double LowerLimit { get; set; }

        #region IGriffinsBaseValue 成员

        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID()
        {
            return Object_ID;
        }

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID)
            {
                JsonVal = toJson()
            };
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json jsonVal))
                {
                    throw new Exception("对象值不是 ObjectValue_Json 类型");
                }

                if (jsonVal.Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是 ChartDataDto 转换的");
                }

                fromJson(jsonVal.JsonVal);
            }
        }

        #endregion

        #region 序列化与反序列化

        private string toJson()
        {
            return JsonSerializer.Serialize(this);
        }

        private void fromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            try
            {
                var parsedData = JsonSerializer.Deserialize<ChartDataDto>(json);
                if (parsedData != null)
                {
                    this.Data = parsedData.Data ?? new List<ChartData>();
                    this.CPK = parsedData.CPK;
                    this.CPU = parsedData.CPU;
                    this.CPL = parsedData.CPL;
                    this.Max = parsedData.Max;
                    this.Min = parsedData.Min;
                    this.Mean = parsedData.Mean;
                    this.UpperLimit = parsedData.UpperLimit;
                    this.LowerLimit = parsedData.LowerLimit;
                }
            }
            catch (JsonException ex)
            {
                // 遇到被破坏的 JSON 数据不报错，维持当前界面的默认值
                System.Diagnostics.Debug.WriteLine($"解析 ChartDataDto JSON 失败: {ex.Message}");
            }
        }

        #endregion
    }
}