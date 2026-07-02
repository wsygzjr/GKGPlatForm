using System.Text.Json;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public class ThermostatTypeOML : ThermostatBaseClass
            {
                /// <summary>
                /// 初始化
                /// </summary>
                /// <param name="initCfg">初始化参数</param>
                public override void Init(byte[] initCfg)
                {
                    serialPortCommunicate.Init(initCfg);
                    serialPortCommunicate.Open(500);
                }

                public override void StartThermostat()
                {
                    int iTemperature = (int)dTemperature;
                    string strTemperature = "08102103000102" + iTemperature.ToString("X4");
                    serialPortCommunicate.Write(strTemperature);
                    if (timerThread != null && timerThread.IsAlive)
                    {
                        timerThread.Join();
                    }
                    bIsThermostat = true;
                    timerThread = new Thread(ThermostatThreadTimer);
                    timerThread.Start();
                    base.OnStartThermostatFinished();
                }

                public override void StopThermostat()
                {
                    bIsThermostat = false;
                    int iTemperature = 20;
                    string strTemperature = "08102103000102" + iTemperature.ToString("X4");
                    serialPortCommunicate.Write(strTemperature);
                    if (timerThread != null && timerThread.IsAlive)
                    {
                        timerThread.Join();
                    }
                    base.OnStopThermostatFinished();
                }

                public override void SetTemperatureAndTime(double temperature, int time)
                {
                    dTemperature = temperature;
                    iTime = time;
                }

                public override double GetCurTemperature()
                {
                    serialPortCommunicate.Write("080320000001");
                    byte[] readBytes = null;
                    serialPortCommunicate.ReadLength(7, out readBytes);
                    if (readBytes.Length != 7)
                    {
                        return -1;
                    }
                    return (double)readBytes[4];
                }

                private void ThermostatThreadTimer()
                {
                    DateTime startThermostatTime = DateTime.Now;
                    while (bIsThermostat)
                    {
                        DateTime curTime = DateTime.Now;
                        TimeSpan ellapsedTime = curTime - startThermostatTime;
                        if (ellapsedTime.TotalMilliseconds * 1000 >= iTime)
                        {
                            break;
                        }
                    }
                }

                private SerialPortCommunicate serialPortCommunicate = new SerialPortCommunicate();

                private double dTemperature;

                private int iTime;

                private Thread timerThread;

                private bool bIsThermostat;
            }
        }
    }
}