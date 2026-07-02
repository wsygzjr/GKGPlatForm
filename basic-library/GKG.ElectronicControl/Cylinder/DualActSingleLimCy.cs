using System.Text.Json;

namespace GKG.ElectronicControl.General
{
    public class DualActSingleLimCy : CylinderBaseClass
    {
        /// <summary>
        /// 气缸缩回
        /// </summary>
        public override void Retract()
        {
            onControlStateIO.Write(false);
            offControlStateIO.Write(true);

            if (limitStateIO.CheckValue(true, initParameters.CylinderDelay))
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
            if (limitStateIO.CheckValue(true, initParameters.CylinderDelay))
            {
                base.OnStretchFinished();
            }
        }

        public override void SetStateIOInstanceList(List<IBaseStateIO> stateIOList)
        {
            if (stateIOList == null || stateIOList.Count != 3)
                throw new ArgumentException("SingleActDualLimCy SetStateIOInstanceList error: stateIOList is invalid!");
            onControlStateIO = stateIOList[0];
            offControlStateIO = stateIOList[0];
            limitStateIO = stateIOList[1];
        }

        protected override ECylinderPosType _GetCylinderPosType()
        {
            if (limitStateIO.Read() == true)
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
        private IBaseStateIO onControlStateIO;

        /// <summary>
        /// 气缸控制IO
        /// </summary>
        private IBaseStateIO offControlStateIO;

        /// <summary>
        /// 气缸上限位IO
        /// </summary>
        private IBaseStateIO limitStateIO;

    }
}
