using System;
using System.Collections.Generic;
using System.Text;
using GF_Gereric;

namespace Griffins.Map.CtlMapCell.Generic.ComboBox
{
    /// <summary>
    /// 值名称编码
    /// </summary>
    [Serializable]
    public class DecimalValueName
    {
        private decimal val;
        /// <summary>
        /// 值
        /// </summary>
        public decimal Val
        {
            get { return val; }
            set { val = value; }
        }

        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
    /// <summary>
    /// 值名称编码表
    /// </summary>
    [Serializable]
    public class DecamalValueNameList : List<DecimalValueName>
    {
        /// <summary>
        /// 转换为字节流
        /// </summary>
        /// <returns></returns>
        public byte[] ToByte()
        {
            ByteArrayWriter bw = new ByteArrayWriter();
            bw.Write(this.Count);
            foreach (DecimalValueName valueName in this)
            {
                bw.Write(valueName.Val);

                if (valueName.Name == null)
                    bw.Write((int)0);
                else
                {
                    bw.Write(valueName.Name.Length);
                    bw.Write(valueName.Name);
                }
            }
            return bw.ToBytes();
        }

        /// <summary>
        /// 从字节流加载
        /// </summary>
        /// <param name="data"></param>
        public void LoadFromBytes(byte[] data)
        {
            this.Clear();
            if ((data == null) || (data.Length == 0))
                return;
            ByteArrayReader br = new ByteArrayReader(data);
            int c = br.ReadInt32();
            for (int i = 0; i < c; i++)
            {
                DecimalValueName valueName = new DecimalValueName();
                valueName.Val = br.ReadDecimal();
                int nameLen = br.ReadInt32();
                if (nameLen == 0)
                    valueName.Name = "";
                else
                    valueName.Name = br.ReadString();
                this.Add(valueName);
            }
        }

    }
}
