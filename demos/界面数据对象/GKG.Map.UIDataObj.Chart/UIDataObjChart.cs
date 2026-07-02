using GF_Gereric;
using Griffins;
using Griffins.IOT;
using Griffins.Map;
using System.Text.Json;

namespace GKG.Map.UIDataObj.Chart
{
	public class UIDataObjChart: GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadOnly, "ChartDatas")]
        public ChartDatas ChartDatas { get; set; }
    }

    public class ChartDatas : IGriffinsBaseValue
    {
        public static readonly Guid Object_ID = new Guid("{BDA30552-1DC0-4DE1-A5DE-BEB6B1D96811}");

        #region 属性字段

        public string Type { get; set; }
        public DateTime Time { get; set; }
        public double Value { get; set; }
        public double Cpk { get; set; }
        public double UpperLimit { get; set; }
        public double LowerLimit { get; set; }

        #endregion

        #region IGriffinsBaseValue接口实现

        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID()
        {
            return Object_ID;
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if ((object)baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                {
                    throw new Exception("对象值不是ChartDatas转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是ChartDatas转换的");
                }

                fromJson((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = toJson();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        #endregion

        #region 序列化与反序列化


        private class SaveModel
        {
            public string Type { get; set; }
            public DateTime Time { get; set; }
            public double Value { get; set; }
            public double Cpk { get; set; }
            public double UpperLimit { get; set; }
            public double LowerLimit { get; set; }
        }

        private string toJson()
        {
            var data = new SaveModel()
            {
                Type = this.Type,
                Time = this.Time,
                Value = this.Value,
                Cpk = this.Cpk,
                UpperLimit = this.UpperLimit,
                LowerLimit = this.LowerLimit
            };

            return JsonSerializer.Serialize(data);
        }

        private void fromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            try
            {
                var data = JsonSerializer.Deserialize<SaveModel>(json);
                if (data != null)
                {
                    this.Type = data.Type;
                    this.Time = data.Time;
                    this.Value = data.Value;
                    this.Cpk = data.Cpk;
                    this.UpperLimit = data.UpperLimit;
                    this.LowerLimit = data.LowerLimit;
                }
            }
            catch (JsonException)
            {
                // 遇到被破坏的 JSON 数据不报错，维持当前界面的默认值
            }
        }


        #endregion
    }
}
