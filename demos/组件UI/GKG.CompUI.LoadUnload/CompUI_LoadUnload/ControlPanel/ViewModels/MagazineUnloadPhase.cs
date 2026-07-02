namespace GKG.CompUI.LoadUnload.ControlPanel.ViewModels
{
    /// <summary>
    /// 一键下料：单料盒作业阶段
    /// </summary>
    public enum MagazineUnloadPhase
    {
        /// <summary>到位且夹紧，可开始下料（松开气缸）</summary>
        Ready,

        /// <summary>已松开，等待操作员确认下料完成（夹紧气缸）</summary>
        AwaitingConfirm,

        /// <summary>本料盒下料流程已结束</summary>
        Finished
    }
}
