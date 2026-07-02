using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using Newtonsoft.JsonG;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮杂项信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class IconButtonMiscInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{7A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5EC}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly IconButtonMiscInfo Default = new IconButtonMiscInfo();

        private BitmapData _iconSource;
        private string _iconSourceBase64 = "";
        private bool _isUpdatingIconSource;
        private bool _isUpdatingIconSourceBase64;
        private BitmapData _backgroundImage;
        private string _backgroundImageBase64 = "";
        private bool _isUpdatingBackgroundImage;
        private bool _isUpdatingBackgroundImageBase64;
        private int _iconWidth = 24;
        private int _iconHeight = 24;
        private int _iconMargin = 0;
        private int _iconMarginLeft = 0;
        private int _iconMarginTop = 0;
        private int _iconMarginRight = 0;
        private int _iconMarginBottom = 0;
        private IconPositionEnum _iconPosition = IconPositionEnum.Left;
        private ImageSizeMode _imageSizeMode = ImageSizeMode.None;

        /// <summary>
        /// Icon源
        /// </summary>
        [DisplayName("Icon源")]
        [Category("杂项")]
        [JsonIgnore]
        public BitmapData IconSource
        {
            get
            {
                if ((_iconSource == null || _iconSource.Bitmap == null) && !string.IsNullOrWhiteSpace(_iconSourceBase64))
                {
                    try
                    {
                        var bytes = System.Convert.FromBase64String(_iconSourceBase64);
                        _iconSource = new BitmapData();
                        _iconSource.FromBytes(bytes);
                    }
                    catch
                    {
                        _iconSource = null;
                    }
                }
                return _iconSource;
            }
            set
            {
                if (_isUpdatingIconSource)
                    return;

                _isUpdatingIconSource = true;
                try
                {
                    var normalized = value;
                    if (normalized == null)
                    {
                        var changed = _iconSource != null;
                        _iconSource = null;
                        _iconSourceBase64 = "";
                        if (changed)
                        {
                            RaisePropertyChanged(nameof(IconSource));
                            RaisePropertyChanged(nameof(IconSourceBase64));
                        }
                        return;
                    }

                    SetProperty(ref _iconSource, normalized);

                    try
                    {
                        if (normalized != null && normalized.Bitmap != null)
                        {
                            var bytes = normalized.ToBytes();
                            _iconSourceBase64 = bytes != null && bytes.Length > 0 ? System.Convert.ToBase64String(bytes) : "";
                        }
                        else
                        {
                            _iconSourceBase64 = "";
                        }
                    }
                    catch
                    {
                        _iconSourceBase64 = "";
                    }
                    finally
                    {
                        RaisePropertyChanged(nameof(IconSourceBase64));
                    }
                }
                finally
                {
                    _isUpdatingIconSource = false;
                }
            }
        }

        [Browsable(false)]
        public string IconSourceBase64
        {
            get
            {
                try
                {
                    if (_iconSource != null && _iconSource.Bitmap != null)
                    {
                        var bytes = _iconSource.ToBytes();
                        if (bytes != null && bytes.Length > 0)
                        {
                            _iconSourceBase64 = System.Convert.ToBase64String(bytes);
                            return _iconSourceBase64;
                        }
                    }
                }
                catch
                {
                }

                return _iconSourceBase64 ?? "";
            }
            set
            {
                if (_isUpdatingIconSourceBase64)
                    return;

                _isUpdatingIconSourceBase64 = true;
                try
                {
                    _iconSourceBase64 = value ?? "";
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        IconSource = null;
                        return;
                    }

                    var bytes = System.Convert.FromBase64String(value);
                    var bitmapData = new BitmapData();
                    bitmapData.FromBytes(bytes);
                    IconSource = bitmapData;
                }
                catch
                {
                    _iconSourceBase64 = "";
                    IconSource = null;
                }
                finally
                {
                    _isUpdatingIconSourceBase64 = false;
                }
            }
        }

        /// <summary>
        /// 背景图片
        /// </summary>
        [DisplayName("背景图片")]
        [Category("杂项")]
        [JsonIgnore]
        public BitmapData BackgroundImage
        {
            get
            {
                if ((_backgroundImage == null || _backgroundImage.Bitmap == null) && !string.IsNullOrWhiteSpace(_backgroundImageBase64))
                {
                    try
                    {
                        var bytes = System.Convert.FromBase64String(_backgroundImageBase64);
                        _backgroundImage = new BitmapData();
                        _backgroundImage.FromBytes(bytes);
                    }
                    catch
                    {
                        _backgroundImage = null;
                    }
                }
                return _backgroundImage;
            }
            set
            {
                if (_isUpdatingBackgroundImage)
                    return;

                _isUpdatingBackgroundImage = true;
                try
                {
                    var normalized = value;
                    if (normalized == null)
                    {
                        var changed = _backgroundImage != null;
                        _backgroundImage = null;
                        _backgroundImageBase64 = "";
                        if (changed)
                        {
                            RaisePropertyChanged(nameof(BackgroundImage));
                            RaisePropertyChanged(nameof(BackgroundImageBase64));
                        }
                        return;
                    }

                    SetProperty(ref _backgroundImage, normalized);

                    try
                    {
                        if (normalized.Bitmap != null)
                        {
                            var bytes = normalized.ToBytes();
                            _backgroundImageBase64 = bytes != null && bytes.Length > 0 ? System.Convert.ToBase64String(bytes) : "";
                        }
                        else
                        {
                            _backgroundImageBase64 = "";
                        }
                    }
                    catch
                    {
                        _backgroundImageBase64 = "";
                    }
                    finally
                    {
                        RaisePropertyChanged(nameof(BackgroundImageBase64));
                    }
                }
                finally
                {
                    _isUpdatingBackgroundImage = false;
                }
            }
        }

        [Browsable(false)]
        public string BackgroundImageBase64
        {
            get
            {
                try
                {
                    if (_backgroundImage != null && _backgroundImage.Bitmap != null)
                    {
                        var bytes = _backgroundImage.ToBytes();
                        if (bytes != null && bytes.Length > 0)
                        {
                            _backgroundImageBase64 = System.Convert.ToBase64String(bytes);
                            return _backgroundImageBase64;
                        }
                    }
                }
                catch
                {
                }

                return _backgroundImageBase64 ?? "";
            }
            set
            {
                if (_isUpdatingBackgroundImageBase64)
                    return;

                _isUpdatingBackgroundImageBase64 = true;
                try
                {
                    _backgroundImageBase64 = value ?? "";
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        BackgroundImage = null;
                        return;
                    }

                    var bytes = System.Convert.FromBase64String(value);
                    var bitmapData = new BitmapData();
                    bitmapData.FromBytes(bytes);
                    BackgroundImage = bitmapData;
                }
                catch
                {
                    _backgroundImageBase64 = "";
                    BackgroundImage = null;
                }
                finally
                {
                    _isUpdatingBackgroundImageBase64 = false;
                }
            }
        }

        /// <summary>
        /// Icon宽度
        /// </summary>
        [DisplayName("宽度")]
        [Category("杂项.Icon大小")]
        [Range(0, 1000)]
        public int IconWidth
        {
            get => _iconWidth;
            set => SetProperty(ref _iconWidth, value);
        }

        /// <summary>
        /// Icon高度
        /// </summary>
        [DisplayName("高度")]
        [Category("杂项.Icon大小")]
        [Range(0, 1000)]
        public int IconHeight
        {
            get => _iconHeight;
            set => SetProperty(ref _iconHeight, value);
        }

        /// <summary>
        /// Icon外边距
        /// </summary>
        [DisplayName("Icon外边距")]
        [Category("杂项")]
        public int IconMargin
        {
            get => _iconMargin;
            set
            {
                if (SetProperty(ref _iconMargin, value))
                {
                    IconMarginLeft = value;
                    IconMarginTop = value;
                    IconMarginRight = value;
                    IconMarginBottom = value;
                }
            }
        }

        /// <summary>
        /// Icon左外边距
        /// </summary>
        [DisplayName("左")]
        [Category("杂项.Icon外边距")]
        public int IconMarginLeft
        {
            get => _iconMarginLeft;
            set => SetProperty(ref _iconMarginLeft, value);
        }

        /// <summary>
        /// Icon上外边距
        /// </summary>
        [DisplayName("上")]
        [Category("杂项.Icon外边距")]
        public int IconMarginTop
        {
            get => _iconMarginTop;
            set => SetProperty(ref _iconMarginTop, value);
        }

        /// <summary>
        /// Icon右外边距
        /// </summary>
        [DisplayName("右")]
        [Category("杂项.Icon外边距")]
        public int IconMarginRight
        {
            get => _iconMarginRight;
            set => SetProperty(ref _iconMarginRight, value);
        }

        /// <summary>
        /// Icon下外边距
        /// </summary>
        [DisplayName("下")]
        [Category("杂项.Icon外边距")]
        public int IconMarginBottom
        {
            get => _iconMarginBottom;
            set => SetProperty(ref _iconMarginBottom, value);
        }

        /// <summary>
        /// Icon位置
        /// </summary>
        [DisplayName("Icon位置")]
        [Category("杂项")]
        public IconPositionEnum IconPosition
        {
            get => _iconPosition;
            set => SetProperty(ref _iconPosition, value);
        }

        /// <summary>
        /// 背景图片大小模式
        /// </summary>
        [DisplayName("背景图大小模式")]
        [Category("杂项")]
        public ImageSizeMode ImageSizeMode
        {
            get => _imageSizeMode;
            set => SetProperty(ref _imageSizeMode, value);
        }

        /// <summary>
        /// Icon位置枚举
        /// </summary>
        public enum IconPositionEnum
        {
            /// <summary>
            /// 居文字左
            /// </summary>
            Left,
            /// <summary>
            /// 居文字右
            /// </summary>
            Right,
            /// <summary>
            /// 居文字上
            /// </summary>
            Top,
            /// <summary>
            /// 居文字下
            /// </summary>
            Bottom
        }

        #region IMPPropObjectValue 实现

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                    throw new Exception("对象值不是IconButtonMiscInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是IconButtonMiscInfo转换的");
                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        #endregion

        #region IJsonValueConvert 实现

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;

            JsonElement value;
            if (rootElement.TryGetProperty("IconSourceBase64", out value))
            {
                try
                {
                    var base64 = value.GetString();
                    _iconSourceBase64 = base64 ?? "";
                    if (!string.IsNullOrWhiteSpace(base64))
                    {
                        var bytes = System.Convert.FromBase64String(base64);
                        _iconSource = new BitmapData();
                        _iconSource.FromBytes(bytes);
                    }
                    else
                    {
                        _iconSource = null;
                    }
                }
                catch
                {
                    _iconSourceBase64 = "";
                    _iconSource = null;
                }
            }
            if (rootElement.TryGetProperty("BackgroundImageBase64", out value))
            {
                try
                {
                    var base64 = value.GetString();
                    _backgroundImageBase64 = base64 ?? "";
                    if (!string.IsNullOrWhiteSpace(base64))
                    {
                        var bytes = System.Convert.FromBase64String(base64);
                        _backgroundImage = new BitmapData();
                        _backgroundImage.FromBytes(bytes);
                    }
                    else
                    {
                        _backgroundImage = null;
                    }
                }
                catch
                {
                    _backgroundImageBase64 = "";
                    _backgroundImage = null;
                }
            }
            if (rootElement.TryGetProperty("IconWidth", out value))
                _iconWidth = value.GetInt32();
            if (rootElement.TryGetProperty("IconHeight", out value))
                _iconHeight = value.GetInt32();
            if (rootElement.TryGetProperty("IconMargin", out value))
                _iconMargin = value.GetInt32();
            if (rootElement.TryGetProperty("IconMarginLeft", out value))
                _iconMarginLeft = value.GetInt32();
            if (rootElement.TryGetProperty("IconMarginTop", out value))
                _iconMarginTop = value.GetInt32();
            if (rootElement.TryGetProperty("IconMarginRight", out value))
                _iconMarginRight = value.GetInt32();
            if (rootElement.TryGetProperty("IconMarginBottom", out value))
                _iconMarginBottom = value.GetInt32();
            if (rootElement.TryGetProperty("IconPosition", out value))
            {
                if (value.ValueKind == JsonValueKind.String && Enum.TryParse<IconPositionEnum>(value.GetString(), true, out var iconPosition))
                    _iconPosition = iconPosition;
                else if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var iconPositionInt))
                    _iconPosition = (IconPositionEnum)iconPositionInt;
            }
            if (rootElement.TryGetProperty("ImageSizeMode", out value))
            {
                if (value.ValueKind == JsonValueKind.String && Enum.TryParse<ImageSizeMode>(value.GetString(), true, out var imageSizeMode))
                    _imageSizeMode = imageSizeMode;
                else if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var imageSizeModeInt))
                    _imageSizeMode = (ImageSizeMode)imageSizeModeInt;
            }
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            string iconSourceBase64 = _iconSourceBase64 ?? "";
            if (string.IsNullOrWhiteSpace(iconSourceBase64))
            {
                try
                {
                    if (_iconSource != null)
                    {
                        var bytes = _iconSource.ToBytes();
                        if (bytes != null && bytes.Length > 0)
                        {
                            iconSourceBase64 = System.Convert.ToBase64String(bytes);
                        }
                    }
                }
                catch
                {
                    iconSourceBase64 = "";
                }
            }

            string backgroundImageBase64 = _backgroundImageBase64 ?? "";
            if (string.IsNullOrWhiteSpace(backgroundImageBase64))
            {
                try
                {
                    if (_backgroundImage != null)
                    {
                        var bytes = _backgroundImage.ToBytes();
                        if (bytes != null && bytes.Length > 0)
                        {
                            backgroundImageBase64 = System.Convert.ToBase64String(bytes);
                        }
                    }
                }
                catch
                {
                    backgroundImageBase64 = "";
                }
            }

            var value = new
            {
                IconSourceBase64 = iconSourceBase64,
                BackgroundImageBase64 = backgroundImageBase64,
                IconWidth = _iconWidth,
                IconHeight = _iconHeight,
                IconMargin = _iconMargin,
                IconMarginLeft = _iconMarginLeft,
                IconMarginTop = _iconMarginTop,
                IconMarginRight = _iconMarginRight,
                IconMarginBottom = _iconMarginBottom,
                IconPosition = (int)_iconPosition,
                ImageSizeMode = _imageSizeMode.ToString()
            };
            return JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
