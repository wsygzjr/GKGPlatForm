using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModel
{
	public class TestRecipeCfgCompUIViewModel : ReactiveObject, IDisposable
	{
		private bool isDesign;

		private ICompUIRunTimeCallBack ICompUIRunTimeCallBack;

		private object _viewTag;
		/// <summary>
		/// 对应View的Tag属性（支持双向绑定）
		/// </summary>
		public object ViewTag
		{
			get => _viewTag;
			set => this.RaiseAndSetIfChanged(ref _viewTag, value);
		}

		private string _text;
		/// <summary>
		/// 文本值
		/// </summary>
		public string Text
		{
			get => _text;
			set => this.RaiseAndSetIfChanged(ref _text, value);
		}

		private bool _readOnly;
		/// <summary>
		/// 只读
		/// </summary>
		public bool ReadOnly
		{
			get => _readOnly;
			set => this.RaiseAndSetIfChanged(ref _readOnly, value);
		}

		public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }

		public TestRecipeCfgCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
		{
			this.isDesign = isDesign;
			this.ICompUIRunTimeCallBack = callBack;
			ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);
		}

		private void OnButtonClicked()
		{
			if (isDesign) 
			{
				return;
            }

            var response = ICompUIRunTimeCallBack.ExecConfigSvrCtlCmd("Test", null);
        }

		public void Dispose()
		{

		}
	}
}
