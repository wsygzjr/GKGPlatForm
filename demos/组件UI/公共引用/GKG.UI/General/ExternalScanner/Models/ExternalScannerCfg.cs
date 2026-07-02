using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 通用外接扫码器数据模型
    /// </summary>
    public class ExternalScannerCfg
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = false;

        /// <summary>
        /// 条码校验字符串
        /// </summary>
        public string BarcodeValidationString { get; set; } = "";

        /// <summary>
        /// 条码类型
        /// </summary>
        public BarcodeType BarcodeType { get; set; } = BarcodeType.QR;

        /// <summary>
        /// 扫码器类型
        /// </summary>
        public string ScannerType { get; set; } = "A";

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            Enable = jObject["Enable"]?.Value<bool>() ?? false;
            BarcodeValidationString = jObject["BarcodeValidationString"]?.Value<string>() ?? "";
            BarcodeType = jObject["BarcodeType"]?.Value<BarcodeType>() ?? BarcodeType.QR;
            ScannerType = jObject["ScannerType"]?.Value<string>() ?? "A";
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "Enable", Enable },
                { "BarcodeValidationString", BarcodeValidationString },
                { "BarcodeType", (int)BarcodeType },
                { "ScannerType", ScannerType }
            };
        }

        /// <summary>
        /// 从另一个实例复制数据
        /// </summary>
        /// <param name="src">数据源</param>
        public void CopyFrom(ExternalScannerCfg src)
        {
            if (src == null)
            {
                return;
            }

            Enable = src.Enable;
            BarcodeValidationString = src.BarcodeValidationString;
            BarcodeType = src.BarcodeType;
            ScannerType = src.ScannerType;
        }
    }
}
