using Avalonia;
using Avalonia.Media;
using Griffins.Map.CtlMapCell.Generic.Button;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.CtlMapCell.Generic.TextButton.ViewModel
{
	public class TextButtonViewModel : ReactiveObject, IDisposable
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

		private Color _textColor;
		/// <summary>
		/// 按钮文本颜色
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
		/// 按钮背景颜色
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

		private string _textValue;
		/// <summary>
		/// 输入文本框文本值
		/// </summary>
		public string TextValue
		{
			get
			{
				return _textValue;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _textValue, value);
				//不是设计时修改M对象的属性值
				if (!_designTime) 
				{
					_textButtonPropertyModelEdit.IsRuning = false;
					_textButtonPropertyModelEdit.TextValue = value;
				}
			}
		}

		private double _columnSpacing;
		/// <summary>
		/// 列间距
		/// </summary>
		public double ColumnSpacing
		{
			get
			{
				return _columnSpacing;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _columnSpacing, value);
			}
		}

		public ReactiveCommand<Point, Unit> ButtonClickCommand { get; }

		private Action _clickAction;

		private bool _designTime;

		private TextButtonPropertyModelEdit _textButtonPropertyModelEdit;

		public TextButtonViewModel(bool designTime, TextButtonPropertyModelEdit textButtonPropertyModelEdit, Action clickAction)
		{
			this._designTime = designTime;
			this._textButtonPropertyModelEdit= textButtonPropertyModelEdit;
			this._buttonName = textButtonPropertyModelEdit.ButtonName;
			this._textColor = textButtonPropertyModelEdit.TextColor;
			this._backColor = textButtonPropertyModelEdit.BackColor;
			this._textValue = textButtonPropertyModelEdit.TextValue;
			this._columnSpacing = textButtonPropertyModelEdit.ColumnSpacing;
			ButtonClickCommand = ReactiveCommand.Create<Point>(OnButtonClicked);
			_clickAction = clickAction;
		}

		private void OnButtonClicked(Point screenP)
		{
			_clickAction?.Invoke();
		}

		public void Dispose()
		{

		}
	}
}
