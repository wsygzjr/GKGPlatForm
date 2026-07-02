using System.Text.Json;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            /// <summary>
            /// 双控双限位气缸
            /// </summary>
            public class DualActDualLimCy : CylinderBaseClass
            {
                /// <summary>
                /// 气缸缩回
                /// </summary>
                public override void Retract()
                {
                    onControlStateIO.Write(false);
                    offControlStateIO.Write(true);
                    if (upperLimitStateIO.CheckValue(false, initParameters.CylinderDelay) && lowerLimitStateIO.CheckValue(true, initParameters.CylinderDelay))
                    {
                        base.OnRetractFinished();
                    }
                }

                /// <summary>
                /// 气缸伸出
                /// </summary>
                public override void Stretch()
                {
                    onControlStateIO.Write(true);
                    offControlStateIO.Write(false);
                    if (upperLimitStateIO.CheckValue(true, initParameters.CylinderDelay) && lowerLimitStateIO.CheckValue(false, initParameters.CylinderDelay))
                    {
                        base.OnStretchFinished();
                    }
                }

                public override void SetStateIOInstanceList(List<IBaseStateIO> stateIOList)
                {
                    if (stateIOList == null || stateIOList.Count != 4)
                        throw new ArgumentException("DualActDualLimCy SetStateIOInstanceList error: stateIOList is invalid!");
                    onControlStateIO = stateIOList[0];
                    offControlStateIO = stateIOList[1];
                    upperLimitStateIO = stateIOList[2];
                    lowerLimitStateIO = stateIOList[3];
                }

                protected override ECylinderPosType _GetCylinderPosType()
                {
                    if (upperLimitStateIO.Read() == true && lowerLimitStateIO.Read() == false)
                    {
                        return ECylinderPosType.Stretch;
                    }
                    else
                    {
                        return ECylinderPosType.Retract;
                    }
                }

                /// <summary>
                /// 气缸伸出控制IO
                /// </summary>
                private IBaseStateIO onControlStateIO;

                /// <summary>
                /// 气缸缩回出控制IO
                /// </summary>
                private IBaseStateIO offControlStateIO;

                /// <summary>
                /// 气缸上限位IO
                /// </summary>
                private IBaseStateIO upperLimitStateIO;

                /// <summary>
                /// 气缸下限位IO
                /// </summary>
                private IBaseStateIO lowerLimitStateIO;
            }
        }
    }
}