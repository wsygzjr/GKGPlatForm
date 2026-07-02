namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public interface IBaseThermostat
            {
                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="initCfg"></param>
                void Init(byte[] initCfg);

                /// <summary>
                /// 开始加热
                /// </summary>
                void StartThermostat();

                /// <summary>
                /// 停止加热
                /// </summary>
                void StopThermostat();

                /// <summary>
                /// 设置温度和加热时间
                /// </summary>
                /// <param name="temperature">温度</param>
                /// <param name="time">加热时间</param>
                void SetTemperatureAndTime(double temperature, int time);

                event EventHandler StartThermostatFinished;

                event EventHandler StopThermostatFinished;

                double CurTemperature { get; }
            }

            public abstract class ThermostatBaseClass : IBaseThermostat
            {
                #region 公有事件

                public event EventHandler? StartThermostatFinished;
                public event EventHandler? StopThermostatFinished;

                #endregion

                #region 公有属性

                public virtual double CurTemperature => GetCurTemperature();

                #endregion

                #region 公有方法

                public abstract void Init(byte[] initCfg);

                public abstract void StartThermostat();

                public abstract void StopThermostat();

                public abstract void SetTemperatureAndTime(double temperature, int time);

                public abstract double GetCurTemperature();

                #endregion

                #region 受保护方法

                protected virtual void OnStartThermostatFinished()
                {
                    StartThermostatFinished?.Invoke(this, EventArgs.Empty);
                }

                protected virtual void OnStopThermostatFinished()
                {
                    StopThermostatFinished?.Invoke(this, EventArgs.Empty);
                }

                #endregion
            }
        }
    }
}
