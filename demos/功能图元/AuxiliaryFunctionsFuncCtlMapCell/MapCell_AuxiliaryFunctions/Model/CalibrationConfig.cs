using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.Model
{
    /// <summary>
    /// 定义一个机台校正功能的配置类
    /// </summary>
    public class CalibrationConfig
    {
        public bool HasOutGlue { get; set; } = true;      // 是否有排胶
        public bool HasLaser { get; set; } = true;        // 是否有激光测高
        public bool HasPreciseTeach { get; set; } = true; // 是否有精确校准
        public bool HasObliqueTeach { get; set; } = true; // 是否有倾斜校准
        public bool HasOnePointWeight { get; set; } = true;// 是否有单点重校正
    }
}

