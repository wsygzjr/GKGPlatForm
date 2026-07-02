using Avalonia.Controls;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.View;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModel;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage
{
	internal class TestPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
	{
		private ICompUIRunTimeCallBack callBack;

		private event EventHandler afterModified;

		private TestRecipeCfgCompUIView view;

		private TestRecipeCfgCompUIViewModel viewModel;

		public TestPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
		{
			this.callBack = callBack;
			view = new TestRecipeCfgCompUIView();
			viewModel = new TestRecipeCfgCompUIViewModel(false, this.callBack);
			viewModel.PropertyChanged += viewModel_PropertyChanged;
			view.DataContext = viewModel;
		}

		private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(viewModel.Text))
			{
				afterModified?.Invoke(sender, null);
			}
		}

		/// <summary>
		/// 编辑查看参数界面，应该从Control继承
		/// </summary>
		object IPageTypeRunTimeCompUIView.View 
		{
			get 
			{
				RemoveViewFromParent();
				return view;
			}
		}

		// 辅助方法：从原父容器移除View（兼容Panel/ContentControl等常见容器）
		private void RemoveViewFromParent()
		{
			if (view == null) return;

			// 1. 处理Panel类容器（如StackPanel、Grid，通过Children移除）
			if (view.Parent is Panel panelParent)
			{
				if (panelParent.Children.Contains(view))
				{
					panelParent.Children.Remove(view);
				}
			}
			// 2. 处理ContentControl类容器（如Button、Border，通过Content清空）
			else if (view.Parent is ContentControl contentParent)
			{
				if (contentParent.Content == view)
				{
					contentParent.Content = null;
				}
			}
		}

		/// <summary>
		/// 信息修改事件
		/// </summary>
		event EventHandler IPageTypeRunTimeCompUIView.AfterModified
		{
			add
			{
				afterModified += value;
			}
			remove
			{
				afterModified -= value;
			}
		}


		/// <summary>
		/// 编辑权限所需的操作管理单元ID列表。通过判断用户权限中是否有该操作管理单元ID决定在界面是否可以进行编辑，
		/// 只要包含其中一个就认为该用户具有该功能的操作权限，否则只有只读权限。null或个数为0表示不控制编辑权限。
		/// </summary>
		OpMngCellID[] IPageTypeRunTimeCompUIView.EditFuncMngCellIDs 
		{
			get 
			{
				return null;
			} 
		}
		/// <summary>
		///  设置是否只读
		/// </summary>
		/// <param name="readOnly">true:只读，false:读写</param>
		void IPageTypeRunTimeCompUIView.SetReadOnly(bool readOnly) 
		{
			viewModel.ReadOnly = readOnly;
		}

		private TestRecipeCfgCompUIModel testRecipeCfgCompUIModel;
		public void SetData(TestRecipeCfgCompUIModel testRecipeCfgCompUIModel) 
		{
            if (testRecipeCfgCompUIModel == null)
                testRecipeCfgCompUIModel = new TestRecipeCfgCompUIModel();

            this.testRecipeCfgCompUIModel = testRecipeCfgCompUIModel;
            viewModel.Text= testRecipeCfgCompUIModel.Text;
		}

		public TestRecipeCfgCompUIModel GetData()
		{
			if (this.testRecipeCfgCompUIModel == null)
				this.testRecipeCfgCompUIModel = new TestRecipeCfgCompUIModel();
			this.testRecipeCfgCompUIModel.Text = viewModel.Text;
			return testRecipeCfgCompUIModel;
		}
	}
}
