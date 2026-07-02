using Griffins;
using Griffins.PF;
using System;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// IconButton 组内互斥消息。
    /// 平台广播这条消息后，同组按钮根据 GroupId 和 MapObjID 判断自己是否处于组选中态。
    /// </summary>
    [Serializable]
    public class MutualInfo_IconButtonGroupSelected : MutualInfoBase
    {
        /// <summary>
        /// 互斥消息种类ID。
        /// </summary>
        public static readonly GriffinsInfoKindID InfoKindID = new GriffinsInfoKindID("{A5427D44-7D6F-4E46-8D76-0FC7F9A91A51}");

        /// <summary>
        /// 创建空消息实例，供反序列化使用。
        /// </summary>
        public MutualInfo_IconButtonGroupSelected()
        {
        }

        /// <summary>
        /// 创建组互斥消息。
        /// </summary>
        public MutualInfo_IconButtonGroupSelected(string groupId, string mapObjId)
        {
            GroupId = groupId ?? string.Empty;
            MapObjID = mapObjId ?? string.Empty;
        }

        /// <summary>
        /// 按钮所属组ID。
        /// </summary>
        public string GroupId { get; set; } = string.Empty;

        /// <summary>
        /// 当前被选中的图元ID。
        /// </summary>
        public string MapObjID { get; set; } = string.Empty;
    }
}
