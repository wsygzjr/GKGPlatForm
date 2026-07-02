using System.Text.Json;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            /// <summary>
            /// 单控单限位气缸
            /// </summary>
            public class SingleActSigleLimCy : CylinderBaseClass
            {
                public override void Retract()
                {
                    controlStateIO.Write(false);
                    Thread.Sleep(initParameters.CylinderDelay);
                    if (limitStateIO.CheckValue(false, initParameters.CylinderDelay))
                    {
                        base.OnRetractFinished();
                    }
                }

                public override void Stretch()
                {
                    controlStateIO.Write(true);
                    Thread.Sleep(initParameters.CylinderDelay);
                    if (limitStateIO.CheckValue(true, initParameters.CylinderDelay))
                    {
                        base.OnStretchFinished();
                    }
                }

                public override void SetStateIOInstanceList(List<IBaseStateIO> stateIOList)
                {
                    if (stateIOList == null || stateIOList.Count != 2)
                        throw new ArgumentException("SingleActSigleLimCy SetStateIOInstanceList error: stateIOList is invalid!");
                    controlStateIO = stateIOList[0];
                    limitStateIO = stateIOList[1];
                }

                protected override ECylinderPosType _GetCylinderPosType()
                {
                    if (limitStateIO.Read())
                    {
                        return ECylinderPosType.Stretch;
                    }
                    else
                    {
                        return ECylinderPosType.Retract;
                    }
                }

                private IBaseStateIO controlStateIO;

                private IBaseStateIO limitStateIO;
            }
        }
    }
}