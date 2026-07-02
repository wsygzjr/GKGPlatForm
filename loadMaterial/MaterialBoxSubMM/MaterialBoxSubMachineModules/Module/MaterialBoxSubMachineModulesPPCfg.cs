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
        /// <summary>料盒子模组配方配置。</summary>
        public class MaterialBoxSubMachineModulesPPCfg
        {
            /// <summary>上料侧储料装置配方参数，定义槽位位置、数量和启用状态。</summary>
            public StorageDevicePPCfg LoadStorageDevice { get; set; } = new StorageDevicePPCfg();

            /// <summary>下料侧储料装置配方参数，定义槽位位置、数量和启用状态。</summary>
            public StorageDevicePPCfg UnloadStorageDevice { get; set; } = new StorageDevicePPCfg();

            /// <summary>上料侧运输机构配方参数，定义移动轨迹和步距策略。</summary>
            public TransportMechanismPPCfg LoadTransportMechanism { get; set; } = new TransportMechanismPPCfg();

            /// <summary>下料侧运输机构配方参数，定义移动轨迹和步距策略。</summary>
            public TransportMechanismPPCfg UnloadTransportMechanism { get; set; } = new TransportMechanismPPCfg();

            /// <summary>送料口配方配置参数。</summary>
            public FeedPortPPCfg FeedingPortPPCfg { get; set; } = new FeedPortPPCfg();

            /// <summary>接料口配方配置参数。</summary>
            public FeedPortPPCfg ReceivePortPPCfg { get; set; } = new FeedPortPPCfg();

            /// <summary>按值复制整套配方配置。</summary>
            public void CopyFrom(MaterialBoxSubMachineModulesPPCfg source)
            {
                if (source == null)
                {
                    LoadStorageDevice = new StorageDevicePPCfg();
                    UnloadStorageDevice = new StorageDevicePPCfg();
                    LoadTransportMechanism = new TransportMechanismPPCfg();
                    UnloadTransportMechanism = new TransportMechanismPPCfg();
                    FeedingPortPPCfg = new FeedPortPPCfg();
                    ReceivePortPPCfg = new FeedPortPPCfg();
                    return;
                }

                LoadStorageDevice = new StorageDevicePPCfg();
                LoadStorageDevice.CopyFrom(source.LoadStorageDevice);
                UnloadStorageDevice = new StorageDevicePPCfg();
                UnloadStorageDevice.CopyFrom(source.UnloadStorageDevice);

                LoadTransportMechanism = new TransportMechanismPPCfg();
                LoadTransportMechanism.CopyFrom(source.LoadTransportMechanism);
                UnloadTransportMechanism = new TransportMechanismPPCfg();
                UnloadTransportMechanism.CopyFrom(source.UnloadTransportMechanism);

                FeedingPortPPCfg = new FeedPortPPCfg();
                FeedingPortPPCfg.CopyFrom(source.FeedingPortPPCfg);
                ReceivePortPPCfg = new FeedPortPPCfg();
                ReceivePortPPCfg.CopyFrom(source.ReceivePortPPCfg);
            }

            public byte[] ToBytes()
            {
                return JsonObjConvert.ToJSonBytes(this);
            }

            /// <summary>从字节流恢复整套配方配置。</summary>
            public void FromBytes(byte[] data)
            {
                if (data != null && data.Length > 0)
                    JsonObjConvert.PopulateObject(data, this);
            }
        }
    }
}
