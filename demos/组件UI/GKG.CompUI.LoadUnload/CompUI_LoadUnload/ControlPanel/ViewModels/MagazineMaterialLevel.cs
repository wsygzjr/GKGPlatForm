namespace GKG.CompUI.LoadUnload.ControlPanel.ViewModels
{
    /// <summary>
    /// 料盒级物料汇总状态（对应截图中的状态徽章）
    /// </summary>
    public enum MagazineMaterialLevel
    {
        /// <summary>全部槽位满料</summary>
        Full,

        /// <summary>部分有料</summary>
        HasMaterial,

        /// <summary>存在物料报警槽位</summary>
        MaterialAlarm,

        /// <summary>全部空料</summary>
        Empty
    }
}
