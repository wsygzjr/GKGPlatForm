using GF_Gereric;
using System.Buffers.Text;
using System.Text.Json;

namespace GKG
{
    namespace ElectronicControl
    {
        public class StateControlMotionCard : IBaseStateIO
        {
            void IBaseStateIO.Init(byte[] initCfg)
            {
                //JsonObjConvert.PopulateObject(initCfg, ioStateInitParameters);
                ioStateInitParameters = JsonObjConvert.FromJSonBytes<IOStateInitParameters>(initCfg);
            }

            bool IBaseStateIO.Read()
            {
                bool ret = false;
                ret = iMotionControlBase.StateRead(ioStateInitParameters.channelID);
                return ret;
            }

            void IBaseStateIO.Write(bool state)
            {
                iMotionControlBase.StateWrite(ioStateInitParameters.channelID, state);
            }

            void IBaseStateIO.SetDeviceInstance(IBaseIO baseIO)
            {
                //do nothing
                this.iMotionControlBase = baseIO;
            }
            bool IBaseStateIO.CheckValue(bool state, int outTime)
            {
                PerfmTimer timer = new PerfmTimer();
                timer.Start();
                while (timer.GetElapsedMilliseconds() < outTime)
                {
                    if (state == iMotionControlBase.StateRead(ioStateInitParameters.channelID))
                    {
                        return true;
                    }
                }
                return false;
            }
            private SensorCycleReadInterval sensorCycleReadInterval;

            private IOStateInitParameters ioStateInitParameters;

            private IBaseIO iMotionControlBase;
        }
    }
}
