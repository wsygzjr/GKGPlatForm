namespace Griffins.CompUI.SL.InitCfgPage.Models
{
    public enum SensorCardType
    {
        GaoChuan = 0,
        GMCMiNi = 1,
    }

    public class SensorConfigCfgInfo
    {
        public SensorCardType CardType { get; set; } = SensorCardType.GaoChuan;

        public string Text { get; set; } = string.Empty;

        public void CopyFrom(SensorConfigCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            CardType = src.CardType;
            Text = src.Text;
        }
    }
}
