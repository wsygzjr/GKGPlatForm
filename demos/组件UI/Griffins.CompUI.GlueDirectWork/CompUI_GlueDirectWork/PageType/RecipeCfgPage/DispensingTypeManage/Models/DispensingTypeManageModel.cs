using System;
using System.Collections.Generic;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.Models
{
    [Serializable]
    internal sealed class DispensingTypeManageModel
    {
        public string MachineDispensingTypeName { get; set; } = string.Empty;

        public List<DispensingTypeItemModel> DispensingTypes { get; set; } = new();
    }

    [Serializable]
    internal sealed class DispensingTypeItemModel
    {
        public int SerialNumber { get; set; }

        public string DispensingName { get; set; } = string.Empty;
    }
}
