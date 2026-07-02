using GF_Gereric;
using GKG.DispenserMachineModules.Source;
using GKG.MotionControl;
using GKG.SubMM;
using Griffins.ImeIOT;
using System.IO;

namespace GKG
{
    namespace MM
    {
        public partial class MMCmdExecutor : IMMCmdExecutor
        {
            #region mark

            public MarkResults RunMark(Mark mark)
            {
                mark.Results.Clear();
                // mark禁用或模板基准跳过视觉
                if (mark.Enabled == false || mark.MarkType == MarkType.TemplateBaseline)
                {
                    return mark.Results;
                }
                // 执行视觉方法
                //MarkResults results = new MarkResults();
                bool isMarkOK = false;
                foreach (var markItem in mark.MarkListCollection)
                {
                    MarkResult result = new MarkResult();
                    // 备份Mark逻辑
                    foreach (var item in markItem)
                    {
                        // 移动位置
                        //item.Position;
                        // 移动到安全高度
                        MoveParam moveParam = new MoveParam()
                        {
                            targetPosition = new Point3D(0, 0, initCfg.SafetyHeight),
                            AxisCount = 3,
                            logicAxis = new int[] { AxisConstants.Z },
                            speed = initCfg.ZSpeed,
                            acc = initCfg.ZAcc
                        };
                        iMMCmdExecutorCallBack.ExecSubMMMethod(
                            DispenserMachineModulesConst.Robot_Alias,
                            CategoryARobotSubMachineModulesConst.MoveMethodID,
                            JsonObjConvert.ToJSon(moveParam),
                            moveParam,
                            out string jsResult,
                            out object objResult,
                            out string errorMsg);
                        // XY移动
                        moveParam = new MoveParam()
                        {
                            targetPosition = item.Position,
                            AxisCount = 2,
                            logicAxis = new int[] { AxisConstants.X, AxisConstants.Y },
                            speed = initCfg.XYSpeed,
                            acc = initCfg.XYAcc
                        };
                        iMMCmdExecutorCallBack.ExecSubMMMethod(
                            DispenserMachineModulesConst.Robot_Alias,
                            CategoryARobotSubMachineModulesConst.MoveMethodID,
                            JsonObjConvert.ToJSon(moveParam),
                            moveParam,
                            out jsResult,
                            out objResult,
                            out errorMsg);
                        // Z移动
                        moveParam = new MoveParam()
                        {
                            targetPosition = new Point3D(0, 0, item.Position.Z),
                            AxisCount = 2,
                            logicAxis = new int[] { AxisConstants.X, AxisConstants.Y },
                            speed = initCfg.ZSpeed,
                            acc = initCfg.ZAcc
                        };
                        iMMCmdExecutorCallBack.ExecSubMMMethod(
                            DispenserMachineModulesConst.Robot_Alias,
                            CategoryARobotSubMachineModulesConst.MoveMethodID,
                            JsonObjConvert.ToJSon(moveParam),
                            moveParam,
                            out jsResult,
                            out objResult,
                            out errorMsg);

                        // 执行视觉识别
                        // ...
                        SearchMarkParams searchMarkParams = new SearchMarkParams()
                        {
                            ModelPath = "",
                            mmPerPixel = 0.0,
                            CameraParameters = new CameraParameters(),
                            LightParameters = new LightParameters(),
                            ScriptParameters = new MarkScriptParameters()
                        };
                        iMMCmdExecutorCallBack.ExecSubMMMethod(
                                        DispenserMachineModulesConst.Vision_Alias,
                                        VisionSubMachineModulesConst.SearchMarkMethodID,
                                        JsonObjConvert.ToJSon(searchMarkParams),
                                        searchMarkParams,
                                        out jsResult,
                                        out objResult,
                                        out errorMsg);
                        SearchMarkResult searchMarkResult = (SearchMarkResult)objResult;
                        isMarkOK = searchMarkResult.IsOk;
                        // 视觉识别成功
                        if (isMarkOK)
                        {
                            result.Position = item.Position;
                            result.Offset = searchMarkResult.Offset;
                            result.Angle = searchMarkResult.Angle;
                            result.ScaleX = searchMarkResult.ScaleX;
                            result.ScaleY = searchMarkResult.ScaleY;
                            break;
                        }
                    }
                    if (!isMarkOK)
                    {
                        // 视觉识别失败
                        return mark.Results;
                    }
                    else
                    {
                        mark.Results.Add(result);
                    }
                }
                // Mark逻辑运算
                // 检测mark偏差
                // 检测
                // ...
                return mark.Results;
            }

            public delegate MarkResults RunMarkDelegate(Mark mark);

            public partial class Mark : CmdBase
            {
                /// <summary>
                /// mark运行处理委托
                /// </summary>
                public RunMarkDelegate? RunMarkHandler { get; set; }

                /// <summary>
                /// 使用委托运行Mark
                /// </summary>
                /// <param name="mark"></param>
                /// <returns></returns>
                /// <exception cref="InvalidOperationException"></exception>
                public MarkResults RunByDelegate(Mark mark)
                {
                    if (RunMarkHandler != null)
                        return RunMarkHandler(mark);
                    else
                        throw new GKGException(DispenserErrorCode.RunMarkHandlerNull, DispenserErr.RunMarkHandlerNull, DispenserErr.RunMarkHandlerNull);
                }

                /// <summary>
                /// mark运行
                /// </summary>
                /// <returns></returns>
                public override void Run()
                {
                    RunByDelegate(this);
                }
            }

            #endregion mark

            #region badmark

            public BadMarkResult RunBadMark(BadMark badmark)
            {
                badmark.Result.IsOk = false;
                // mark禁用或模板基准跳过视觉
                if (badmark.Enabled == false)
                {
                    return badmark.Result;
                }
                // 执行视觉方法
                //MarkResults results = new MarkResults();
                bool isMarkOK = false;

                var item = badmark.MarkParam;
                // 移动位置
                //item.Position;
                // 移动到安全高度
                MoveParam moveParam = new MoveParam()
                {
                    targetPosition = new Point3D(0, 0, initCfg.SafetyHeight),
                    AxisCount = 3,
                    logicAxis = new int[] { AxisConstants.Z },
                    speed = initCfg.ZSpeed,
                    acc = initCfg.ZAcc
                };
                iMMCmdExecutorCallBack.ExecSubMMMethod(
                    DispenserMachineModulesConst.Robot_Alias,
                    CategoryARobotSubMachineModulesConst.MoveMethodID,
                    JsonObjConvert.ToJSon(moveParam),
                    moveParam,
                    out string jsResult,
                    out object objResult,
                    out string errorMsg);
                // XY移动
                moveParam = new MoveParam()
                {
                    targetPosition = item.Position,
                    AxisCount = 2,
                    logicAxis = new int[] { AxisConstants.X, AxisConstants.Y },
                    speed = initCfg.XYSpeed,
                    acc = initCfg.XYAcc
                };
                iMMCmdExecutorCallBack.ExecSubMMMethod(
                    DispenserMachineModulesConst.Robot_Alias,
                    CategoryARobotSubMachineModulesConst.MoveMethodID,
                    JsonObjConvert.ToJSon(moveParam),
                    moveParam,
                    out jsResult,
                    out objResult,
                    out errorMsg);
                // Z移动
                moveParam = new MoveParam()
                {
                    targetPosition = new Point3D(0, 0, item.Position.Z),
                    AxisCount = 2,
                    logicAxis = new int[] { AxisConstants.X, AxisConstants.Y },
                    speed = initCfg.ZSpeed,
                    acc = initCfg.ZAcc
                };
                iMMCmdExecutorCallBack.ExecSubMMMethod(
                    DispenserMachineModulesConst.Robot_Alias,
                    CategoryARobotSubMachineModulesConst.MoveMethodID,
                    JsonObjConvert.ToJSon(moveParam),
                    moveParam,
                    out jsResult,
                    out objResult,
                    out errorMsg);

                // 执行视觉识别
                // ...
                SearchMarkParams searchMarkParams = new SearchMarkParams()
                {
                    ModelPath = "",
                    mmPerPixel = 0.0,
                    CameraParameters = new CameraParameters(),
                    LightParameters = new LightParameters(),
                    ScriptParameters = new MarkScriptParameters()
                };
                iMMCmdExecutorCallBack.ExecSubMMMethod(
                                DispenserMachineModulesConst.Vision_Alias,
                                VisionSubMachineModulesConst.SearchMarkMethodID,
                                JsonObjConvert.ToJSon(searchMarkParams),
                                searchMarkParams,
                                out jsResult,
                                out objResult,
                                out errorMsg);
                SearchMarkResult searchMarkResult = (SearchMarkResult)objResult;
                isMarkOK = searchMarkResult.IsOk;

                // 视觉识别成功
                if (isMarkOK)
                {
                    badmark.Result.IsOk = true;
                }

                // Mark逻辑运算
                // 检测mark偏差
                // 检测
                // ...
                return badmark.Result;
            }

            public delegate BadMarkResult RunBadMarkDelegate(BadMark badmark);

            public partial class BadMark : CmdBase
            {
                /// <summary>
                /// badmark运行处理委托
                /// </summary>
                public RunBadMarkDelegate? RunBadMarkHandler { get; set; }

                /// <summary>
                /// 使用委托运行Mark
                /// </summary>
                /// <param name="mark"></param>
                /// <returns></returns>
                /// <exception cref="InvalidOperationException"></exception>
                public BadMarkResult RunByDelegate(BadMark badmark)
                {
                    if (RunBadMarkHandler != null)
                        return RunBadMarkHandler(badmark);
                    else
                        throw new GKGException(DispenserErrorCode.RunBadMarkHandlerNull, DispenserErr.RunBadMarkHandlerNull, DispenserErr.RunBadMarkHandlerNull);
                }

                /// <summary>
                /// badmark运行
                /// </summary>
                public override void Run()
                {
                    RunByDelegate(this);
                }
            }

            #endregion badmark

            #region Edge

            public void RunEdge(Edge edge)
            {
                if (edge.Enabled == false)
                {
                    return;
                }
                Dictionary<int, List<EdgePoint>> edgePoints = new Dictionary<int, List<EdgePoint>>();
                foreach (var trajectory in edge.EdgeTrajectory)
                {
                    if (trajectory.TrajectoryType == MotionTrajectoryType.Linear)
                    {
                        foreach (var item in ((LinearMotionTrajectory)trajectory).LinearMotionTrajectoryItems)
                        {
                            // 识别边缘
                            switch (item.ItemType)
                            {
                                case LinearMotionTrajectoryItemType.StraightLine:
                                    {
                                        // 抓边
                                        EdgeStraightLine straightLine = (EdgeStraightLine)item.ItemBase;
                                        // 参数错误
                                        if (straightLine?.EndPoint?.Position?.Length < 3)
                                            throw new GKGException(DispenserErrorCode.EdgeEndPointPositionInvalid, DispenserErr.EdgeEndPointPositionInvalid, DispenserErr.EdgeEndPointPositionInvalid);
                                        EdgePoint endPoint = (EdgePoint)straightLine.EndPoint;
                                        //SearchEdgeParams searchEdgeParams = endPoint.EdgeStyleParam;
                                        //iMMCmdExecutorCallBack.ExecSubMMMethod(
                                        //    DispenserMachineModulesConst.Vision_Alias,
                                        //    VisionSubMachineModulesConst.SearchEdgeMethodID,
                                        //    JsonObjConvert.ToJSon(searchEdgeParams),
                                        //    searchEdgeParams,
                                        //    out string jsResult,
                                        //    out object objResult,
                                        //    out string errorMsg);

                                        //SearchEdgeResult searchEdgeResult = (SearchEdgeResult)objResult;
                                        //if (!searchEdgeResult.IsOk)
                                        //{
                                        //    // 抓边失败处理
                                        //    // ...
                                        //    return;
                                        //}
                                        if (!edgePoints.ContainsKey(endPoint.VisualFieldIndex))
                                            edgePoints.Add(endPoint.VisualFieldIndex, new List<EdgePoint>());

                                        edgePoints[endPoint.VisualFieldIndex].Add(endPoint);
                                    }
                                    break;

                                case LinearMotionTrajectoryItemType.ArcA:
                                    {
                                        // 抓边
                                        EdgeArcA arcA = (EdgeArcA)item.ItemBase;
                                        // 参数错误
                                        if (arcA?.EndPoint?.Position?.Length < 3 || arcA?.MiddlePoint?.Position?.Length < 3)
                                            throw new GKGException(DispenserErrorCode.EdgeEndPointPositionInvalid, DispenserErr.EdgeEndPointPositionInvalid, DispenserErr.EdgeEndPointPositionInvalid);
                                        EdgePoint middlePoint = (EdgePoint)arcA.MiddlePoint;
                                        EdgePoint endPoint = (EdgePoint)arcA.EndPoint;

                                        if (!edgePoints.ContainsKey(middlePoint.VisualFieldIndex))
                                            edgePoints.Add(middlePoint.VisualFieldIndex, new List<EdgePoint>());

                                        edgePoints[middlePoint.VisualFieldIndex].Add(middlePoint);

                                        if (!edgePoints.ContainsKey(endPoint.VisualFieldIndex))
                                            edgePoints.Add(endPoint.VisualFieldIndex, new List<EdgePoint>());

                                        edgePoints[endPoint.VisualFieldIndex].Add(endPoint);
                                    }
                                    break;

                                case LinearMotionTrajectoryItemType.ArcB:
                                    {
                                        // 抓边
                                        EdgeArcB arcB = (EdgeArcB)item.ItemBase;
                                        // 参数错误
                                        if (arcB?.EndPoint?.Position?.Length < 3 || arcB?.CenterPoint?.Position?.Length < 3)
                                            throw new GKGException(DispenserErrorCode.EdgeEndPointPositionInvalid, DispenserErr.EdgeEndPointPositionInvalid, DispenserErr.EdgeEndPointPositionInvalid);
                                        EdgePoint centerPoint = (EdgePoint)arcB.CenterPoint;
                                        EdgePoint endPoint = (EdgePoint)arcB.EndPoint;

                                        if (!edgePoints.ContainsKey(centerPoint.VisualFieldIndex))
                                            edgePoints.Add(centerPoint.VisualFieldIndex, new List<EdgePoint>());

                                        edgePoints[centerPoint.VisualFieldIndex].Add(centerPoint);

                                        if (!edgePoints.ContainsKey(endPoint.VisualFieldIndex))
                                            edgePoints.Add(endPoint.VisualFieldIndex, new List<EdgePoint>());

                                        edgePoints[endPoint.VisualFieldIndex].Add(endPoint);
                                    }
                                    break;

                                case LinearMotionTrajectoryItemType.CircleA:
                                    {
                                        // 抓边
                                        EdgeCircleA circleA = (EdgeCircleA)item.ItemBase;
                                        // 参数错误
                                        if (circleA?.EndPoint?.Position?.Length < 3 || circleA?.MiddlePoint?.Position?.Length < 3)
                                            throw new GKGException(DispenserErrorCode.EdgeEndPointPositionInvalid, DispenserErr.EdgeEndPointPositionInvalid, DispenserErr.EdgeEndPointPositionInvalid);
                                        EdgePoint middlePoint = (EdgePoint)circleA.MiddlePoint;
                                        EdgePoint endPoint = (EdgePoint)circleA.EndPoint;

                                        if (!edgePoints.ContainsKey(middlePoint.VisualFieldIndex))
                                            edgePoints.Add(middlePoint.VisualFieldIndex, new List<EdgePoint>());

                                        edgePoints[middlePoint.VisualFieldIndex].Add(middlePoint);

                                        if (!edgePoints.ContainsKey(endPoint.VisualFieldIndex))
                                            edgePoints.Add(endPoint.VisualFieldIndex, new List<EdgePoint>());

                                        edgePoints[endPoint.VisualFieldIndex].Add(endPoint);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        EdgeDot dot = ((EdgeDot)trajectory);
                        if (!edgePoints.ContainsKey(dot.VisualFieldIndex))
                            edgePoints.Add(dot.VisualFieldIndex, new List<EdgePoint>());

                        edgePoints[dot.VisualFieldIndex].Add((EdgePoint)dot.TargetPoint);
                    }
                }

                // 遍历字典
                foreach (var visualField in edgePoints)
                {
                    // 按视野
                    // 识别边缘
                    List<SearchEdgeParams> searchEdgeParams = new List<SearchEdgeParams>();
                    foreach (var edgePoint in visualField.Value)
                    {
                        searchEdgeParams.Add(edgePoint.EdgeStyleParam);
                    }
                    // 移动
                    MoveParam moveParam = new MoveParam()
                    {
                        targetPosition = new Point3D(0, 0, visualField.Value[0].Position[2].PositionValue),
                        AxisCount = 1,
                        logicAxis = new int[] { AxisConstants.Z },
                        speed = initCfg.ZSpeed,
                        acc = initCfg.ZAcc
                    };
                    moveParam = new MoveParam()
                    {
                        targetPosition = new Point3D(visualField.Value[0].Position[0].PositionValue, visualField.Value[0].Position[1].PositionValue, 0),
                        AxisCount = 2,
                        logicAxis = new int[] { AxisConstants.X, AxisConstants.Y },
                        speed = initCfg.XYSpeed,
                        acc = initCfg.XYAcc
                    };
                    iMMCmdExecutorCallBack.ExecSubMMMethod(
                        DispenserMachineModulesConst.Robot_Alias,
                        CategoryARobotSubMachineModulesConst.MoveMethodID,
                        JsonObjConvert.ToJSon(searchEdgeParams),
                        searchEdgeParams,
                        out string jsResult,
                        out object objResult,
                        out string errorMsg);
                    iMMCmdExecutorCallBack.ExecSubMMMethod(
                        DispenserMachineModulesConst.Vision_Alias,
                        VisionSubMachineModulesConst.SearchEdgeMethodID,
                        JsonObjConvert.ToJSon(searchEdgeParams),
                        searchEdgeParams,
                        out jsResult,
                        out objResult,
                        out errorMsg);
                    List<SearchEdgeResult> searchEdgeResult = (List<SearchEdgeResult>)objResult;
                    if (visualField.Value.Count != searchEdgeResult.Count)
                    {
                        // 抓边失败处理
                    }
                    // 更新边缘点位置
                    for (int i = 0; i < searchEdgeResult.Count; i++)
                    {
                        if (searchEdgeResult[i].IsOk)
                        {
                            visualField.Value[i].Position[0].PositionValue += searchEdgeResult[i].Offset.X;
                            visualField.Value[i].Position[1].PositionValue += searchEdgeResult[i].Offset.Y;
                        }
                        else
                        {
                            // 抓边失败处理
                            // ...
                            return;
                        }
                    }
                }
            }

            #endregion Edge

            #region Path

            /// <summary>
            /// 路径运行
            /// </summary>
            /// <param name="DicPath">多阀配对后的路径</param>
            public void RunPath(Dictionary<string, Path> dicPath)
            {
                List<string> functionHeadName = new List<string>();
                List<MotionCalculationParameters> motionCalculationParameters = new List<MotionCalculationParameters>();

                foreach (var path in dicPath)
                {
                    if (path.Value.Enabled == true)
                    {
                        functionHeadName.Add(path.Key);
                        motionCalculationParameters.Add(new MotionCalculationParameters()
                        {
                            // 障碍参数
                            BarrierParametersOfTheProduct = null,
                            // 产品加工轨迹
                            ProductProcessingTrajectory = new ProductProcessingTrajectoryItem[1]{new ProcessingTrajectory()
                        {
                                // 轨迹参数
                            MotionTrajectory = path.Value.motionTrajectory,
                            // 加工前后处理参数
                            PrePostProcessingParameters = path.Value.PrePostProcessingParameters
                        } }
                        });
                    }
                }

                MotionCalculateParameters motionCalculateParameters = new MotionCalculateParameters()
                {
                    functionHeadIDs = functionHeadName.ToArray(),
                    parameters = motionCalculationParameters.ToArray()
                };
                // 运动计算
                iMMCmdExecutorCallBack.ExecSubMMMethod(
                   DispenserMachineModulesConst.MotionCalculate_Alias,
                   MotionCalculateSubMachineModulesConst.CalculateMethodID,
                   JsonObjConvert.ToJSon(motionCalculateParameters),
                   motionCalculateParameters,
                   out string jsResult,
                   out object objResult,
                   out string errorMsg);

                // 执行路径运动
                // ...
                MotionTrajectory motionTrajectory = (MotionTrajectory)objResult;
                // 执行点胶路径
                iMMCmdExecutorCallBack.ExecSubMMMethod(
                  DispenserMachineModulesConst.Robot_Alias,
                  CategoryARobotSubMachineModulesConst.ContinuousInterpolationMotionMethodID,
                  JsonObjConvert.ToJSon(motionTrajectory.MotionInstructions),
                  motionTrajectory.MotionInstructions,
                  out jsResult,
                  out objResult,
                  out errorMsg);
            }

            #endregion Path

            #region Dot

            public void RunDot(Dictionary<string, Dot> dicDot)
            {
                //DotMotionTrajectory dotMotionTrajectory = processingTrajectory.MotionTrajectory as DotMotionTrajectory;
                //DotPrePostProcessingParameters prePostProcessingParameters = JsonObjConvert.FromJSonBytes<DotPrePostProcessingParameters>(processingTrajectory.PrePostProcessingParameters.Value.ExtendedParameters);
                //if (dotMotionTrajectory.TargetPoint == null)
                //{
                //    throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull);
                //}
                //List<MotionInstructionBase> motionInstructionList = new List<MotionInstructionBase>();
                //// 获取触发点
                //Point3D TargetPoint = new Point3D(dotMotionTrajectory.TargetPoint.Position[0].PositionValue, dotMotionTrajectory.TargetPoint.Position[1].PositionValue, dotMotionTrajectory.TargetPoint.Position[2].PositionValue);
                //motionInstructionList.Add(new DotMotionInstruction
                //{
                //    TargetPosition = point3DToAxisConstantValues(TargetPoint),
                //    Speed = prePostProcessingParameters.DispensingHeight,
                //    Acceleration = motionCalculateSubMachineModulesInitCfg.ProcessingAcceleration,
                //});
            }

            #endregion Dot
        }
    }
}