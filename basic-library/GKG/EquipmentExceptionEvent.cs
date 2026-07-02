namespace GKG
{
    public class EquipmentExceptionEventArgs : EventArgs
    {
        public EquipmentExceptionEventArgs(int id, string name, int level)
        {
            EventID = id;
            EventName = name;
            EventSeverity = level;
        }
        /// <summary>
        /// 事件ID
        /// </summary>
        public int EventID { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; } = "";
        /// <summary>
        /// 事件级别
        /// </summary>
        public int EventSeverity { get; set; }
    }
}
