using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using GKG.Map.DispensingViewFuncCtlMapCell.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.DispensingViewFuncCtlMapCell.Views
{
    public partial class DispensingView : ReactiveUserControl<DispensingViewModel>
    {
        private List<Point> _renderPoints = new List<Point>();

        public DispensingView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .WhereNotNull()
                    .Subscribe(vm =>
                    {
                        // 【新增逻辑】：组件加载时，如果 VM 中已经有静态点数据，先将它们绘制出来
                        if (vm.TrajectoryPoints.Count > 0)
                        {
                            _renderPoints.Clear();
                            foreach (var pt in vm.TrajectoryPoints)
                            {
                                double screenX = pt.X * 2.5 + 300;
                                double screenY = pt.Y * 2.5 + 120;
                                _renderPoints.Add(new Point(screenX, screenY));
                            }
                            UpdatePolyline();
                        }

                        // 订阅后续的集合变化事件（真实机器点胶或清空动作）
                        vm.TrajectoryPoints.CollectionChanged += OnTrajectoryCollectionChanged;

                        Disposable.Create(() =>
                        {
                            vm.TrajectoryPoints.CollectionChanged -= OnTrajectoryCollectionChanged;
                        }).DisposeWith(disposables);
                    })
                    .DisposeWith(disposables);
            });
        }

        private void OnTrajectoryCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                        {
                            foreach (DispensePoint newPoint in e.NewItems)
                            {
                                double scale = 2.5;
                                double offsetX = 300;
                                double offsetY = 120;

                                double screenX = newPoint.X * scale + offsetX;
                                double screenY = newPoint.Y * scale + offsetY;

                                _renderPoints.Add(new Point(screenX, screenY));
                            }
                            UpdatePolyline();
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        _renderPoints.Clear();
                        UpdatePolyline();
                        break;
                }
            });
        }

        private void UpdatePolyline()
        {
            TrajectoryLine.Points = new List<Point>(_renderPoints);
        }
    }
}