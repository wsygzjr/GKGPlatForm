using GKG;
using System;
using System.Collections.Generic;

namespace GKG.SubMM.StorageDeviceModule
{
    /// <summary>储料装置内部机构的初始化参数。</summary>
    public class StorageMechanismInitCfg
    {
        public StorageMechanismInitCfg()
        {
            SenserIOGuids = new List<Guid>();
            CylinderInitParameters = new List<CylinderInitParameters>();
            IsFeeding = true;
        }
        /// <summary>用于检测料盒存在等状态的传感器 IO 列表，按储料位索引对应。</summary>
        public List<Guid> SenserIOGuids { get; set; } = new List<Guid>();

        /// <summary>夹料气缸初始化参数列表，运行时按气缸索引操作。</summary>
        public List<CylinderInitParameters> CylinderInitParameters { get; set; } = new List<CylinderInitParameters>();

        public bool IsFeeding { get; set; } = true;

        /// <summary>按值复制机构初始化参数。</summary>
        public void CopyFrom(StorageMechanismInitCfg source)
        {
            if (source == null)
            {
                SenserIOGuids = new List<Guid>();
                CylinderInitParameters = new List<CylinderInitParameters>();
                return;
            }

            SenserIOGuids = source.SenserIOGuids != null ? new List<Guid>(source.SenserIOGuids) : new List<Guid>();
            CylinderInitParameters = source.CylinderInitParameters != null ? new List<CylinderInitParameters>(source.CylinderInitParameters) : new List<CylinderInitParameters>();
            IsFeeding = source.IsFeeding;
        }
    }
}
