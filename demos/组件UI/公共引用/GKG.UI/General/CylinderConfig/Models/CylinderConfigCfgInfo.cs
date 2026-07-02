namespace GKG.UI.General
{
    public enum ECylinderType
    {
        SingleActSingleLimit = 0,
        SingleActDoubleLimit = 1,
        DoubleActSingleLimit = 2,
        DoubleActDoubleLimit = 3,
    }

    public class CylinderConfigCfgInfo
    {
        public ECylinderType ConfigType { get; set; } = ECylinderType.SingleActSingleLimit;

        public SingleControlSingleLimitCfgInfo? SingleActSingleLimit { get; set; }

        public SingleControlDoubleLimitCfgInfo? SingleActDoubleLimit { get; set; }

        public DoubleControlSingleLimitCfgInfo? DoubleActSingleLimit { get; set; }

        public DoubleControlDoubleLimitCfgInfo? DoubleActDoubleLimit { get; set; }

        public void CopyFrom(CylinderConfigCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            ConfigType = src.ConfigType;
            SingleActSingleLimit = src.SingleActSingleLimit;
            SingleActDoubleLimit = src.SingleActDoubleLimit;
            DoubleActSingleLimit = src.DoubleActSingleLimit;
            DoubleActDoubleLimit = src.DoubleActDoubleLimit;
        }
    }
}
