using Avalonia.Media;
using Avalonia.Media.Imaging;
using GF_Gereric;
using Griffins;
using Griffins.Map.CtlMapCell.Generic.Lable.View;
using Griffins.Map.CtlMapCell.Generic.Lable.ViewModel;
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

namespace Griffins.Map.CtlMapCell.Generic.Lable
{
    class MapCellLableCtlObj : ControlCellBase
    {
        private LableView view;

		private LableViewModel lableViewModel;
		static MapCellLableCtlObj()
        {

        }
        public MapCellLableCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

		public MapCellLableCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
		{
			PropertyEditModelBase = CreatePropertyModelEditBase();
			PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
			EventBindEditModel = CreateEventBindEditModel();
			base.SetID(mapCellID);
			base.SetName(mapCellName);
			view = new LableView();
			RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.LableValue), ResourceA.TextValue, GriffinsBaseDataType.String, Guid.Empty, typeof(string), false, true, null));
			RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.BackColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(33, 0, 0, 0).ToColorString())));
			RegisterProperty(new MapObjPropertyInfo(nameof(LablePropertyModelEdit.LableColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
			RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.LableValue_MapOprtCellID, ResourceA.LableValue_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.LableColor_MapOprtCellID, ResourceA.LableColor_MapOprtCellName));
			RegisterOprtCellInfo(new MapOprtCellInfo(LableMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.LableBackColor_MapOprtCellName));
			RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.LableValue), ResourceA.LableValue_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=LableMapOprtCellConst.LableValue_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.LableColor), ResourceA.LableColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=LableMapOprtCellConst.LableColor_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			RegisterOprtInfo(new MapOprtInfo(nameof(LablePropertyModelEdit.BackColor), ResourceA.LableBackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
			{
				new MapOprtCellInstInfo()
				{
				  InstanceID=Guid.NewGuid(),
				  OprtCellID=LableMapOprtCellConst.BackColor_MapOprtCellID,
				  CfgInfo=null
				}
			}));
			(this as IMapCellTypeBase).Name = ResourceA.Button;
			lableViewModel = new LableViewModel(designTime, LablePropertyModelEdit);
			view.DataContext = lableViewModel;
		}
		public LablePropertyModelEdit LablePropertyModelEdit
		{
			get { return PropertyEditModelBase as LablePropertyModelEdit; }
		}

		public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
		{
			LablePropertyModelEdit.IsRuning = isRuning;
			return LablePropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
		}

        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);			
			var propertyEditModelBase = JsonObjConvert.FromJSon<LablePropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
			(PropertyEditModelBase as LablePropertyModelEdit).CopyFrom(propertyEditModelBase);
			var propertyBindEditModelBase = JsonObjConvert.FromJSon<LablePropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
			(PropertyBindEditModelBase as LablePropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
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
			MapCellLableCtlObj mapCellLableCtlObj = (source as MapCellLableCtlObj);
			base._CopyFrom(mapCellLableCtlObj);
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
			return lableViewModel;
		}

		/// <summary>
		/// 创建图元属性编辑模型对象
		/// </summary>
		/// <returns>图元属性编辑模型对象</returns>
		public override PropertyEditModelBase CreatePropertyModelEditBase()
		{
			return new LablePropertyModelEdit();
		}

		/// <summary>
		/// 创建图元属性绑定编辑模型对象
		/// </summary>
		/// <returns>图元属性绑定编辑模型对象</returns>
		public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
		{
			return new LablePropertyBindEditModel();
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
			if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.LableValue_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new LableValueMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.LableColor_MapOprtCellID)
			{
				if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
				{
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				else
				{
					mapOprtCellExector = new LableColorMapOprtCellExector(this);
					mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
					MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
					mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
				}
				return true;
			}
			if (mapOprtCellInstInfo.OprtCellID == LableMapOprtCellConst.BackColor_MapOprtCellID)
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
				LablePropertyModelEdit.SetPropertyValue(gFBaseTypePropValue.PropertyID.ToString(), gFBaseTypePropValue.Value);
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
			if (string.Compare(propertyID, nameof(LablePropertyModelEdit.LableValue)) == 0)
			{
				CallBack?.ExecOprt(nameof(LablePropertyModelEdit.LableValue));
			}
			if (string.Compare(propertyID, nameof(LablePropertyModelEdit.LableColor)) == 0)
			{
				CallBack?.ExecOprt(nameof(LablePropertyModelEdit.LableColor));
			}
			if (string.Compare(propertyID, nameof(LablePropertyModelEdit.BackColor)) == 0)
			{
				CallBack?.ExecOprt(nameof(LablePropertyModelEdit.BackColor));
			}
			if (!LablePropertyModelEdit.IsRuning)
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
            return "标签";
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
			return null;
        }


        #region 操作原子执行对象

        /// <summary>
        /// 标签值操作原子执行对象
        /// </summary>
        private class LableValueMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellLableCtlObj mapCellLableCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public LableValueMapOprtCellExector(MapCellLableCtlObj mapCellLableCtlObj)
			{
				this.mapCellLableCtlObj = mapCellLableCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is LableViewModel lableViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.LableValue));
					if (mapCellPropValue is { })
					{
						lableViewModel.LableText = mapCellPropValue.ToPrimitiveValue<string>();
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 标签颜色操作原子执行对象
		/// </summary>
		private class LableColorMapOprtCellExector : IMapOprtCellExector
		{
			private MapCellLableCtlObj mapCellLableCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public LableColorMapOprtCellExector(MapCellLableCtlObj mapCellLableCtlObj)
			{
				this.mapCellLableCtlObj = mapCellLableCtlObj;
			}
			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is LableViewModel lableViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.LableColor));
					if (mapCellPropValue is { })
					{
						var color = mapCellPropValue.ToPrimitiveValue<string>();
						lableViewModel.LableColor = Color.Parse(color);
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
			private MapCellLableCtlObj mapCellLableCtlObj;

			private IMapOprtCellExectorCallBack callBack;

			public BackColorMapOprtCellExector(MapCellLableCtlObj mapCellLableCtlObj)
			{
				this.mapCellLableCtlObj = mapCellLableCtlObj;
			}

			#region  IMapOprtCellExector 成员

			void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
			{
				this.callBack = callBack;
			}

			void IMapOprtCellExector.Exec(byte[] cfg)
			{
				object viewModel = callBack.GetMapCellVMObjInstance();
				if (viewModel != null && viewModel is LableViewModel lableViewModel)
				{
					GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(LablePropertyModelEdit.BackColor));
					if (mapCellPropValue is { })
					{
						var color = mapCellPropValue.ToPrimitiveValue<string>();
						lableViewModel.BackColor = Color.Parse(color);
					}
				}
			}

			#endregion
		}

		#endregion
	}


	/// <summary>
	/// 标签属性编辑模型对象
	/// </summary>
	[Serializable]
	[MapPropertyOrder] 
	[CategoryPriority("图元信息",1)] 
	public class LablePropertyModelEdit : ControlCellPropertyModelEdit
	{
		public LablePropertyModelEdit() 
		{
			Styles.Add(ResourceA.LableStyle1);
		}

		private string _lableValue;
		/// <summary>
		/// 标签值
		/// </summary>
		[DisplayName("标签值")]
		[Category("图元信息")]
		[PropertySortOrder(15)]
		public string LableValue
		{
			get
			{
				return _lableValue;
			}
			set
			{
				SetProperty(ref _lableValue, value);
			}
		}

		private Color _lableColor = Colors.Black;
		/// <summary>
		/// 标签颜色
		/// </summary>
		[DisplayName("标签颜色")]
		[Category("图元信息")]
		[PropertySortOrder(16)]
		[JsonConverter(typeof(ColorConvert))]
		public Color LableColor
		{
			get
			{
				return _lableColor;
			}
			set
			{
				SetProperty(ref _lableColor, value);
			}
		}

		private Color _backColor = Color.FromArgb(33, 0, 0, 0);
		/// <summary>
		/// 背景颜色
		/// </summary>
		[DisplayName("背景颜色")]
		[Category("图元信息")]
		[PropertySortOrder(17)]
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

		/// <summary>
		/// 设置属性值
		/// </summary>
		/// <param name="propertyID">属性ID</param>
		/// <param name="propertyVal">属性值</param>
		/// <returns>True:已经找到该属性并设置，false:没有该属性</returns>
		public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
		{
			if (string.Compare(propertyID, nameof(LableValue)) == 0)
			{
				if (propertyVal is { })
				{
					LableValue = propertyVal.ToPrimitiveValue<string>();
				}
				else
				{
					LableValue = "";
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
			if (string.Compare(propertyID, nameof(LableColor)) == 0)
			{
				if (propertyVal is { })
				{
					var color1 = propertyVal.ToPrimitiveValue<string>();
					LableColor = Color.Parse(color1);
				}
				else
				{
					LableColor = Colors.Black;
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
		public void CopyFrom(LablePropertyModelEdit source)
		{
			base.CopyFrom(source);
			this.LableValue = source.LableValue;
			this.LableColor = source.LableColor;
			this.BackColor = source.BackColor;
		}
	}

	/// <summary>
	/// 标签图元属性绑定编辑模型对象，可由图元继承
	/// </summary>
	[Serializable]
	[MapPropertyOrder]
	[CategoryPriority("界面数据", 1)]
	[CategoryPriority("绑定信息", 2)]
	public class LablePropertyBindEditModel : ControlCellPropertyBindEditModel
	{
		private PropertyBindInfo _lableValue = new PropertyBindInfo(GriffinsBaseDataType.String);
		/// <summary>
		/// 标签值
		/// </summary>
		[DisplayName("标签值")]
		[Category("绑定信息")]
		[PropertySortOrder(6)]
		[BindMPPropertyID]
		public PropertyBindInfo LableValue
		{
			get
			{
				return _lableValue;
			}
			set
			{
				SetProperty(ref _lableValue, value);
			}
		}

		private PropertyBindInfo _lableColor = new PropertyBindInfo(GriffinsBaseDataType.String);
		/// <summary>
		/// 标签颜色
		/// </summary>
		[DisplayName("标签颜色")]
		[Category("绑定信息")]
		[PropertySortOrder(8)]
		[BindMPPropertyID]
		public PropertyBindInfo LableColor
		{
			get
			{
				return _lableColor;
			}
			set
			{
				SetProperty(ref _lableColor, value);
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

		/// <summary>
		/// 从来源实例复制字段到本实例
		/// </summary>
		/// <param name="source">来源实例</param>
		public void CopyFrom(LablePropertyBindEditModel source)
		{
			base.CopyFrom(source);
			this.LableValue.CopyFrom(source.LableValue);
			this.LableColor.CopyFrom(source.LableColor);
			this.BackColor.CopyFrom(source.BackColor);
		}
	}
}
