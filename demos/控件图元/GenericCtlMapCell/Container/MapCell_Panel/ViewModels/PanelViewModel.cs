using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.CtlMapCell.Generic.Container.ViewModel
{
	public class PanelViewModel : ReactiveObject, IDisposable
	{
		private bool _showBorder;

		/// <summary>
		/// 显示边框
		/// </summary>
		public bool ShowBorder
		{
			get
			{
				return _showBorder;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _showBorder, value);
			}
		}

		private string _compTypeID;
		/// <summary>
		/// 组件类型
		/// </summary>
		public string CompTypeID
		{
			get
			{
				return _compTypeID;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _compTypeID, value);
			}
		}

		private string _alias;
		/// <summary>
		/// 别名
		/// </summary>
		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _alias, value);
			}
		}

		private string _viewID;
		/// <summary>
		/// 界面ID
		/// </summary>
		public string ViewID
		{
			get
			{
				return _viewID;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _viewID, value);
			}
		}

		private string _pageTypeID;
		/// <summary>
		/// 页面类型ID
		/// </summary>
		public string PageTypeID
		{
			get
			{
				return _pageTypeID;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _pageTypeID, value);
			}
		}

		private bool _designTime;

		private PanelPropertyModelEdit _panelPropertyModelEdit;

		public PanelViewModel(bool designTime, PanelPropertyModelEdit panelPropertyModelEdit)
		{
			this._designTime = designTime;
			this._panelPropertyModelEdit= panelPropertyModelEdit;
			this._compTypeID = panelPropertyModelEdit.CompTypeID;
			this._alias = panelPropertyModelEdit.Alias;
			this._viewID = panelPropertyModelEdit.ViewID;
			this._pageTypeID = panelPropertyModelEdit.PageTypeID;
		}

		public void Dispose()
		{

		}
	}
}
