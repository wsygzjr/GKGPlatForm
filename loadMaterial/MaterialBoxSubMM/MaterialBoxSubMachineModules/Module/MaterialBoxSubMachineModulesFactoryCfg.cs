using GF_Gereric;
using MaterialBoxSubMachineModules.FeedPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKG.SubMM.StorageDeviceModule;
using GKG.SubMM.TransportMechanismModule;

namespace GKG
{
    namespace SubMM
    {
        /// <summary>料盒子模组出厂配置。</summary>
        public class MaterialBoxSubMachineModulesFactoryCfg
        {
            /// <summary>上料侧储料装置出厂参数，定义该侧有几个储料位等基础数量信息。</summary>
            public StorageDeviceFactoryCfg LoadStorageDevice { get; set; } = new StorageDeviceFactoryCfg();

            /// <summary>下料侧储料装置出厂参数，定义该侧有几个储料位等基础数量信息。</summary>
            public StorageDeviceFactoryCfg UnloadStorageDevice { get; set; } = new StorageDeviceFactoryCfg();

            /// <summary>上料侧运输机构出厂参数。</summary>
            public TransportMechanismFactoryCfg LoadTransportMechanism { get; set; } = new TransportMechanismFactoryCfg();

            /// <summary>下料侧运输机构出厂参数。</summary>
            public TransportMechanismFactoryCfg UnloadTransportMechanism { get; set; } = new TransportMechanismFactoryCfg();

            /// <summary>送料口出厂配置参数。</summary>
            public FeedPortFactoryCfg FeedingPortFactoryCfg { get; set; } = new FeedPortFactoryCfg();

            /// <summary>接料口出厂配置参数。</summary>
            public FeedPortFactoryCfg ReceivePortFactoryCfg { get; set; } = new FeedPortFactoryCfg();

            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="source">复制源</param>
            public void CopyFrom(MaterialBoxSubMachineModulesFactoryCfg source)
            {
                if (source == null)
                {
                    LoadStorageDevice = new StorageDeviceFactoryCfg();
                    UnloadStorageDevice = new StorageDeviceFactoryCfg();
                    LoadTransportMechanism = new TransportMechanismFactoryCfg();
                    UnloadTransportMechanism = new TransportMechanismFactoryCfg();
                    FeedingPortFactoryCfg = new FeedPortFactoryCfg();
                    ReceivePortFactoryCfg = new FeedPortFactoryCfg();
                    return;
                }

                LoadStorageDevice = new StorageDeviceFactoryCfg();
                LoadStorageDevice.CopyFrom(source.LoadStorageDevice);
                UnloadStorageDevice = new StorageDeviceFactoryCfg();
                UnloadStorageDevice.CopyFrom(source.UnloadStorageDevice);

                LoadTransportMechanism = new TransportMechanismFactoryCfg();
                LoadTransportMechanism.CopyFrom(source.LoadTransportMechanism);
                UnloadTransportMechanism = new TransportMechanismFactoryCfg();
                UnloadTransportMechanism.CopyFrom(source.UnloadTransportMechanism);

                FeedingPortFactoryCfg = new FeedPortFactoryCfg
                {
                    PortCount = source.FeedingPortFactoryCfg?.PortCount ?? 0
                };
                ReceivePortFactoryCfg = new FeedPortFactoryCfg
                {
                    PortCount = source.ReceivePortFactoryCfg?.PortCount ?? 0
                };
            }

            public byte[] ToBytes()
            {
                return JsonObjConvert.ToJSonBytes(this);
            }

            /// <summary>从字节流恢复整套出厂配置。</summary>
            public void FromBytes(byte[] data)
            {
                if (data != null && data.Length > 0)
                    JsonObjConvert.PopulateObject(data, this);
            }
        }
    }
}
