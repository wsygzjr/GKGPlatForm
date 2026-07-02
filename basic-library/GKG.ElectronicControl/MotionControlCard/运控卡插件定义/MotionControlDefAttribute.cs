using System;

namespace GKG.ElectronicControl
{
    /// <summary>
    /// 运控卡插件定义特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MotionControlDefAttribute : Attribute
    {
        /// <summary>
        /// 运控卡插件种类字符串表示
        /// </summary>
        public const string PLUGINKIND_Str = "{1988A65F-6C27-4F0B-9966-E1042135C46C}";
        /// <summary>
        /// 运控卡插件插件种类
        /// </summary>
        public readonly static Guid PLUGINKIND = new Guid(PLUGINKIND_Str);

         /// <summary>
        /// 运控卡插件属性构造函数
		/// </summary>
        /// <param name="motionControlIDStr">运控卡ID（Guid的字符串表示）</param>
        public MotionControlDefAttribute(string motionControlIDStr)
		{
            motionControlID=new Guid(motionControlIDStr);

        }

        private Guid motionControlID =Guid.Empty;
        /// <summary>
        /// 运控卡ID
        /// </summary>
        public Guid MotionControlID
        {
            get { return motionControlID; }
        }
    }
}
