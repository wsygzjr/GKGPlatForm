using Avalonia.Controls;
using GKG.UI;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;
using System;
namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Tools
{
    /// <summary>
    /// DataGrid项-视图模型基类（绑定实体基类）
    /// </summary>
    /// <typeparam name="TEntity">实体类型（继承EntityBase）</typeparam>
    public abstract class DataGridItemBaseViewModel<TEntity> : ReactiveObject
        where TEntity : EntityBase, new()
    {
        protected Control? _viewReference;
        private bool _isSelected;

        /// <summary>
        /// 是否勾选属性
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                this.RaiseAndSetIfChanged(ref _isSelected, value);

                IsSelectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 序号-数据模型
        /// </summary>
        public TextBlockViewModel SerialNumberViewModel { get; } = new TextBlockViewModel();

        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber
        {
            get => int.TryParse(SerialNumberViewModel.Text, out var num) ? num : 0;
            set
            {
                SerialNumberViewModel.Text = value.ToString();
                this.RaisePropertyChanged(nameof(SerialNumber));
            }
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid ID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 是否选中的变更事件
        /// </summary>
        public event EventHandler? IsSelectedChanged;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 从实体复制数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public abstract void CopyFrom(TEntity entity);

        /// <summary>
        /// 将数据复制到实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        public abstract void CopyTo(TEntity entity);

        /// <summary>
        /// 复制基类属性
        /// </summary>
        /// <param name="entity">实体对象</param>
        protected void CopyBasePropertiesFrom(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ID = entity.ID;
            SerialNumber = entity.SerialNumber;
        }

        /// <summary>
        /// 复制基类属性
        /// </summary>
        /// <param name="entity">实体对象</param>
        protected void CopyBasePropertiesTo(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entity.ID = ID;
            entity.SerialNumber = SerialNumber;
        }

        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// 子类需要设置view的情况子类重写该方法即可
        /// </summary>
        public virtual void SetViewReference(Control view)
        {
            _viewReference = view;
        }

    }

    /// <summary>
    /// 带ID和序号的实体基类
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid ID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { get; set; }
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            SerialNumber = jObject["SerialNumber"]?.Value<int>() ?? 0;

        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject
            {
                { "SerialNumber", (int)SerialNumber },
            };
            return jObject;
        }
    }
}
