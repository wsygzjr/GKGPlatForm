using Avalonia.Media;
using Avalonia.Media.Imaging;
using GF_Gereric;
using Griffins;
using Griffins.Map.CtlMapCell.Generic.Lable;
using Griffins.Map.CtlMapCell.Generic.TextButton.View;
using Griffins.Map.CtlMapCell.Generic.TextButton.ViewModel;
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

namespace Griffins.Map.CtlMapCell.Generic.TextButton
{
    class MapCellTextButtonCtlObj : ControlCellBase
    {
        private TextButtonView view;

		private TextButtonViewModel buttonViewModel;
		static MapCellTextButtonCtlObj()
        {

        }
        public MapCellTextButtonCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

		public MapCellTextButtonCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
		{
			PropertyEditModelBase = CreatePropertyModelEditBase();
			PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
			EventBindEditModel = CreateEventBindEditModel();
			base.SetID(mapCellID);
			base.SetName(mapCellName);
			view = new TextButtonView();
			RegisterProperty(new MapObjPropertyInfo(nameof(TextButtonPropertyModelEdit.TextValue), ResourceA.TextValue, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, null));
			RegisterProperty(new MapObjPropertyInfo(nameof(TextButtonPropertyModelEdit.BackColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(33, 0, 0, 0).ToColorString())));
			RegisterProperty(new MapObjPropertyInfo(nameof(TextButtonPropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
			RegisterProperty(new MapObjPropertyInfo(nameof(TextButtonPropertyModelEdit.ButtonName), ResourceA.ButtonName, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, null));
			RegisterProperty(new MapObjPropertyInfo(nameof(TextButtonPropertyModelEdit.ColumnSpacing), ResourceA.ColumnSpacing, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(decimal), false, true, new GriffinsBaseValue(8)));
			RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
			RegisterOprtCellInfo(new MapOprtCellInfo(TextButtonMapOprtCellConst.ButtonName_MapOprtCellID, ResourceA.ButtonName_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(TextButtonMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(TextButtonMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(TextButtonMapOprtCellConst.TextValue_MapOprtCellID, ResourceA.TextValue_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(TextButtonMapOprtCellConst.ColumnSpacing_MapOprtCellID, ResourceA.ColumnSpacing_MapOprtCellName));
			RegisterOprtInfo(new MapOprtInfo(nameof(TextButtonPropertyModelEdit.ButtonName), ResourceA.ButtonName_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=TextButtonMapOprtCellConst.ButtonName_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(TextButtonPropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=TextButtonMapOprtCellConst.TextColor_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(TextButtonPropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=TextButtonMapOprtCellConst.BackColor_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(TextButtonPropertyModelEdit.TextValue), ResourceA.TextValue_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=TextButtonMapOprtCellConst.TextValue_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(TextButtonPropertyModelEdit.ColumnSpacing), ResourceA.ColumnSpacing_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=TextButtonMapOprtCellConst.ColumnSpacing_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			(this as IMapCellTypeBase).Name = ResourceA.Button;
			buttonViewModel = new TextButtonViewModel(designTime, TextButtonPropertyModelEdit, clickExec);
			view.DataContext = buttonViewModel;
		}

		private void clickExec()
		{
			EventCmdInfo? eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault
				(info => info.EventID == MapObjPropEventConst.Event_MouseClick);
			if (eventCmdInfo != null)
			{
				GFBaseTypeParamValueList cmdParam = null;
				if (!string.IsNullOrWhiteSpace(eventCmdInfo.CmdParam))
				{
					cmdParam = new GFBaseTypeParamValueList();
					cmdParam.FromJson(eventCmdInfo.CmdParam);
				}
				CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, cmdParam, out GFBaseTypeParamValueList retVal);
			}
		}

		[Browsable(false)]
		public TextButtonPropertyModelEdit TextButtonPropertyModelEdit
		{
			get { return PropertyEditModelBase as TextButtonPropertyModelEdit; }
		}

		public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
		{
			TextButtonPropertyModelEdit.IsRuning = isRuning;
			return TextButtonPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
		}

        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);			
			var propertyEditModelBase = JsonObjConvert.FromJSon<TextButtonPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
			(PropertyEditModelBase as TextButtonPropertyModelEdit).CopyFrom(propertyEditModelBase);
			var propertyBindEditModelBase = JsonObjConvert.FromJSon<TextButtonPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
			(PropertyBindEditModelBase as TextButtonPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
			var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
			EventBindEditModel.CopyFrom(eventBindEditModel);
		}

        /// <summary>
        /// 当把画图信息写入到字节流中（必须先调用基类的OnWriteDrawInfoToBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="bw">字节流写入对象</param>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
			bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
			bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
			bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
		}

        protected override void OnCopyFrom(ControlCellBase source)
        {
			MapCellTextButtonCtlObj mapCellTextButtonCtlObj = (source as MapCellTextButtonCtlObj);
			base._CopyFrom(mapCellTextButtonCtlObj);
			(PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
			(PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
			EventBindEditModel.CopyFrom(source.EventBindEditModel);
		}

        /// <summary>
        /// 初始化时
        /// </summary>
        protected override void OnInit()
        {

        }

        protected override object OnGetView()
        {
            return view;
        }

		protected override object OnGetViewModel()
		{
			return buttonViewModel;
		}

		/// <summary>
		/// 创建图元属性编辑模型对象
		/// </summary>
		/// <returns>图元属性编辑模型对象</returns>
		public override PropertyEditModelBase CreatePropertyModelEditBase()
		{
			return new TextButtonPropertyModelEdit();
		}

		/// <summary>
		/// 创建图元属性绑定编辑模型对象
		/// </summary>
		/// <returns>图元属性绑定编辑模型对象</returns>
		public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
		{
			return new TextButtonPropertyBindEditModel();
		}

		/// <summary>
		/// 创建图元事件绑定编辑模型对象
		/// </summary>
		/// <returns>图元事件绑定编辑模型对象</returns>
		public override EventBindEditModel CreateEventBindEditModel()
		{
			return new EventBindEditModel()
			{
				EventCmdInfos = new BindingList<EventCmdInfo>()
				{
					new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseClick },
				}
			}; ;
		}

		/// <summary>
		/// 执行图元内部操作原子
		/// </summary>
		/// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
		/// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
		protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
		{
			if (mapOprtCellInstInfo.OprtCellID == TextButtonMapOprtCellConst.ButtonName_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new ButtonNameMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == TextButtonMapOprtCellConst.TextColor_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new TextColorMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == TextButtonMapOprtCellConst.BackColor_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new BackColorMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == TextButtonMapOprtCellConst.TextValue_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new TextValueMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == TextButtonMapOprtCellConst.ColumnSpacing_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new ColumnSpacingMapOprtCellExector(this);
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
			//对于通用图元来说，框架已经将界面数据对象属性值转换为图元对象属性值，所以在此可以直接用
			foreach (GFBaseTypePropValue gFBaseTypePropValue in gFBaseTypePropValues)
			{
				TextButtonPropertyModelEdit.SetPropertyValue(gFBaseTypePropValue.PropertyID.ToString(), gFBaseTypePropValue.Value);
			}
			return true;
		}

		/// <summary>
		/// 属性值改变后需要做的处理，如：需要执行什么操作，如果是在View改变数据，调用回调接口将数据写到后端
		/// </summary>
		/// <param name="propertyID">属性ID</param>
		/// <param name="propertyValue">属性值</param>
		protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
		{
			base.AfterPropertyChanged(propertyID, propertyValue);
			if (string.Compare(propertyID, nameof(TextButtonPropertyModelEdit.ButtonName)) == 0)
			{
				CallBack?.ExecOprt(nameof(TextButtonPropertyModelEdit.ButtonName));
			}
			if (string.Compare(propertyID, nameof(TextButtonPropertyModelEdit.TextColor)) == 0)
			{
				CallBack?.ExecOprt(nameof(TextButtonPropertyModelEdit.TextColor));
			}
			if (string.Compare(propertyID, nameof(TextButtonPropertyModelEdit.BackColor)) == 0)
			{
				CallBack?.ExecOprt(nameof(TextButtonPropertyModelEdit.BackColor));
			}
			if (string.Compare(propertyID, nameof(TextButtonPropertyModelEdit.TextValue)) == 0)
			{
				CallBack?.ExecOprt(nameof(TextButtonPropertyModelEdit.TextValue));
			}
			if (string.Compare(propertyID, nameof(TextButtonPropertyModelEdit.ColumnSpacing)) == 0)
			{
				CallBack?.ExecOprt(nameof(TextButtonPropertyModelEdit.ColumnSpacing));
			}
			if (!TextButtonPropertyModelEdit.IsRuning)
			{
				//对于通用图元来说，直接调用回调将图元对象属性值传给框架，
				//由框架将图元对象属性值转换为界面数据属性值发送给后端
				CallBack?.UpdatePropertyValue(new GFBaseTypePropValueList()
				{
					new GFBaseTypePropValue(MPPropertyID.Parse(propertyID),propertyValue)
				});
			}
		}

		public override void OnZoomChanged()
        {

        }

        public override string ToString()
        {
            return "文本+按钮";
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
			return null;
        }


        #region 操作原子执行对象

        /// <summary>
        /// 按钮文本操作原子执行对象
        /// </summary>
        private class ButtonNameMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellTextButtonCtlObj mapCellTextButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public ButtonNameMapOprtCellExector(MapCellTextButtonCtlObj mapCellTextButtonCtlObj)
			{
				this.mapCellTextButtonCtlObj = mapCellTextButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is TextButtonViewModel textButtonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(TextButtonPropertyModelEdit.ButtonName));
					if (mapCellPropValue is { })
					{
						textButtonViewModel.ButtonName = mapCellPropValue.ToPrimitiveValue<string>();
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 文本颜色操作原子执行对象
		/// </summary>
		private class TextColorMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellTextButtonCtlObj mapCellTextButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public TextColorMapOprtCellExector(MapCellTextButtonCtlObj mapCellTextButtonCtlObj)
			{
				this.mapCellTextButtonCtlObj = mapCellTextButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is TextButtonViewModel textButtonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(TextButtonPropertyModelEdit.TextColor));
					if (mapCellPropValue is { })
					{
						var color = mapCellPropValue.ToPrimitiveValue<string>();
						textButtonViewModel.TextColor = Color.Parse(color);
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 背景颜色操作原子执行对象
		/// </summary>
		private class BackColorMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellTextButtonCtlObj mapCellTextButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public BackColorMapOprtCellExector(MapCellTextButtonCtlObj mapCellTextButtonCtlObj)
			{
				this.mapCellTextButtonCtlObj = mapCellTextButtonCtlObj;
			}

			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is TextButtonViewModel textButtonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(TextButtonPropertyModelEdit.BackColor));
					if (mapCellPropValue is { })
					{
						var color = mapCellPropValue.ToPrimitiveValue<string>();
						textButtonViewModel.BackColor = Color.Parse(color);
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 文本值操作原子执行对象
		/// </summary>
		private class TextValueMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellTextButtonCtlObj mapCellTextButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public TextValueMapOprtCellExector(MapCellTextButtonCtlObj mapCellTextButtonCtlObj)
			{
				this.mapCellTextButtonCtlObj = mapCellTextButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is TextButtonViewModel textButtonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(TextButtonPropertyModelEdit.TextValue));
					if (mapCellPropValue is { })
					{
						textButtonViewModel.TextValue = mapCellPropValue.ToPrimitiveValue<string>();
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 列间距操作原子执行对象
		/// </summary>
		private class ColumnSpacingMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellTextButtonCtlObj mapCellTextButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public ColumnSpacingMapOprtCellExector(MapCellTextButtonCtlObj mapCellTextButtonCtlObj)
			{
				this.mapCellTextButtonCtlObj = mapCellTextButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is TextButtonViewModel textButtonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(TextButtonPropertyModelEdit.ColumnSpacing));
					if (mapCellPropValue != null)
					{
						textButtonViewModel.ColumnSpacing = mapCellPropValue.ToPrimitiveValue<double>(); 
					}
				}
			}

			#endregion
		}

		#endregion
	}


	/// <summary>
	/// 按钮属性编辑模型对象
	/// </summary>
	[Serializable]
	[MapPropertyOrder] 
	[CategoryPriority("图元信息",1)] 
	public class TextButtonPropertyModelEdit : ControlCellPropertyModelEdit
	{
		public TextButtonPropertyModelEdit() 
		{
			Styles.Add(ResourceA.TextButtonStyle1);
		}

		private string _textValue;
		/// <summary>
		/// 文本值
		/// </summary>
		[DisplayName("文本值")]
		[Category("图元信息")]
		[PropertySortOrder(15)]
		public string TextValue
		{
			get
			{
				return _textValue;
			}
			set
			{
				SetProperty(ref _textValue, value);
			}
		}

		private string _buttonName;
		/// <summary>
		/// 按钮名称
		/// </summary>
		[DisplayName("按钮名称")]
		[Category("图元信息")]
		[PropertySortOrder(16)]
		public string ButtonName
		{
			get
			{
				return _buttonName;
			}
			set
			{
				SetProperty(ref _buttonName, value);
			}
		}

		private Color _textColor = Colors.Black;
		/// <summary>
		/// 文本颜色
		/// </summary>
		[DisplayName("文本颜色")]
		[Category("图元信息")]
		[PropertySortOrder(17)]
		[JsonConverter(typeof(ColorConvert))]
		public Color TextColor
		{
			get
			{
				return _textColor;
			}
			set
			{
				SetProperty(ref _textColor, value);
			}
		}

		private Color _backColor = Color.FromArgb(33, 0, 0, 0);
		/// <summary>
		/// 背景颜色
		/// </summary>
		[DisplayName("背景颜色")]
		[Category("图元信息")]
		[PropertySortOrder(18)]
		[JsonConverter(typeof(ColorConvert))]
		public Color BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				SetProperty(ref _backColor, value);
			}
		}

		private double _columnSpacing = 8;
		/// <summary>
		/// 列间距
		/// </summary>
		[DisplayName("列间距")]
		[Category("图元信息")]
		[FloatPrecision(1)]
		[PropertySortOrder(19)]
		public double ColumnSpacing
		{
			get
			{
				return _columnSpacing;
			}
			set
			{
				SetProperty(ref _columnSpacing, value);
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
			if (string.Compare(propertyID, nameof(TextValue)) == 0)
			{
				if (propertyVal is { })
				{
					TextValue = propertyVal.ToPrimitiveValue<string>();
				}
				else
				{
					TextValue = "";
				}
				return true;
			}
			if (string.Compare(propertyID, nameof(BackColor)) == 0)
			{
				if (propertyVal is { })
				{
					var color1 = propertyVal.ToPrimitiveValue<string>();
					BackColor = Color.Parse(color1);
				}
				else
				{
					BackColor = Color.FromArgb(33, 0, 0, 0);
				}
				return true;
			}
			if (string.Compare(propertyID, nameof(TextColor)) == 0)
			{
				if (propertyVal is { })
				{
					var color1 = propertyVal.ToPrimitiveValue<string>();
					TextColor = Color.Parse(color1);
				}
				else
				{
					TextColor = Colors.Black;
				}
				return true;
			}
			if (string.Compare(propertyID, nameof(ButtonName)) == 0)
			{
				if (propertyVal is { })
				{
					ButtonName = propertyVal.ToPrimitiveValue<string>();
				}
				else
				{
					ButtonName = "";
				}
				return true;
			}
			if (string.Compare(propertyID, nameof(ColumnSpacing)) == 0)
			{
				if (propertyVal is { })
				{
					ColumnSpacing = propertyVal.ToPrimitiveValue<double>();
				}
				else
				{
					ColumnSpacing = 0;
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
		public void CopyFrom(TextButtonPropertyModelEdit source)
		{
			base.CopyFrom(source);
			this.TextValue = source.TextValue;
			this.ButtonName = source.ButtonName;
			this.TextColor = source.TextColor;
			this.BackColor = source.BackColor;
			this.ColumnSpacing=source.ColumnSpacing;
		}
	}

	/// <summary>
	/// 按钮图元属性绑定编辑模型对象，可由图元继承
	/// </summary>
	[Serializable]
	[MapPropertyOrder]
	[CategoryPriority("界面数据", 1)]
	[CategoryPriority("绑定信息", 2)]
	public class TextButtonPropertyBindEditModel : ControlCellPropertyBindEditModel
	{
		private PropertyBindInfo _textValue = new PropertyBindInfo(GriffinsBaseDataType.String);
		/// <summary>
		/// 文本值
		/// </summary>
		[DisplayName("文本值")]
		[Category("绑定信息")]
		[PropertySortOrder(6)]
		[BindMPPropertyID]
		public PropertyBindInfo TextValue
		{
			get
			{
				return _textValue;
			}
			set
			{
				SetProperty(ref _textValue, value);
			}
		}

		private PropertyBindInfo _buttonName = new PropertyBindInfo(GriffinsBaseDataType.String);
		/// <summary>
		/// 按钮名称所绑定点位的属性
		/// </summary>
		[DisplayName("按钮名称")]
		[Category("绑定信息")]
		[PropertySortOrder(7)]
		[BindMPPropertyID]
		public PropertyBindInfo ButtonName
		{
			get
			{
				return _buttonName;
			}
			set
			{
				SetProperty(ref _buttonName, value);
			}
		}

		private PropertyBindInfo _textColor = new PropertyBindInfo(GriffinsBaseDataType.String);
		/// <summary>
		/// 文本颜色
		/// </summary>
		[DisplayName("文本颜色")]
		[Category("绑定信息")]
		[PropertySortOrder(8)]
		[BindMPPropertyID]
		public PropertyBindInfo TextColor
		{
			get
			{
				return _textColor;
			}
			set
			{
				SetProperty(ref _textColor, value);
			}
		}

		private PropertyBindInfo _backColor = new PropertyBindInfo(GriffinsBaseDataType.String);
		/// <summary>
		/// 背景颜色
		/// </summary>
		[DisplayName("背景颜色")]
		[Category("绑定信息")]
		[PropertySortOrder(9)]
		[BindMPPropertyID]
		public PropertyBindInfo BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				SetProperty(ref _backColor, value);
			}
		}

		private PropertyBindInfo _columnSpacing = new PropertyBindInfo(GriffinsBaseDataType.Decimal);
		/// <summary>
		/// 列间距
		/// </summary>
		[DisplayName("列间距")]
		[Category("绑定信息")]
		[PropertySortOrder(10)]
		[BindMPPropertyID]
		public PropertyBindInfo ColumnSpacing
		{
			get
			{
				return _columnSpacing;
			}
			set
			{
				SetProperty(ref _columnSpacing, value);
			}
		}

		/// <summary>
		/// 从来源实例复制字段到本实例
		/// </summary>
		/// <param name="source">来源实例</param>
		public void CopyFrom(TextButtonPropertyBindEditModel source)
		{
			base.CopyFrom(source);
			this.TextValue.CopyFrom(source.TextValue);
			this.ButtonName.CopyFrom(source.ButtonName);
			this.TextColor.CopyFrom(source.TextColor);
			this.BackColor.CopyFrom(source.BackColor);
			this.ColumnSpacing.CopyFrom(source.ColumnSpacing);
		}
	}
}
