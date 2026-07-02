using System.Text.Json;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            /// <summary>
            /// 单控双限位气缸
            /// </summary>
            public class SingleActDualLimCy : CylinderBaseClass
            {
                /// <summary>
                /// 气缸缩回
                /// </summary>
                public override void Retract()
                {
                    controlStateIO.Write(false);

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
                    controlStateIO.Write(true);

                    if (upperLimitStateIO.CheckValue(true, initParameters.CylinderDelay) && lowerLimitStateIO.CheckValue(false, initParameters.CylinderDelay))
                    {
                        base.OnStretchFinished();
                    }
                }

                public override void SetStateIOInstanceList(List<IBaseStateIO> stateIOList)
                {
                    if(stateIOList == null || stateIOList.Count != 3)
                        throw new ArgumentException("SingleActDualLimCy SetStateIOInstanceList error: stateIOList is invalid!");
                    controlStateIO = stateIOList[0];
                    upperLimitStateIO = stateIOList[1];
                    lowerLimitStateIO = stateIOList[2];
                }

                protected override ECylinderPosType _GetCylinderPosType()
                {
                    if (lowerLimitStateIO.Read() == true && upperLimitStateIO.Read() == false)
                    {
                        return ECylinderPosType.Retract;
                    }
                    else if (lowerLimitStateIO.Read() == false && upperLimitStateIO.Read() == true)
                    {
                        return ECylinderPosType.Stretch;
                    }
                    else
                    {
                        return ECylinderPosType.Retract;
                    }
                }

                /// <summary>
                /// 气缸控制IO
                /// </summary>
                private IBaseStateIO controlStateIO;

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