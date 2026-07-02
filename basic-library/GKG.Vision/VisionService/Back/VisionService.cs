using GF_Gereric;
using GKG.ElectronicControl;

namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉服务。
        /// 作为业务层入口，对外保持统一接口，内部委托给具体视觉驱动。
        /// </summary>
        public class VisionService : IVisionService
        {
            private IVisionDriver? driver;
            private VisionInitParameters initParameters = new VisionInitParameters();
            private List<IBaseStateIO>? ioInstances;

            #region 初始化

            public void Init(byte[] initParam)
            {
                VisionPluginManager.Init();
                initParameters = JsonObjConvert.FromJSonBytes<VisionInitParameters>(initParam) ?? new VisionInitParameters();
                driver = VisionPluginManager.GetVisionDriver(initParameters.DriverName);
                driver.Init(initParam);

                if (ioInstances != null)
                {
                    driver.SetIOInstance(ioInstances);
                }
            }

            public void SetIOInstance(List<IBaseStateIO> ioList)
            {
                ioInstances = ioList;
                EnsureDriver().SetIOInstance(ioList);
            }

            #endregion

            #region 图片区域 / 拍照

            public void TriggerCCD()
            {
                EnsureDriver().TriggerCCD();
            }

            public void GrabOne()
            {
                EnsureDriver().GrabOne();
            }

            public Point2D[] GetFlyingRst()
            {
                return EnsureDriver().GetFlyingRst();
            }

            #endregion

            #region 预处理

            public byte[] Preprocess(byte[] preprocessParam)
            {
                return EnsureDriver().Preprocess(preprocessParam);
            }

            #endregion

            #region 找模板

            public void CreateModel(SearchMarkParams searchMarkParams)
            {
                EnsureDriver().CreateModel(searchMarkParams);
            }

            public SearchMarkResult SearchMark(SearchMarkParams searchMarkParams)
            {
                return EnsureDriver().SearchMark(searchMarkParams);
            }

            public TwoDResultBase SearchBlob(SearchBlobParams searchBlobParams)
            {
                return EnsureDriver().SearchBlob(searchBlobParams);
            }

            public byte[] SearchLine(byte[] searchLineParams)
            {
                return EnsureDriver().SearchLine(searchLineParams);
            }

            public byte[] SearchCircle(byte[] searchCircleParams)
            {
                return EnsureDriver().SearchCircle(searchCircleParams);
            }

            public byte[] SearchContour(byte[] searchContourParams)
            {
                return EnsureDriver().SearchContour(searchContourParams);
            }

            #endregion

            #region 坐标转换

            public Point2D CalibrateFor9Pt(SearchMarkParams searchMarkParams)
            {
                return EnsureDriver().CalibrateFor9Pt(searchMarkParams);
            }

            public void CalibForFlying(SearchMarkParams searchMarkParams)
            {
                EnsureDriver().CalibForFlying(searchMarkParams);
            }

            public Point2D CoordinateTransform(Point2D imagePoint, byte[] transformParam)
            {
                return EnsureDriver().CoordinateTransform(imagePoint, transformParam);
            }

            #endregion

            #region 检测

            public bool Detect(byte[] detectParam)
            {
                return EnsureDriver().Detect(detectParam);
            }

            #endregion

            #region 扫码

            public string ScanCode(ScanCodeParams scanCodeParams)
            {
                return EnsureDriver().ScanCode(scanCodeParams);
            }

            #endregion

            private IVisionDriver EnsureDriver()
            {
                if (driver != null)
                {
                    return driver;
                }

                VisionPluginManager.Init();
                driver = VisionPluginManager.GetVisionDriver(initParameters.DriverName);
                driver.Init(JsonObjConvert.ToJSonBytes(initParameters));

                if (ioInstances != null)
                {
                    driver.SetIOInstance(ioInstances);
                }

                return driver;
            }
        }
    }
}
