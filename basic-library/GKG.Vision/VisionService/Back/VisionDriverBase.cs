using GF_Gereric;
using GKG.ElectronicControl;

namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 视觉驱动基类。
        /// 提供统一的初始化与默认未实现行为，方便扩展不同视觉后端。
        /// </summary>
        public abstract class VisionDriverBase : IVisionDriver
        {
            #region 受保护属性

            protected VisionInitParameters InitParameters { get; private set; } = new VisionInitParameters();
            protected IBaseStateIO ChangeCCDOrJet { get; private set; }
            protected IBaseStateIO TriggerCCDIO { get; private set; }

            #endregion

            #region 公有属性

            public abstract string DriverName { get; }

            #endregion

            #region 公有方法

            public virtual void Init(byte[] initParam)
            {
                InitParameters = JsonObjConvert.FromJSonBytes<VisionInitParameters>(initParam) ?? new VisionInitParameters();
                OnInit(InitParameters);
            }

            public virtual void SetIOInstance(List<IBaseStateIO> ioList)
            {
                if (ioList == null || ioList.Count < 2)
                {
                    ChangeCCDOrJet = null;
                    TriggerCCDIO = null;
                    return;
                }

                ChangeCCDOrJet = ioList[0];
                TriggerCCDIO = ioList[1];
            }

            public virtual void TriggerCCD()
            {
                if (!InitParameters.EnableFlying || ChangeCCDOrJet == null || TriggerCCDIO == null)
                {
                    return;
                }

                ChangeCCDOrJet.Write(true);
                TriggerCCDIO.Write(true);
                Thread.Sleep(1);
                TriggerCCDIO.Write(false);
                ChangeCCDOrJet.Write(false);
            }

            public abstract void GrabOne();

            public abstract Point2D[] GetFlyingRst();

            public abstract byte[] Preprocess(byte[] preprocessParam);

            public abstract void CreateModel(SearchMarkParams searchMarkParams);

            public abstract SearchMarkResult SearchMark(SearchMarkParams searchMarkParams);

            public abstract TwoDResultBase SearchBlob(SearchBlobParams searchBlobParams);

            public abstract byte[] SearchLine(byte[] searchLineParams);

            public abstract byte[] SearchCircle(byte[] searchCircleParams);

            public abstract byte[] SearchContour(byte[] searchContourParams);

            public abstract Point2D CalibrateFor9Pt(SearchMarkParams searchMarkParams);

            public abstract void CalibForFlying(SearchMarkParams searchMarkParams);

            public abstract Point2D CoordinateTransform(Point2D imagePoint, byte[] transformParam);

            public abstract bool Detect(byte[] detectParam);

            public abstract string ScanCode(ScanCodeParams scanCodeParams);

            #endregion

            #region 受保护方法

            protected abstract void OnInit(VisionInitParameters initParameters);

            #endregion
        }
    }
}
