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
        /// <summary>料盒子模组初始化配置。</summary>
        public class MaterialBoxSubMachineModulesInitCfg
        {
            /// <summary>上料侧储料装置初始化参数，定义 IO、气缸和启停状态等接线信息。</summary>
            public StorageDeviceInitCfg LoadStorageDevice { get; set; } = new StorageDeviceInitCfg();

            /// <summary>下料侧储料装置初始化参数，定义 IO、气缸和启停状态等接线信息。</summary>
            public StorageDeviceInitCfg UnloadStorageDevice { get; set; } = new StorageDeviceInitCfg();

            /// <summary>上料侧运输机构初始化参数，主要用于绑定上料轴资源。</summary>
            public TransportMechanismInitCfg LoadTransportMechanism { get; set; } = new TransportMechanismInitCfg();

            /// <summary>下料侧运输机构初始化参数，主要用于绑定下料轴资源。</summary>
            public TransportMechanismInitCfg UnloadTransportMechanism { get; set; } = new TransportMechanismInitCfg();

            /// <summary>送料口初始化配置参数。</summary>
            public FeedPortInitCfg FeedingPortInitCfg { get; set; } = new FeedPortInitCfg();

            /// <summary>接料口初始化配置参数。</summary>
            public FeedPortInitCfg ReceivePortInitCfg { get; set; } = new FeedPortInitCfg();

            /// <summary>复制初始化配置。/summary>
            public void CopyFrom(MaterialBoxSubMachineModulesInitCfg source)
            {
                if (source == null)
                {
                    LoadStorageDevice = new StorageDeviceInitCfg();
                    UnloadStorageDevice = new StorageDeviceInitCfg();
                    LoadTransportMechanism = new TransportMechanismInitCfg();
                    UnloadTransportMechanism = new TransportMechanismInitCfg();
                    FeedingPortInitCfg = new FeedPortInitCfg();
                    ReceivePortInitCfg = new FeedPortInitCfg();
                    return;
                }

                LoadStorageDevice = source.LoadStorageDevice ?? new StorageDeviceInitCfg();
                UnloadStorageDevice = source.UnloadStorageDevice ?? new StorageDeviceInitCfg();
                LoadTransportMechanism = source.LoadTransportMechanism ?? new TransportMechanismInitCfg();
                UnloadTransportMechanism = source.UnloadTransportMechanism ?? new TransportMechanismInitCfg();
                FeedingPortInitCfg = source.FeedingPortInitCfg ?? new FeedPortInitCfg();
                ReceivePortInitCfg = source.ReceivePortInitCfg ?? new FeedPortInitCfg();
            }

            public byte[] ToBytes()
            {
                return JsonObjConvert.ToJSonBytes(this);
            }

            /// <summary>从字节流恢复整套初始化配置。</summary>
            public void FromBytes(byte[] data)
            {
                if (data != null && data.Length > 0)
                    JsonObjConvert.PopulateObject(data, this);
            }
        }
    }
}
