namespace GKG.CompUI.LoadUnload.ControlPanel.ViewModels
{
    /// <summary>
    /// 一键抽检：单料盒作业阶段
    /// </summary>
    public enum MagazineInspectPhase
    {
        /// <summary>可开始抽检（松开气缸）</summary>
        Ready,

        /// <summary>已松开，等待操作员确认抽检完成（夹紧气缸）</summary>
        AwaitingConfirm,

        /// <summary>本料盒抽检流程已结束</summary>
        Finished
    }
}
