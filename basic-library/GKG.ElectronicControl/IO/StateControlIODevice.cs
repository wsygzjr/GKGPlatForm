using System.Text.Json;

namespace GKG
{
    namespace ElectronicControl
    {
        public class StateControlIODevice : IBaseStateIO
        {
            void IBaseStateIO.Init(byte[] initCfg)
            {

            }

            bool IBaseStateIO.Read()
            {
                bool ret = false;
                if (sensorCycleReadInterval.cycleReadNums > 0)
                {
                    int readNums = sensorCycleReadInterval.cycleReadNums;
                    while (true)
                    {
                        if (readNums <= 0)
                            break;
                        ret = baseIO.StateRead(iOStateInitParameters.channelID);
                        Thread.Sleep(sensorCycleReadInterval.cycleReadTimes);
                        readNums--;
                    }
                }
                else
                    ret = baseIO.StateRead(iOStateInitParameters.channelID);
                return ret;
            }

            void IBaseStateIO.Write(bool state)
            {
                baseIO.StateWrite(iOStateInitParameters.channelID, state);
            }

            void IBaseStateIO.SetDeviceInstance(IBaseIO baseIO)
            {
                this.baseIO = baseIO;
            }

            bool IBaseStateIO.CheckValue(bool state, int outTime)
            {
                PerfmTimer timer = new PerfmTimer();
                timer.Start();
                while (timer.GetElapsedMilliseconds() < outTime)
                {
                    if (state == baseIO.StateRead(iOStateInitParameters.channelID))
                    {
                        return true;
                    }
                }
                return false;
            }

            private IBaseIO baseIO;

            private IOStateInitParameters iOStateInitParameters;

            private SensorCycleReadInterval sensorCycleReadInterval = new SensorCycleReadInterval();

        }
    }
}
