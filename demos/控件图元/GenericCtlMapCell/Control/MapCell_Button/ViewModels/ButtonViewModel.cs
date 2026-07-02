using Avalonia;
using Avalonia.Media;
using Griffins.Map.CtlMapCell.Generic.Button;
using Griffins.Map.UI;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.CtlMapCell.Generic.ViewModel
{
	public class ButtonViewModel : ReactiveObject, IDisposable
	{
		private string _buttonName;
		/// <summary>
		/// 按钮名称
		/// </summary>
		public string ButtonName
		{
			get
			{
				return _buttonName;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _buttonName, value);
			}
		}

		private FontInfo _fontInfo;

		public FontInfo TextFont
		{
			get
			{
				return _fontInfo;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _fontInfo, value);
			}
		}

		private Color _textColor;
		/// <summary>
		/// 文本颜色
		/// </summary>
		public Color TextColor
		{
			get
			{
				return _textColor;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _textColor, value);
			}
		}

		private Color _backColor;
		/// <summary>
		/// 背景颜色
		/// </summary>
		public Color BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _backColor, value);
			}
		}

		private BitmapData _backgroundImage;
		/// <summary>
		/// 文本颜色
		/// </summary>
		public BitmapData BackgroundImage
		{
			get
			{
				return _backgroundImage;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _backgroundImage, value);
			}
		}

		private ImageSizeMode _imageSizeMode;
		/// <summary>
		/// 图片定位方式
		/// </summary>
		public ImageSizeMode ImageSizeMode
		{
			get
			{
				return _imageSizeMode;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _imageSizeMode, value);
			}
		}

		public ReactiveCommand<Point, Unit> ButtonClickCommand { get; }

		private Action _clickAction;

		public ButtonViewModel(ButtonPropertyModelEdit buttonPropertyModelEdit, Action clickAction)
		{
			this.ButtonName = buttonPropertyModelEdit.ButtonName;
			this.TextFont = buttonPropertyModelEdit.TextFont;
			this.TextColor = buttonPropertyModelEdit.TextColor;
			this.BackColor = buttonPropertyModelEdit.BackColor;
			this.BackgroundImage = buttonPropertyModelEdit.BackgroundImage;
			this.ImageSizeMode = buttonPropertyModelEdit.ImageSizeMode;
			ButtonClickCommand = ReactiveCommand.Create<Point>(OnButtonClicked);
			_clickAction = clickAction;
		}

		private void OnButtonClicked(Point screenP)
		{
			//System.Diagnostics.Debug.WriteLine(screenP);
			_clickAction?.Invoke();
		}

		public void Dispose()
		{

		}
	}
}
