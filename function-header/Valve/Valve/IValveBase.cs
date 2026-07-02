using GKG.ElectronicControl.General;
namespace GKG
{
    namespace ElectronicControl
    {
        namespace Dispenser
        {
            public enum ValveType
            {
                GKGPiezoValve = 0,
            }
            public interface IValveBase
            {
                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="InitParams"></param>
                void Init(byte[] InitParams);

                /// <summary>
                /// 释放
                /// </summary>
                void Dispose();

                /// <summary>
                /// 设置配方参数
                /// </summary>
                /// <param name="formulaParams"></param>
                void SetFormulaParams(byte[] formulaParams);

                /// <summary>
                /// 打开
                /// </summary>
                void Open();

                /// <summary>
                /// 打开
                /// </summary>
                void Open(int pointCount);

                /// <summary>
                /// 关闭
                /// </summary>
                void Close();

                /// <summary>
                /// 设置阀气压
                /// </summary>
                /// <param name="param"></param>
                void SetValveAirPressure(DetectPressureParams param);

                /// <summary>
                /// 获取阀气压
                /// </summary>
                /// <returns></returns>
                double GetValveAirPressure();

                void SetValveParam();

                byte[] GetValveParam();
            }

            public class ValveBase : IValveBase
            {
                /// <summary>
                /// 气压控制
                /// </summary>
                protected IPressureControlBase pressureControl;

                /// <summary>
                /// 阀通讯
                /// </summary>
                protected IBaseCommunicate communicator;

                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="InitParams"></param>
                public virtual void Init(byte[] InitParams)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// 设置配方参数
                /// </summary>
                /// <param name="formulaParams"></param>
                public virtual void SetFormulaParams(byte[] formulaParams)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// 打开
                /// </summary>
                public virtual void Open()
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// 打开
                /// </summary>
                public virtual void Open(int pointCount)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// 关闭
                /// </summary>
                public virtual void Close()
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// 设置阀气压
                /// </summary>
                /// <param name="pressure"></param>
                public virtual void SetValveAirPressure(DetectPressureParams param)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// 获取阀气压
                /// </summary>
                /// <returns></returns>
                public virtual double GetValveAirPressure()
                {
                    throw new NotImplementedException();
                }

                public virtual void SetValveParam()
                {
                    throw new NotImplementedException();
                }

                public virtual byte[] GetValveParam()
                {
                    throw new NotImplementedException();
                }

                public virtual void Dispose()
                {
                    throw new NotImplementedException();
                }
            }

            public class DetectPressureParams
            {
                public bool DetectPressure { get; set; } = false;
                public double ValvePressure { get; set; } = 0;
                public double MaxAllowablePressure { get; set; } = 0;
                public double MinAllowablePressure { get; set; } = 0;
            }

            public static class ValveFactory
            {
                public static IValveBase CreateValve(ValveType valveType)
                {
                    return valveType switch
                    {
                        ValveType.GKGPiezoValve => new GKGPiezoValve(),
                        _ => throw new ArgumentException(nameof(valveType),new Exception("不支持的阀类型")),
                    };
                }
            }
        }
    }
}