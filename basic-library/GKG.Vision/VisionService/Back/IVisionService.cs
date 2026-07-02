using GF_Gereric;
using GKG.ElectronicControl;

namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉服务接口定义
        /// <para>按“初始化 / 图片区域 / 预处理 / 找模板 / 坐标转换 / 检测 / 扫码”能力分组。</para>
        /// </summary>
        public interface IVisionService
        {
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
