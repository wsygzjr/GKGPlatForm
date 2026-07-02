using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Griffins.Map.CtlMapCell.Generic.Container.ViewModel;
using Griffins.Map.UI;
using Griffins.PF.RichClient;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.Map.CtlMapCell.Generic.Container.View;

public partial class PanelView : ReactiveUserControl<PanelViewModel>, IDisposable
{
	private bool designTime;

	private string mapCellName;

	private Func<IControlMapCellCallBack> callBack;

	private Control comUIControl = null;

	public PanelView(bool designTime, string mapCellName, Func<IControlMapCellCallBack> callBack)
	{
		InitializeComponent();
		this.designTime = designTime;
		this.mapCellName = mapCellName;
		this.callBack = callBack;
		this.WhenActivated(disposables =>
		{
			if (ViewModel == null) return;

			// 监听 ViewModel 所有关键属性变化（强类型访问，无需转换）
			this.WhenAnyValue(
					v => v.ViewModel.CompTypeID,
					v => v.ViewModel.Alias,
					v => v.ViewModel.ViewID,
					v => v.ViewModel.PageTypeID
				)
				// 过滤空ViewModel（避免异常）
				.Where(_ => ViewModel != null)
				// 触发面板更新
				.Subscribe(ViewModel => UpdateCustomPanel())
				// 自动管理订阅生命周期（添加到disposables）
				.DisposeWith(disposables);

			// 初始加载时执行一次（若ViewModel已带全属性）
			//UpdateCustomPanel();
		});
		this.Unloaded += OnUnLoaded;
	}

	private void OnUnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		CustomPanel.Children.Clear();
	}

	/// <summary>
	/// 验证属性有效性并更新CustomPanel内容（核心逻辑）
	/// </summary>
	private void UpdateCustomPanel()
	{
		// ViewModel为空或属性不全 → 清空面板
		if (ViewModel == null || !AreAllPropertiesValid())
		{
			CustomPanel.Children.Clear();
			TextBlock textBlock = new TextBlock();
			textBlock.Text = mapCellName;
			textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
			textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
			CustomPanel.Children.Add(textBlock);
			return;
		}

		try
		{
			if (!this.designTime)
			{
				if (comUIControl == null)
					comUIControl = CreateTargetUserControl();
			}
			else
			{
				comUIControl = CreateTargetUserControl();
			}
			// 2. 刷新面板（清空旧控件，添加新控件）
			CustomPanel.Children.Clear();
			CustomPanel.Children.Add(comUIControl);

			// 3. 确保控件填充面板
			comUIControl.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
			comUIControl.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"动态加载UserControl失败：{ex.Message}");
			CustomPanel.Children.Clear();
		}
	}

	/// <summary>
	/// 根据ViewModel属性创建目标UserControl（核心映射逻辑）
	/// </summary>
	private Control CreateTargetUserControl()
	{
		object responseData = ClientInnerInfoSender.ExecCliInnerCommand(MIMI_GetCompUIViewCommand.InnerCommandKindID, "", designTime, PageTypeID.Parse(ViewModel.PageTypeID), CompTypeID.Parse(ViewModel.CompTypeID), CompInstanceID.Parse(ViewModel.Alias), ViewModel.ViewID);
		if (responseData == null)
		{
			return new TextBlock() { Text = "内部命令执行结果为空" };
		}
		Control control = (responseData as Control);
		if (control != null)
		{
			control.Tag = ViewModel.Alias;
		}
		return control;
	}

	/// <summary>
	/// 验证ViewModel所有关键属性是否有效（可根据业务修改）
	/// </summary>
	private bool AreAllPropertiesValid()
	{
		return  !string.IsNullOrWhiteSpace(ViewModel.CompTypeID) 
			   && !string.IsNullOrWhiteSpace(ViewModel.PageTypeID)
			   && !string.IsNullOrWhiteSpace(ViewModel.Alias)
			   && !string.IsNullOrWhiteSpace(ViewModel.ViewID); // ViewID非空
	}

	// 资源释放
	private readonly CompositeDisposable _disposables = new();
	public void Dispose() 
	{ 
		_disposables.Dispose();
		for (int index = CustomPanel.Children.Count - 1; index >= 0; index--) 
		{
			Control control = CustomPanel.Children[index];
			control.DataContext = null;
			CustomPanel.Children.RemoveAt(index);
		}
		CustomPanel.Children.Clear(); // 释放子控件资源
		GC.SuppressFinalize(this);
		this.callBack = null;
	}

}