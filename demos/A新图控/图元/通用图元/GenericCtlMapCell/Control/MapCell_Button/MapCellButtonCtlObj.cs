using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicData.Binding;
using GF_Gereric;
using Griffins;
using Griffins.Map.CtlMapCell.Generic.ViewModel;
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

namespace Griffins.Map.CtlMapCell.Generic.Button
{
    class MapCellButtonCtlObj : ControlCellBase
    {
        private ButtonView view;

		private ButtonViewModel buttonViewModel;
		static MapCellButtonCtlObj()
        {

        }
        public MapCellButtonCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

		public MapCellButtonCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
		{
			PropertyEditModelBase = CreatePropertyModelEditBase();
			PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
			EventBindEditModel = CreateEventBindEditModel();
			base.SetID(mapCellID);
			base.SetName(mapCellName);
			view = new ButtonView();
			RegisterProperty(new MapObjPropertyInfo(nameof(ButtonPropertyModelEdit.BackgroundImage), ResourceA.Image, GriffinsBaseDataType.Object_Bytes, BitmapData.Object_ID, typeof(Bitmap), false, true, null));
			RegisterProperty(new MapObjPropertyInfo(nameof(ButtonPropertyModelEdit.ImageSizeMode), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_ImageSizeMode), GriffinsBaseDataType.Integer, Guid.Empty, typeof(int), false, true, new GriffinsBaseValue(ImageSizeMode.None)));
			RegisterProperty(new MapObjPropertyInfo(nameof(ButtonPropertyModelEdit.BackColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(33, 0, 0, 0).ToColorString())));
			RegisterProperty(new MapObjPropertyInfo(nameof(ButtonPropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
			//RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_Cursor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_Cursor), MapCellPropDataType.Integer, Guid.Empty, typeof(int), true, true, new MapCellPropValue(CtlCellCursor.Default)));
			RegisterProperty(new MapObjPropertyInfo(nameof(ButtonPropertyModelEdit.ButtonName), ResourceA.ButtonName, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, null));
			RegisterProperty(new MapObjPropertyInfo(nameof(ButtonPropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), false, true, new GriffinsBaseValue(FontInfo.DefaultFont)));
			RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
			RegisterOprtCellInfo(new MapOprtCellInfo(ButtonMapOprtCellConst.ButtonName_MapOprtCellID, ResourceA.ButtonName_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(ButtonMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(ButtonMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(ButtonMapOprtCellConst.BackgroundImage_MapOprtCellID, ResourceA.BackgroundImage_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(ButtonMapOprtCellConst.ImageSizeMode_MapOprtCellID, ResourceA.ImageSizeMode_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(ButtonMapOprtCellConst.TextFont_MapOprtCellID, ResourceA.TextFont_MapOprtCellName));
			RegisterOprtInfo(new MapOprtInfo(nameof(ButtonPropertyModelEdit.ButtonName), ResourceA.ButtonName_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=ButtonMapOprtCellConst.ButtonName_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(ButtonPropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=ButtonMapOprtCellConst.TextColor_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(ButtonPropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=ButtonMapOprtCellConst.BackColor_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(ButtonPropertyModelEdit.BackgroundImage), ResourceA.BackgroundImage_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=ButtonMapOprtCellConst.BackgroundImage_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(ButtonPropertyModelEdit.ImageSizeMode), ResourceA.ImageSizeMode_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=ButtonMapOprtCellConst.ImageSizeMode_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(ButtonPropertyModelEdit.TextFont), ResourceA.TextFont_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=ButtonMapOprtCellConst.TextFont_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			(this as IMapCellTypeBase).Name = ResourceA.Button;
			buttonViewModel = new ButtonViewModel(ButtonPropertyModelEdit, clickExec);
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
		public ButtonPropertyModelEdit ButtonPropertyModelEdit
		{
			get { return PropertyEditModelBase as ButtonPropertyModelEdit; }
		}

		public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal,bool isRuning)
        {
			ButtonPropertyModelEdit.IsRuning = isRuning;
			return ButtonPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);			
			var propertyEditModelBase = JsonObjConvert.FromJSon<ButtonPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
			(PropertyEditModelBase as ButtonPropertyModelEdit).CopyFrom(propertyEditModelBase);
			var propertyBindEditModelBase = JsonObjConvert.FromJSon<ButtonPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
			(PropertyBindEditModelBase as ButtonPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
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
			MapCellButtonCtlObj mapCellButtonCtlObj=(source as MapCellButtonCtlObj);
			base._CopyFrom(mapCellButtonCtlObj);
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
			return new ButtonPropertyModelEdit();
		}

		/// <summary>
		/// 创建图元属性绑定编辑模型对象
		/// </summary>
		/// <returns>图元属性绑定编辑模型对象</returns>
		public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
		{
			return new ButtonPropertyBindEditModel();
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
			};
		}

		/// <summary>
		/// 执行图元内部操作原子
		/// </summary>
		/// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
		/// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
		protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
		{
			if (mapOprtCellInstInfo.OprtCellID == ButtonMapOprtCellConst.ButtonName_MapOprtCellID)
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
			if (mapOprtCellInstInfo.OprtCellID == ButtonMapOprtCellConst.TextColor_MapOprtCellID)
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
			if (mapOprtCellInstInfo.OprtCellID == ButtonMapOprtCellConst.BackColor_MapOprtCellID)
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
			if (mapOprtCellInstInfo.OprtCellID == ButtonMapOprtCellConst.BackgroundImage_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new ImageBackgroundMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == ButtonMapOprtCellConst.ImageSizeMode_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new ImageSizeModeMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == ButtonMapOprtCellConst.TextFont_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new TextFontMapOprtCellExector(this);
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
				ButtonPropertyModelEdit.SetPropertyValue(gFBaseTypePropValue.PropertyID.ToString(), gFBaseTypePropValue.Value);
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
			if (string.Compare(propertyID, nameof(ButtonPropertyModelEdit.ButtonName)) == 0)
			{
				CallBack?.ExecOprt(nameof(ButtonPropertyModelEdit.ButtonName));
			}
			if (string.Compare(propertyID, nameof(ButtonPropertyModelEdit.TextColor)) == 0)
			{
				CallBack?.ExecOprt(nameof(ButtonPropertyModelEdit.TextColor));
			}
			if (string.Compare(propertyID, nameof(ButtonPropertyModelEdit.BackColor)) == 0)
			{
				CallBack?.ExecOprt(nameof(ButtonPropertyModelEdit.BackColor));
			}
			if (string.Compare(propertyID, nameof(ButtonPropertyModelEdit.BackgroundImage)) == 0)
			{
				CallBack?.ExecOprt(nameof(ButtonPropertyModelEdit.BackgroundImage));
			}
			if (string.Compare(propertyID, nameof(ButtonPropertyModelEdit.ImageSizeMode)) == 0)
			{
				CallBack?.ExecOprt(nameof(ButtonPropertyModelEdit.ImageSizeMode));
			}
			if (string.Compare(propertyID, nameof(ButtonPropertyModelEdit.TextFont)) == 0)
			{
				CallBack?.ExecOprt(nameof(ButtonPropertyModelEdit.TextFont));
			}
			if (!ButtonPropertyModelEdit.IsRuning)
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
            SetButtonTextFont();
        }

		internal void SetButtonTextFont()
		{
			double size = base.CallBack?.Calc?.CalcZoomVal((decimal)this.ButtonPropertyModelEdit.TextFont.FontSize) ?? this.ButtonPropertyModelEdit.TextFont.FontSize;
			if (size < 2)
				size = 2;
			FontInfo font = new FontInfo(this.ButtonPropertyModelEdit.TextFont.FontFamily, size, this.ButtonPropertyModelEdit.TextFont.FontWeight, this.ButtonPropertyModelEdit.TextFont.FontStyle);
			this.buttonViewModel.TextFont = font;
		}

        public override string ToString()
        {
            return "按钮";
        }

		#region 操作原子执行对象

		/// <summary>
		/// 按钮文本操作原子执行对象
		/// </summary>
		private class ButtonNameMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellButtonCtlObj mapCellButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public ButtonNameMapOprtCellExector(MapCellButtonCtlObj mapCellButtonCtlObj)
			{
				this.mapCellButtonCtlObj = mapCellButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is ButtonViewModel buttonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(ButtonPropertyModelEdit.ButtonName));
					if (mapCellPropValue is { })
					{
						buttonViewModel.ButtonName = mapCellPropValue.ToPrimitiveValue<string>();
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
			private MapCellButtonCtlObj mapCellButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public TextColorMapOprtCellExector(MapCellButtonCtlObj mapCellButtonCtlObj)
			{
				this.mapCellButtonCtlObj = mapCellButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is ButtonViewModel buttonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(ButtonPropertyModelEdit.TextColor));
					if (mapCellPropValue is { })
					{
						var color = mapCellPropValue.ToPrimitiveValue<string>();
						buttonViewModel.TextColor = Color.Parse(color);
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
			private MapCellButtonCtlObj mapCellButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public BackColorMapOprtCellExector(MapCellButtonCtlObj mapCellButtonCtlObj)
			{
				this.mapCellButtonCtlObj = mapCellButtonCtlObj;
			}

			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is ButtonViewModel buttonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(ButtonPropertyModelEdit.BackColor));
					if (mapCellPropValue is { })
					{
						var color = mapCellPropValue.ToPrimitiveValue<string>();
						buttonViewModel.BackColor = Color.Parse(color);
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 背景图片操作原子执行对象
		/// </summary>
		private class ImageBackgroundMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellButtonCtlObj mapCellButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public ImageBackgroundMapOprtCellExector(MapCellButtonCtlObj mapCellButtonCtlObj)
			{
				this.mapCellButtonCtlObj = mapCellButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is ButtonViewModel buttonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(ButtonPropertyModelEdit.BackgroundImage));
					if (mapCellPropValue is { })
					{
						ObjectValue_Bytes objectValue_Bytes = mapCellPropValue.ToObjectValue_Bytes();
						GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Bytes);
						IGriffinsBaseValue bitmapData = new BitmapData();
						bitmapData.PopulateFromBaseValue(griffinsBaseValue);
						buttonViewModel.BackgroundImage = (BitmapData)bitmapData;
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 背景图片定位方式操作原子执行对象
		/// </summary>
		private class ImageSizeModeMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellButtonCtlObj mapCellButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public ImageSizeModeMapOprtCellExector(MapCellButtonCtlObj mapCellButtonCtlObj)
			{
				this.mapCellButtonCtlObj = mapCellButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is ButtonViewModel buttonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(ButtonPropertyModelEdit.ImageSizeMode));
					if (mapCellPropValue is { })
					{
						ImageSizeMode imageSizeMode = mapCellPropValue.ToPrimitiveValue<ImageSizeMode>();
						buttonViewModel.ImageSizeMode = imageSizeMode;
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 文本字体操作原子执行对象
		/// </summary>
		private class TextFontMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellButtonCtlObj mapCellButtonCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public TextFontMapOprtCellExector(MapCellButtonCtlObj mapCellButtonCtlObj)
			{
				this.mapCellButtonCtlObj = mapCellButtonCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is ButtonViewModel buttonViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(ButtonPropertyModelEdit.TextFont));
					if (mapCellPropValue is { })
					{
						ObjectValue_Json objectValue_Json = mapCellPropValue.ToObjectValue_Json();
						GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
						IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
						iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
						buttonViewModel.TextFont = (FontInfo)iMPPropObjectValue;
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
	public class ButtonPropertyModelEdit : ControlCellPropertyModelEdit
	{
		public ButtonPropertyModelEdit() 
		{
			TextFont = new FontInfo(FontManager.Current.DefaultFontFamily, 14.0, FontWeight.Normal, FontStyle.Normal);
			//TextFont.PropertyChanged += textFont_PropertyChanged;
			Styles.Add(ResourceA.ButtonStyle1);
		}

		private void textFont_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// 通知PropertyGrid：嵌套对象的属性已变化
			RaisePropertyChanged(nameof(TextFont));
		}

		private string _buttonName;
		/// <summary>
		/// 按钮名称
		/// </summary>
		[DisplayName("按钮名称")]
		[Category("图元信息")]
		[PropertySortOrder(15)]
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

		private FontInfo _textFont;// = FontInfo.DefaultFont;
		[DisplayName("文字字体")]
		[Category("图元信息")]
		[PropertySortOrder(16)]
		public FontInfo TextFont
		{
			get
			{
				return _textFont;
			}
			set
			{
				//SetProperty(ref _textFont, value);
				// 1. 旧实例：取消事件监听（避免内存泄漏+无效监听）
				if (_textFont != null)
					_textFont.PropertyChanged -= textFont_PropertyChanged;

				// 2. 设置新值，并触发自身的PropertyChanged（关键）
				if (SetProperty(ref _textFont, value)) // 用基类的SetProperty，而非直接赋值
				{
					// 3. 新实例：绑定事件监听
					if (_textFont != null)
						_textFont.PropertyChanged += textFont_PropertyChanged;
				}
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

		private BitmapData _backgroundImage;
		/// <summary>
		/// 背景图片
		/// </summary>
		[DisplayName("背景图片")]
		[Category("图元信息")]
		[PropertySortOrder(19)]
		public BitmapData BackgroundImage
		{
			get
			{
				return _backgroundImage;
			}
			set
			{
				SetProperty(ref _backgroundImage, value);
			}
		}

		private ImageSizeMode _imageSizeMode = ImageSizeMode.None;
		/// <summary>
		/// 图片定位方式
		/// </summary>
		[DisplayName("图片定位方式")]
		[Category("图元信息")]
		[PropertySortOrder(20)]
		public ImageSizeMode ImageSizeMode
		{
			get
			{
				return _imageSizeMode;
			}
			set
			{
				SetProperty(ref _imageSizeMode, value);
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
			if (string.Compare(propertyID, nameof(BackgroundImage)) == 0)
			{
				if (propertyVal is { })
				{
					ObjectValue_Bytes objectValue_Bytes = propertyVal.ToObjectValue_Bytes();
					GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Bytes);
					IGriffinsBaseValue bitmapData = new BitmapData();
					bitmapData.PopulateFromBaseValue(griffinsBaseValue);
					BackgroundImage = (BitmapData)bitmapData;
				}
				else
					BackgroundImage = null;
				return true;
			}
			if (string.Compare(propertyID, nameof(ImageSizeMode)) == 0)
			{
				if (propertyVal is { })
				{
					ImageSizeMode imageSizeMode = propertyVal.ToPrimitiveValue<ImageSizeMode>();
					ImageSizeMode = imageSizeMode;
				}
				else
				{
					ImageSizeMode = ImageSizeMode.None;
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
			if (string.Compare(propertyID, nameof(TextFont)) == 0)
			{
				if (propertyVal is { })
				{
					ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
					GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
					IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
					iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
					TextFont = (FontInfo)iMPPropObjectValue;
				}
				else
				{
					TextFont = FontInfo.DefaultFont;
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
		public void CopyFrom(ButtonPropertyModelEdit source)
		{
			base.CopyFrom(source);
			this.ButtonName = source.ButtonName;
			if (source.TextFont != null)
			{
				this.TextFont = new FontInfo(source.TextFont.FontFamily, source.TextFont.FontSize, source.TextFont.FontWeight, source.TextFont.FontStyle);
			}
			this.TextColor = source.TextColor;
			this.BackColor = source.BackColor;
			if (source.BackgroundImage != null)
			{
				var bitMapData = new BitmapData();
				bitMapData.FromBytes(source.BackgroundImage.ToBytes());
				this.BackgroundImage = bitMapData;
			}
			this.ImageSizeMode = source.ImageSizeMode;
		}
	}

	/// <summary>
	/// 按钮图元属性绑定编辑模型对象，可由图元继承
	/// </summary>
	[Serializable]
	[MapPropertyOrder]
	[CategoryPriority("界面数据", 1)]
	[CategoryPriority("绑定信息", 2)]
	public class ButtonPropertyBindEditModel : ControlCellPropertyBindEditModel
	{
		private PropertyBindInfo _buttonName = new PropertyBindInfo(GriffinsBaseDataType.String);
		/// <summary>
		/// 按钮名称所绑定点位的属性
		/// </summary>
		[DisplayName("按钮名称")]
		[Category("绑定信息")]
		[PropertySortOrder(6)]
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

		private PropertyBindInfo _textFont = new PropertyBindInfo(GriffinsBaseDataType.Object_Json, FontInfo.Object_ID);
		[DisplayName("文字字体")]
		[Category("绑定信息")]
		[PropertySortOrder(7)]
		[BindMPPropertyID]
		public PropertyBindInfo TextFont
		{
			get
			{
				return _textFont;
			}
			set
			{
				SetProperty(ref _textFont, value);
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

		private PropertyBindInfo _backgroundImage = new PropertyBindInfo(GriffinsBaseDataType.Object_Bytes, BitmapData.Object_ID);
		/// <summary>
		/// 文本颜色
		/// </summary>
		[DisplayName("背景图片")]
		[Category("绑定信息")]
		[PropertySortOrder(10)]
		[BindMPPropertyID]
		public PropertyBindInfo BackgroundImage
		{
			get
			{
				return _backgroundImage;
			}
			set
			{
				SetProperty(ref _backgroundImage, value);
			}
		}

		private PropertyBindInfo _imageSizeMode = new PropertyBindInfo(GriffinsBaseDataType.Integer);
		/// <summary>
		/// 图片定位方式
		/// </summary>
		[DisplayName("图片定位方式")]
		[Category("绑定信息")]
		[PropertySortOrder(11)]
		[BindMPPropertyID]
		public PropertyBindInfo ImageSizeMode
		{
			get
			{
				return _imageSizeMode;
			}
			set
			{
				SetProperty(ref _imageSizeMode, value);
			}
		}

		/// <summary>
		/// 从来源实例复制字段到本实例
		/// </summary>
		/// <param name="source">来源实例</param>
		public void CopyFrom(ButtonPropertyBindEditModel source)
		{
			base.CopyFrom(source);
			this.ButtonName.CopyFrom(source.ButtonName);
			this.TextFont.CopyFrom(source.TextFont);
			this.TextColor.CopyFrom(source.TextColor);
			this.BackColor.CopyFrom(source.BackColor);
			this.BackgroundImage.CopyFrom(source.BackgroundImage);
			this.ImageSizeMode.CopyFrom(source.ImageSizeMode);
		}
	}
}
