using Avalonia.Media;
using Avalonia.Media.Imaging;
using GF_Gereric;
using Griffins;
using Griffins.Map.CtlMapCell.Generic.Container.View;
using Griffins.Map.CtlMapCell.Generic.Container.ViewModel;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Griffins.Map.CtlMapCell.Generic.Container
{
    class MapCellPanelCtlObj : ControlCellBase
    {
        private PanelView view;

		private PanelViewModel panelViewModel;
		static MapCellPanelCtlObj()
        {

        }
        public MapCellPanelCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

		public MapCellPanelCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
		{
			PropertyEditModelBase = CreatePropertyModelEditBase();
			PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
			EventBindEditModel = CreateEventBindEditModel();
			base.SetID(mapCellID);
			base.SetName(mapCellName);
			view = new PanelView(designTime, mapCellName, new Func<IControlMapCellCallBack>(() => { return CallBack; }));
			RegisterProperty(new MapObjPropertyInfo(nameof(PanelPropertyModelEdit.ShowBorder), ResourceA.ShowBorder, GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), false, true, new GriffinsBaseValue(false)));
			RegisterProperty(new MapObjPropertyInfo(nameof(PanelPropertyModelEdit.CompTypeID), ResourceA.CompTypeID, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, null));
			RegisterProperty(new MapObjPropertyInfo(nameof(PanelPropertyModelEdit.Alias), ResourceA.Alias, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, null));
			RegisterProperty(new MapObjPropertyInfo(nameof(PanelPropertyModelEdit.ViewID), ResourceA.ViewID, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, null));
			RegisterProperty(new MapObjPropertyInfo(nameof(PanelPropertyModelEdit.PageTypeID), ResourceA.PageTypeID, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, null));
			RegisterOprtCellInfo(new MapOprtCellInfo(PamelMapOprtCellConst.ShowBorder_MapOprtCellID, ResourceA.ShowBorder_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(PamelMapOprtCellConst.CompTypeID_MapOprtCellID, ResourceA.CompTypeID_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(PamelMapOprtCellConst.Alias_MapOprtCellID, ResourceA.Alias_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(PamelMapOprtCellConst.ViewID_MapOprtCellID, ResourceA.ViewID_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(PamelMapOprtCellConst.PageTypeID_MapOprtCellID, ResourceA.PageTypeID_MapOprtCellName));
			RegisterOprtInfo(new MapOprtInfo(nameof(PanelPropertyModelEdit.ShowBorder), ResourceA.ShowBorder_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=PamelMapOprtCellConst.ShowBorder_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(PanelPropertyModelEdit.CompTypeID), ResourceA.CompTypeID_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=PamelMapOprtCellConst.CompTypeID_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(PanelPropertyModelEdit.Alias), ResourceA.Alias_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=PamelMapOprtCellConst.Alias_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(PanelPropertyModelEdit.ViewID), ResourceA.ViewID_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=PamelMapOprtCellConst.ViewID_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(PanelPropertyModelEdit.PageTypeID), ResourceA.PageTypeID_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=PamelMapOprtCellConst.PageTypeID_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			(this as IMapCellTypeBase).Name = ResourceA.PanelContainer;
			panelViewModel = new PanelViewModel(designTime, PanelPropertyModelEdit);
			view.DataContext = panelViewModel;
		}

		[Browsable(false)]
		public PanelPropertyModelEdit PanelPropertyModelEdit
		{
			get { return PropertyEditModelBase as PanelPropertyModelEdit; }
		}

		public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
		{
			PanelPropertyModelEdit.IsRuning = isRuning;
			return PanelPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
		}

        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);			
			var propertyEditModelBase = JsonObjConvert.FromJSon<PanelPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
			(PropertyEditModelBase as PanelPropertyModelEdit).CopyFrom(propertyEditModelBase);
		}

        /// <summary>
        /// 当把画图信息写入到字节流中（必须先调用基类的OnWriteDrawInfoToBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="bw">字节流写入对象</param>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
			bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
		}

        protected override void OnCopyFrom(ControlCellBase source)
        {
			MapCellPanelCtlObj mapCellPanelCtlObj = (source as MapCellPanelCtlObj);
			base._CopyFrom(mapCellPanelCtlObj);
			(PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
		}

        /// <summary>
        /// 初始化时
        /// </summary>
        protected override void OnInit()
        {

        }

		public override void OnDispose()
		{
			base.OnDispose();
			if (view is IDisposable iDisposable) 
			{
				iDisposable.Dispose();
			}
		}

        protected override object OnGetView()
        {
            return view;
        }

		protected override object OnGetViewModel()
		{
			return panelViewModel;
		}

		/// <summary>
		/// 创建图元属性编辑模型对象
		/// </summary>
		/// <returns>图元属性编辑模型对象</returns>
		public override PropertyEditModelBase CreatePropertyModelEditBase()
		{
			return new PanelPropertyModelEdit();
		}

		/// <summary>
		/// 创建图元属性绑定编辑模型对象
		/// </summary>
		/// <returns>图元属性绑定编辑模型对象</returns>
		public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
		{
			return  null;
		}

		/// <summary>
		/// 创建图元事件绑定编辑模型对象
		/// </summary>
		/// <returns>图元事件绑定编辑模型对象</returns>
		public override EventBindEditModel CreateEventBindEditModel()
		{
			return null;
		}

		/// <summary>
		/// 执行图元内部操作原子
		/// </summary>
		/// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
		/// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
		protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
		{
			if (mapOprtCellInstInfo.OprtCellID == PamelMapOprtCellConst.ShowBorder_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new ShowBorderMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == PamelMapOprtCellConst.CompTypeID_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new CompTypeIDMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == PamelMapOprtCellConst.ViewID_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new ViewIDMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == PamelMapOprtCellConst.Alias_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new AliasMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == PamelMapOprtCellConst.PageTypeID_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new PageTypeIDMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			return base.ExecOprtCell(mapOprtCellInstInfo);
		}

		/// <summary>
		/// 设置图元属性值
		/// </summary>
		/// <param name="gFBaseTypePropValues">图元属性值列表</param>
		/// <returns>true:已执行，false:未执行</returns>
		protected override bool SetPropertyValue(GFBaseTypePropValueList gFBaseTypePropValues)
		{
			//容器图元不存在绑定界面数据对象实例，所以也不存在设置
			return false;
		}

		/// <summary>
		/// 属性值改变后需要做的处理，如：需要执行什么操作，如果是在View改变数据，调用回调接口将数据写到后端
		/// </summary>
		/// <param name="propertyID">属性ID</param>
		/// <param name="propertyValue">属性值</param>
		protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
		{
			base.AfterPropertyChanged(propertyID, propertyValue);
			if (string.Compare(propertyID, nameof(PanelPropertyModelEdit.ShowBorder)) == 0)
			{
				CallBack?.ExecOprt(nameof(PanelPropertyModelEdit.ShowBorder));
			}
			if (string.Compare(propertyID, nameof(PanelPropertyModelEdit.CompTypeID)) == 0)
			{
				CallBack?.ExecOprt(nameof(PanelPropertyModelEdit.CompTypeID));
			}
			if (string.Compare(propertyID, nameof(PanelPropertyModelEdit.ViewID)) == 0)
			{
				CallBack?.ExecOprt(nameof(PanelPropertyModelEdit.ViewID));
			}
			if (string.Compare(propertyID, nameof(PanelPropertyModelEdit.Alias)) == 0)
			{
				CallBack?.ExecOprt(nameof(PanelPropertyModelEdit.Alias));
			}
			if (string.Compare(propertyID, nameof(PanelPropertyModelEdit.PageTypeID)) == 0)
			{
				CallBack?.ExecOprt(nameof(PanelPropertyModelEdit.PageTypeID));
			}
			if (!PanelPropertyModelEdit.IsRuning)
			{
				//容器图元不存在绑定界面数据对象实例，所以也不存在更新界面数据对象属性到后端
				//CallBack?.UpdatePropertyValue(new GFBaseTypePropValueList());
			}
		}

		public override void OnZoomChanged()
        {

        }

        public override string ToString()
        {
            return ResourceA.PanelContainer;
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
			return null;
        }

        #region 操作原子执行对象

        /// <summary>
        /// 显示边框操作原子执行对象
        /// </summary>
        private class ShowBorderMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellPanelCtlObj mapCellPanelCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public ShowBorderMapOprtCellExector(MapCellPanelCtlObj mapCellPanelCtlObj)
			{
				this.mapCellPanelCtlObj = mapCellPanelCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is PanelViewModel panelViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PanelPropertyModelEdit.ShowBorder));
					if (mapCellPropValue is { })
					{
						panelViewModel.ShowBorder = mapCellPropValue.ToPrimitiveValue<bool>();
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 组件类型操作原子执行对象
		/// </summary>
		private class CompTypeIDMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellPanelCtlObj mapCellPanelCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public CompTypeIDMapOprtCellExector(MapCellPanelCtlObj mapCellPanelCtlObj)
			{
				this.mapCellPanelCtlObj = mapCellPanelCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is PanelViewModel panelViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PanelPropertyModelEdit.CompTypeID));
					if (mapCellPropValue is { })
					{
						panelViewModel.CompTypeID = mapCellPropValue.ToPrimitiveValue<string>();
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 实例别名操作原子执行对象
		/// </summary>
		private class AliasMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellPanelCtlObj mapCellPanelCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public AliasMapOprtCellExector(MapCellPanelCtlObj mapCellPanelCtlObj)
			{
				this.mapCellPanelCtlObj = mapCellPanelCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is PanelViewModel panelViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PanelPropertyModelEdit.Alias));
					if (mapCellPropValue is { })
					{
						panelViewModel.Alias = mapCellPropValue.ToPrimitiveValue<string>();
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 界面ID操作原子执行对象
		/// </summary>
		private class ViewIDMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellPanelCtlObj mapCellPanelCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public ViewIDMapOprtCellExector(MapCellPanelCtlObj mapCellPanelCtlObj)
			{
				this.mapCellPanelCtlObj = mapCellPanelCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is PanelViewModel panelViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PanelPropertyModelEdit.ViewID));
					if (mapCellPropValue is { })
					{
						panelViewModel.ViewID = mapCellPropValue.ToPrimitiveValue<string>();
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 页面类型ID操作原子执行对象
		/// </summary>
		private class PageTypeIDMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellPanelCtlObj mapCellPanelCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public PageTypeIDMapOprtCellExector(MapCellPanelCtlObj mapCellPanelCtlObj)
			{
				this.mapCellPanelCtlObj = mapCellPanelCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is PanelViewModel panelViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PanelPropertyModelEdit.PageTypeID));
					if (mapCellPropValue is { })
					{
						panelViewModel.PageTypeID = mapCellPropValue.ToPrimitiveValue<string>();
					}
				}
			}

			#endregion
		}
		#endregion
	}


	/// <summary>
	/// Panel容器属性编辑模型对象
	/// </summary>
	[Serializable]
	[MapPropertyOrder] 
	[CategoryPriority("图元信息",1)] 
	public class PanelPropertyModelEdit : ControlCellPropertyModelEdit
	{
		public PanelPropertyModelEdit() 
		{
			Styles.Add(ResourceA.PanelStyle1);
		}
		private bool _showBorder;

		/// <summary>
		/// 组件类型
		/// </summary>
		[DisplayName("显示边框")]
		[Category("图元信息")]
		[PropertySortOrder(15)]
		public bool ShowBorder
		{
			get
			{
				return _showBorder;
			}
			set
			{
				SetProperty(ref _showBorder, value);
			}
		}

		private string _compTypeID;
		/// <summary>
		/// 组件类型
		/// </summary>
		[DisplayName("组件类型")]
		[Category("图元信息")]
		[PropertySortOrder(16)]
		[CompTypeID]
		public string CompTypeID
		{
			get
			{
				return _compTypeID;
			}
			set
			{
				SetProperty(ref _compTypeID, value);
			}
		}

		private string _alias;
		/// <summary>
		/// 别名
		/// </summary>
		[DisplayName("组件实例")]
		[Category("图元信息")]
		[PropertySortOrder(17)]
		[CompInstanceID]
		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				SetProperty(ref _alias, value);
			}
		}

		private string _viewID;
		/// <summary>
		/// 界面ID
		/// </summary>
		[DisplayName("界面ID")]
		[Category("图元信息")]
		[PropertySortOrder(18)]
		[ViewID]
		public string ViewID
		{
			get
			{
				return _viewID;
			}
			set
			{
				SetProperty(ref _viewID, value);
			}
		}

		private string _pageTypeID;
		/// <summary>
		/// 页面类型ID
		/// </summary>
		[Browsable(false)]
		public string PageTypeID
		{
			get
			{
				return _pageTypeID;
			}
			set
			{
				SetProperty(ref _pageTypeID, value);
			}
		}



		/// <summary>
		/// 设置属性值
		/// </summary>
		/// <param name="propertyID">属性ID</param>
		/// <param name="propertyVal">属性值</param>
		/// <returns>True:已经找到该属性并设置，false:没有该属性</returns>
		public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
		{
			if (string.Compare(propertyID, nameof(ShowBorder)) == 0)
			{
				if (propertyVal is { })
				{
					ShowBorder = propertyVal.ToPrimitiveValue<bool>();
				}
				else
				{
					ShowBorder = false;
				}
				return true;
			}
			if (string.Compare(propertyID, nameof(CompTypeID)) == 0)
			{
				if (propertyVal is { })
				{
					CompTypeID = propertyVal.ToPrimitiveValue<string>();
				}
				else
				{
					CompTypeID = string.Empty;
				}
				return true;
			}
			if (string.Compare(propertyID, nameof(Alias)) == 0)
			{
				if (propertyVal is { })
				{
					Alias = propertyVal.ToPrimitiveValue<string>();
				}
				else
				{
					Alias = string.Empty;
				}
				return true;
			}
			if (string.Compare(propertyID, nameof(ViewID)) == 0)
			{
				if (propertyVal is { })
				{
					ViewID = propertyVal.ToPrimitiveValue<string>();
				}
				else
				{
					ViewID = string.Empty;
				}
				return true;
			}
			if (string.Compare(propertyID, nameof(PageTypeID)) == 0)
			{
				if (propertyVal is { })
				{
					PageTypeID = propertyVal.ToPrimitiveValue<string>();
				}
				else
				{
					PageTypeID = string.Empty;
				}
				return true;
			}			
			//注意最后一定要对在没有找到图元定义的属性调用其父类的SetPropertyValue
			return base.SetPropertyValue(propertyID, propertyVal);
		}

		/// <summary>
		/// 从来源实例复制字段到本实例
		/// </summary>
		/// <param name="source">来源实例</param>
		public void CopyFrom(PanelPropertyModelEdit source)
		{
			base.CopyFrom(source);
			this.ShowBorder = source.ShowBorder;
			this.PageTypeID = source.PageTypeID;
			this.CompTypeID = source.CompTypeID;
			this.Alias = source.Alias;
			this.ViewID = source.ViewID;
		}
	}
}
