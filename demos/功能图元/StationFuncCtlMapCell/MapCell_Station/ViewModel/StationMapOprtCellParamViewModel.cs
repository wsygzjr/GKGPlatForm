using GF_Gereric;
using Newtonsoft.JsonG;
using ReactiveUI;
using System;
using System.Text;

namespace GKG.Map.StationFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 工位图元操作原子参数 ViewModel
    /// </summary>
    public class StationMapOprtCellParamViewModel : ReactiveObject, IDisposable
    {
        private bool _showUnit;
        /// <summary>
        /// 是否显示单位
        /// </summary>
        public bool ShowUnit
        {
            get => _showUnit;
            set => this.RaiseAndSetIfChanged(ref _showUnit, value);
        }

        private string _unit = string.Empty;
        /// <summary>
        /// 具体单位字符串
        /// </summary>
        public string Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        /// <summary>
        /// 从字节数组反序列化并更新属性
        /// </summary>
        public void FromBytes(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                string json = Encoding.UTF8.GetString(data);
                var temp = JsonObjConvert.FromJSon<StationMapOprtCellParamViewModel>(json);
                if (temp != null)
                {
                    this.ShowUnit = temp.ShowUnit;
                    this.Unit = temp.Unit;
                }
            }
        }

        /// <summary>
        /// 序列化为 UTF-8 字节数组
        /// </summary>
        public byte[] ToBytes()
        {
            string json = JsonObjConvert.ToJSon(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public void Dispose()
        {
            // 资源释放
        }
    }
}