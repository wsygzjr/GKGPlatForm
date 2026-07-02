using GF_Gereric;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public interface IBaseCylinder
            {
                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="initCfg"></param>
                void Init(byte[] initCfg);

                /// <summary>
                /// 伸出
                /// </summary>
                void Stretch();

                /// <summary>
                /// 缩回
                /// </summary>
                void Retract();

                /// <summary>
                /// 设置状态量IO实例列表
                /// </summary>
                /// <param name="stateIOList"></param>
                void SetStateIOInstanceList(List<IBaseStateIO> stateIOList);

                /// <summary>
                /// 伸出完成事件
                /// </summary>
                event EventHandler? StretchFinished;

                /// <summary>
                /// 缩回完成事件
                /// </summary>
                event EventHandler? RetractFinished;

                /// <summary>
                /// 气缸状态
                /// </summary>
                ECylinderPosType CylinderPosType { get; }
            }

            public abstract class CylinderBaseClass : IBaseCylinder
            {
                #region 受保护字段

                protected CylinderInitParameters? initParameters;

                #endregion

                #region 公有属性

                public ECylinderPosType CylinderPosType => _GetCylinderPosType();

                #endregion

                #region 公有事件

                public event EventHandler? StretchFinished;
                public event EventHandler? RetractFinished;

                #endregion

                #region 公有方法

                public virtual void Init(byte[] initCfg)
                {
                    initParameters = JsonObjConvert.FromJSonBytes<CylinderInitParameters>(initCfg);
                }

                public abstract void Retract();

                public abstract void Stretch();

                public abstract void SetStateIOInstanceList(List<IBaseStateIO> stateIOList);

                #endregion

                #region 受保护方法

                protected void OnStretchFinished()
                {
                    StretchFinished?.Invoke(this, EventArgs.Empty);
                }

                protected void OnRetractFinished()
                {
                    RetractFinished?.Invoke(this, EventArgs.Empty);
                }

                protected abstract ECylinderPosType _GetCylinderPosType();

                #endregion
            }

            public static class CylinderFactory
            {
                #region 公有方法

                public static IBaseCylinder CreateCylinder(ECylinderType cylinderType)
                {
                    switch (cylinderType)
                    {
                        case ECylinderType.SingleControlSingleLimit:
                            return new SingleActSigleLimCy();
                        case ECylinderType.SingleControlDoubleLimit:
                            return new SingleActDualLimCy();
                        case ECylinderType.DoubleControlSingleLimit:
                            return new DualActSingleLimCy();
                        case ECylinderType.DoubleControlDoubleLimit:
                            return new DualActDualLimCy();
                        default:
                            throw new ArgumentOutOfRangeException(nameof(cylinderType), cylinderType, null);
                    }
                }

                #endregion
            }
        }
    }
}
