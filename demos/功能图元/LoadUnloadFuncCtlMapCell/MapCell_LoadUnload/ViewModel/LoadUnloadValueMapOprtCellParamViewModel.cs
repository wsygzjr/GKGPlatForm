using GF_Gereric;
using Newtonsoft.JsonG;
using ReactiveUI;
using System;
using System.Text;

namespace GKG.Map.LoadUnloadFuncCtlMapCell.ViewModels
{
    /// <summary>
    /// 上下料图元操作原子参数视图模型 (ViewModel)
    /// 负责承载和管理操作原子的附加配置参数（如单位显示等），并提供序列化/反序列化能力。
    /// </summary>
    public class LoadUnloadValueMapOprtCellParamViewModel : ReactiveObject, IDisposable
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
        /// 从字节数组反序列化并无损更新当前属性
        /// </summary>
        /// <param name="data">外部传入的 UTF-8 编码字节流</param>
        public void FromBytes(byte[] data)
        {
            // 防御拦截：空数据直接退出
            if (data == null || data.Length == 0) return;

            try
            {
                string json = Encoding.UTF8.GetString(data);

                var temp = JsonObjConvert.FromJSon<LoadUnloadValueMapOprtCellParamViewModel>(json);

                if (temp != null)
                {
                    this.ShowUnit = temp.ShowUnit;
                    this.Unit = temp.Unit;
                }
            }
            catch (Exception ex)
            {
                // 健壮性防御：反序列化失败时静默拦截并输出日志，避免引发整个参数面板或加载过程崩溃
                System.Diagnostics.Debug.WriteLine($"操作原子参数反序列化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 序列化为 UTF-8 字节数组，供底层框架持久化存储
        /// </summary>
        public byte[] ToBytes()
        {
            string json = JsonObjConvert.ToJSon(this);
            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 当前暂无非托管资源或强引用事件需要释放
        }
    }
}