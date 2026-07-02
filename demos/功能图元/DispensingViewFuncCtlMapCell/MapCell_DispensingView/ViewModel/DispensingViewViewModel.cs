using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;

namespace GKG.Map.DispensingViewFuncCtlMapCell.ViewModels
{
    public class DispensePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsDispensing { get; set; }

        public DispensePoint(double x, double y, bool isDispensing = true)
        {
            X = x; Y = y; IsDispensing = isDispensing;
        }
    }

    public class DispensingViewModel : ReactiveObject, IDisposable
    {
        [Reactive]
        public double CurrentX { get; set; }

        [Reactive]
        public double CurrentY { get; set; }

        public ObservableCollection<DispensePoint> TrajectoryPoints { get; } = new();

        public ReactiveCommand<Unit, Unit> ClearCommand { get; }

        public DispensingViewModel()
        {
            ClearCommand = ReactiveCommand.Create(() =>
            {
                TrajectoryPoints.Clear();
                CurrentX = 0;
                CurrentY = 0;
            });

            // 启动时加载一组静态的演示轨迹，避免界面纯白
            LoadStaticDemoData();
        }

        public void AppendRealtimePoint(double x, double y)
        {
            CurrentX = Math.Round(x, 2);
            CurrentY = Math.Round(y, 2);
            TrajectoryPoints.Add(new DispensePoint(CurrentX, CurrentY));
        }

        /// <summary>
        /// 生成静态模拟轨迹数据用于界面预览
        /// </summary>
        private void LoadStaticDemoData()
        {
            // 1. 画一个工业风的蛇形涂覆轨迹
            double startX = -80, startY = -20;
            for (int row = 0; row < 3; row++)
            {
                double endX = (row % 2 == 0) ? 80 : -80;
                // 横向打点
                for (double x = startX; (startX < endX ? x <= endX : x >= endX); x += (startX < endX ? 5 : -5))
                {
                    AppendRealtimePoint(x, startY);
                }
                startX = endX;
                // 向下换行
                for (double y = startY; y <= startY + 15; y += 3)
                {
                    AppendRealtimePoint(startX, y);
                }
                startY += 15;
            }

            // 2. 画一个闭合点胶圆环
            double radius = 25;
            double centerX = 0;
            double centerY = startY + radius + 10;

            for (int angle = 0; angle <= 360; angle += 8)
            {
                double rad = angle * Math.PI / 180.0;
                double x = centerX + radius * Math.Cos(rad);
                double y = centerY + radius * Math.Sin(rad);
                AppendRealtimePoint(x, y);
            }
        }

        public void Dispose() { }
    }
}