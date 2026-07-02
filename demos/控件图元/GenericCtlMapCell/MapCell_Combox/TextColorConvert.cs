using Newtonsoft.JsonG.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Newtonsoft.JsonG
{
    public class TextColorConvert : CustomCreationConverter<Color>
    {
        public override Color Create(Type objectType)
        {
            return SystemColors.InfoText;
        }
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                Color color = (Color)value;
                writer.WriteValue(color.ToArgb());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            int rgb = (int)reader.Value;
            return Color.FromArgb(rgb);
        }
    }
}
