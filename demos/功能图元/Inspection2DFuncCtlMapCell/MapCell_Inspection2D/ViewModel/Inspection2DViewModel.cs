using System;
using System.Collections.ObjectModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GKG.Map.Inspection2DFuncCtlMapCell.ViewModels
{
    /// <summary>
    /// 单个2D检测项的视图模型
    /// </summary>
    public class InspectionItemViewModel : ReactiveObject
    {
        public string Title { get; set; } = string.Empty;
        public string? ImagePath { get; set; } // 实际开发中可改为 Bitmap 对象
        public bool IsOk { get; set; }
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// 2D检测图元主视图模型
    /// </summary>
    public class Inspection2DViewModel : ReactiveObject, IDisposable
    {
        // 使用 Fody 自动生成属性通知
        [Reactive] public ObservableCollection<InspectionItemViewModel> InspectionList { get; set; }
        [Reactive] public InspectionItemViewModel? SelectedItem { get; set; }

        public Inspection2DViewModel()
        {
            InspectionList = new ObservableCollection<InspectionItemViewModel>();

            // 模拟初始化数据
            LoadMockData();

            // 默认选中第一条记录
            if (InspectionList.Count > 0)
            {
                SelectedItem = InspectionList[0];
            }
        }

        private void LoadMockData()
        {
            InspectionList.Add(new InspectionItemViewModel
            {
                Title = "点胶轨迹_01",
                IsOk = true,
                Details = "胶宽: 0.45mm, 正常",
                Timestamp = DateTime.Now
            });
            InspectionList.Add(new InspectionItemViewModel
            {
                Title = "点胶轨迹_02",
                IsOk = true,
                Details = "胶宽: 0.48mm, 正常",
                Timestamp = DateTime.Now.AddSeconds(-2)
            });
            InspectionList.Add(new InspectionItemViewModel
            {
                Title = "点胶轨迹_03",
                IsOk = false,
                Details = "缺陷: 断胶 (缺口长 1.2mm)",
                Timestamp = DateTime.Now.AddSeconds(-4)
            });
            InspectionList.Add(new InspectionItemViewModel
            {
                Title = "点胶轨迹_04",
                IsOk = true,
                Details = "无溢胶, 正常",
                Timestamp = DateTime.Now.AddSeconds(-6)
            });
        }

        public void Dispose() { }
        
    }
}