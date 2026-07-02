namespace GKG.SubMM
{
    /// <summary>
    /// 推杆轴状态变更通知参数。
    /// </summary>
    public class PushRodAxisStatus
    {
        /// <summary>
        /// 状态码；0 表示轴位置已变化，前端需主动查询当前坐标。
        /// </summary>
        public int Staus { get; set; }

        /// <summary>
        /// 当前推杆轴坐标；与位置变更通知一并下发，前端可直接刷新显示。
        /// </summary>
        public double Position { get; set; }
    }
}
