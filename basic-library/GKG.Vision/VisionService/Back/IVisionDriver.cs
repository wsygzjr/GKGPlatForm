using GF_Gereric;
using GKG.ElectronicControl;

namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉驱动接口。
        /// 用于对接不同视觉后端，实现与业务层解耦。
        /// </summary>
        public interface IVisionDriver
        {
            /// <summary>
            /// 驱动名称。
            /// </summary>
            string DriverName { get; }

            void Init(byte[] initParam);

            void SetIOInstance(List<IBaseStateIO> ioList);

            void TriggerCCD();

            void GrabOne();

            Point2D[] GetFlyingRst();

            byte[] Preprocess(byte[] preprocessParam);

            void CreateModel(SearchMarkParams searchMarkParams);

            SearchMarkResult SearchMark(SearchMarkParams searchMarkParams);

            TwoDResultBase SearchBlob(SearchBlobParams searchBlobParams);

            byte[] SearchLine(byte[] searchLineParams);

            byte[] SearchCircle(byte[] searchCircleParams);

            byte[] SearchContour(byte[] searchContourParams);

            Point2D CalibrateFor9Pt(SearchMarkParams searchMarkParams);

            void CalibForFlying(SearchMarkParams searchMarkParams);

            Point2D CoordinateTransform(Point2D imagePoint, byte[] transformParam);

            bool Detect(byte[] detectParam);

            string ScanCode(ScanCodeParams scanCodeParams);
        }
    }
}
