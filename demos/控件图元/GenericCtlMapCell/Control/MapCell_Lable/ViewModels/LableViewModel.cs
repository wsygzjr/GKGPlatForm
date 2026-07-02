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

namespace Griffins.Map.CtlMapCell.Generic.Lable.ViewModel
{
	public class LableViewModel : ReactiveObject, IDisposable
	{
		private string _lableText;
		/// <summary>
		/// Lable文本
		/// </summary>
		public string LableText
		{
			get
			{
				return _lableText;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _lableText, value);
			}
		}

		private Color _lableColor;
		/// <summary>
		/// 标签颜色
		/// </summary>
		public Color LableColor
		{
			get
			{
				return _lableColor;
			}
			set
			{
				this.RaiseAndSetIfChanged(ref _lableColor, value);
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

		private bool _designTime;

		private LablePropertyModelEdit _lablePropertyModelEdit;

		public LableViewModel(bool designTime, LablePropertyModelEdit lablePropertyModelEdit)
		{
			this._designTime = designTime;
			this._lablePropertyModelEdit = lablePropertyModelEdit;
			this._lableText = lablePropertyModelEdit.LableValue;
			this._lableColor = lablePropertyModelEdit.LableColor;
			this._backColor = lablePropertyModelEdit.BackColor;
		}

		public void Dispose()
		{

		}
	}
}
