namespace GKG.CompUI.LoadUnload.ControlPanel.ViewModels
{
    /// <summary>
    /// 一键上料：单料盒作业阶段
    /// </summary>
    public enum MagazineLoadPhase
    {
        /// <summary>到位且夹紧，可开始上料（松开气缸）</summary>
        Ready,

        /// <summary>已松开，等待操作员确认上料完成（夹紧气缸）</summary>
        AwaitingConfirm,

        /// <summary>本料盒上料流程已结束</summary>
        Finished
    }
}
