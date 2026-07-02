using Griffins.CompUI.SL.InitCfgPage.Models;

namespace Griffins.CompUI.SL.InitCfgPage.Models
{
    public class RegularSingleTrackCompUIModel
    {
        public TrackBasicParamCompUIModel TrackBasicParamCompUIModel { get; set; } = new();

        public TransportMotorCompUIModel TransportMotorCompUIModel { get; set; } = new();

        public SingleTrackStationCompUIModel SingleTrackStationCompUIModel { get; set; } = new();

        public PcCommunicationCompUIModel PcCommunicationCompUIModel { get; set; } = new();

        public TrackWideningCompUIModel‌ TrackWideningCompUIModel { get; set; } = new();
    }
}
