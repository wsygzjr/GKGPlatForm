using System;
using System.Globalization;
using GF_Gereric;
using ShareMemRPCLite;

namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 鍩轰簬 GVision / ShareMemRPC 鐨勯粯璁よ瑙夐┍鍔ㄣ€?        /// </summary>
        public class GVisionDriver : VisionDriverBase
        {
            #region 常量

            public const string DefaultDriverName = "GVision";

            private const string CreateModelMethod = "CreateModel";
            private const string SearchMarkMethod = "FindModel";
            private const string SearchBlobMethod = "SearchBlob";
            private const string ScanCodeMethod = "ScanCode";
            private const string CalibFor9PtMethod = "CalibFor9Pt";
            private const string CalibForFlyingMethod = "CalibForFlying";
            private const string GrabOneMethod = "GrabOne";
            private const string ModelPathKey = "ModelPath";

            #endregion

            #region 私有字段

            private readonly CallGVision call;

            #endregion

            #region 构造函数

            public GVisionDriver()
                : this(new CallGVision())
            {
            }

            public GVisionDriver(CallGVision call)
            {
                this.call = call ?? throw new ArgumentNullException(nameof(call));
            }

            #endregion

            #region 公有属性

            public override string DriverName => DefaultDriverName;

            #endregion

            #region 公有方法

            public override Point2D[] GetFlyingRst()
            {
                call.WaitRst(0, out SGVisionRtn _, 60000);
                return Array.Empty<Point2D>();
            }

            public override void GrabOne()
            {
                if (!InitParameters.EnableFlying)
                {
                    return;
                }

                call.RunNoWait(GrabOneMethod, 0);
                TriggerCCD();
                call.WaitRst(InitParameters.CameraID, out SGVisionRtn _, 4000);
            }

            public override byte[] Preprocess(byte[] preprocessParam)
            {
                throw new NotSupportedException($"{DriverName}.Preprocess 暂不支持。");
            }

            public override void CreateModel(SearchMarkParams searchMarkParams)
            {
                var prms = CreateModelPathParams(searchMarkParams.ModelPath);
                call.RunNoWait(CreateModelMethod, 0, prms.ToArray());
            }

            public override SearchMarkResult SearchMark(SearchMarkParams searchMarkParams)
            {
                var prms = CreateModelPathParams(searchMarkParams.ModelPath);
                prms.Add(CreateParam("mmPerPixel", searchMarkParams.mmPerPixel));
                prms.Add(CreateParam("Score", searchMarkParams.ScriptParameters.MarkModelParameters.Score));

                var rst = RunAndWaitAfterTrigger(SearchMarkMethod, 4000, prms.ToArray());

                return new SearchMarkResult
                {
                    IsOk = GetBool(rst, "IsOk"),
                    Offset = new Point2D(GetDouble(rst, "OffsetX"), GetDouble(rst, "OffsetY")),
                    Angle = GetDouble(rst, "Angle"),
                    ScaleX = GetDouble(rst, "ScaleX"),
                    ScaleY = GetDouble(rst, "ScaleY")
                };
            }

            public override TwoDResultBase SearchBlob(SearchBlobParams searchBlobParams)
            {
                var prms = CreateModelPathParams(searchBlobParams.ModelPath);
                var rst = RunAndWaitAfterTrigger(SearchBlobMethod, 4000, prms.ToArray());

                TwoDResultBase twoDResultBase = TwoDResultBase.Create(searchBlobParams.ScriptParameters.DetectionPass.PassType);
                twoDResultBase.IsOk = GetBool(rst, "IsOk");
                return twoDResultBase;
            }

            public override byte[] SearchLine(byte[] searchLineParams)
            {
                throw new NotSupportedException($"{DriverName}.SearchLine 暂不支持。");
            }

            public override byte[] SearchCircle(byte[] searchCircleParams)
            {
                throw new NotSupportedException($"{DriverName}.SearchCircle 暂不支持。");
            }

            public override byte[] SearchContour(byte[] searchContourParams)
            {
                throw new NotSupportedException($"{DriverName}.SearchContour 暂不支持。");
            }

            public override Point2D CalibrateFor9Pt(SearchMarkParams searchMarkParams)
            {
                var prms = CreateModelPathParams(searchMarkParams.ModelPath);
                call.RunAndWaitRst(CalibFor9PtMethod, 0, out SGVisionRtn rst, prms.ToArray());

                double[] hvHomMat2D = GetHomographyMatrix(rst);
                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                MapPoint2d(1, 1, ref y1, ref x1, hvHomMat2D);
                MapPoint2d(2, 2, ref y2, ref x2, hvHomMat2D);
                return new Point2D(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
            }

            public override void CalibForFlying(SearchMarkParams searchMarkParams)
            {
                var prms = CreateModelPathParams(searchMarkParams.ModelPath);
                call.RunAndWaitRst(CalibForFlyingMethod, 0, out SGVisionRtn rst, prms.ToArray());

                double[] hvHomMat2D = GetHomographyMatrix(rst);
                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                MapPoint2d(1, 1, ref y1, ref x1, hvHomMat2D);
                MapPoint2d(2, 2, ref y2, ref x2, hvHomMat2D);
            }

            public override Point2D CoordinateTransform(Point2D imagePoint, byte[] transformParam)
            {
                return new Point2D(imagePoint.X * 0.011222, imagePoint.Y * 0.011222);
            }

            public override bool Detect(byte[] detectParam)
            {
                throw new NotSupportedException($"{DriverName}.Detect 暂不支持。");
            }

            public override string ScanCode(ScanCodeParams scanCodeParams)
            {
                var prms = CreateModelPathParams(scanCodeParams.ModelPath);
                var rst = RunAndWaitAfterTrigger(ScanCodeMethod, 4000, prms.ToArray());

                if (rst.StringDic != null && rst.StringDic.TryGetValue("Code", out string codeRst))
                {
                    return codeRst;
                }

                return string.Empty;
            }

            #endregion

            #region 受保护方法

            protected override void OnInit(VisionInitParameters initParameters)
            {
            }

            #endregion

            #region 私有方法

            private static Tuple<string, string> CreateParam(string key, string value)
            {
                return new Tuple<string, string>(key, value);
            }

            private static Tuple<string, string> CreateParam(string key, double value)
            {
                return CreateParam(key, value.ToString(CultureInfo.InvariantCulture));
            }

            private static bool GetBool(SGVisionRtn rst, string key, bool defaultValue = false)
            {
                if (rst.BoolDic != null && rst.BoolDic.TryGetValue(key, out bool value))
                {
                    return value;
                }

                return defaultValue;
            }

            private static double GetDouble(SGVisionRtn rst, string key, double defaultValue = 0)
            {
                if (rst.DoubleDic != null && rst.DoubleDic.TryGetValue(key, out double value))
                {
                    return value;
                }

                return defaultValue;
            }

            private static double[] GetHomographyMatrix(SGVisionRtn rst)
            {
                double[] hvHomMat2D = new double[9];
                if (rst.DoubleDic == null)
                {
                    return hvHomMat2D;
                }

                for (int i = 0; i < 9; i++)
                {
                    rst.DoubleDic.TryGetValue($"dOutMat9{i}", out hvHomMat2D[i]);
                }

                return hvHomMat2D;
            }

            private static void MapPoint2d(double row, double col, ref double y, ref double x, double[] mapMatrix)
            {
                y = row * mapMatrix[0] + col * mapMatrix[1] + mapMatrix[2];
                x = row * mapMatrix[3] + col * mapMatrix[4] + mapMatrix[5];
            }

            private static List<Tuple<string, string>> CreateModelPathParams(string modelPath)
            {
                return new List<Tuple<string, string>>
                {
                    CreateParam(ModelPathKey, modelPath)
                };
            }

            private SGVisionRtn RunAndWaitAfterTrigger(string method, int timeout, params Tuple<string, string>[] prms)
            {
                call.RunNoWait(method, 0, prms);
                TriggerCCD();
                call.WaitRst(InitParameters.CameraID, out SGVisionRtn rst, (uint)timeout);
                return rst;
            }

            #endregion
        }
    }
}
