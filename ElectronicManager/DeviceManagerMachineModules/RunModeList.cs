using GF_Gereric;
using Griffins;
using Newtonsoft.JsonG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.MM
{
    public class RunModeList :List<string>, IGriffinsBaseValue
    {
        public static readonly Guid Object_ID = new Guid("{137DE180-713B-488A-876B-06E272F263CA}");
        public const string Object_IDStr = "{137DE180-713B-488A-876B-06E272F263CA}";

        [JsonIgnore]
        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID()
        {
            return Object_ID;
        }

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json jsonValue = new ObjectValue_Json(Object_ID)
            {
                JsonVal = toJson()
            };
            return new GriffinsBaseValue(jsonValue);
        }

        private string toJson()
        {
            return JsonObjConvert.ToJSon(this);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if ((baseValue == null) || (baseValue.Val == null))
                return;
            if (!(baseValue.Val is ObjectValue_Json))
                throw new Exception("对象值不是RunModeList转换的");
            if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                throw new Exception("对象值不是RunModeList转换的");
            fromJson((baseValue.Val as ObjectValue_Json).JsonVal);
        }

        private void fromJson(string json)
        {
            RunModeList obj = JsonObjConvert.FromJSon<RunModeList>(json);
            if (obj != null)
            {
                this.Clear();
                this.AddRange(obj);
            }
        }
    }
}
