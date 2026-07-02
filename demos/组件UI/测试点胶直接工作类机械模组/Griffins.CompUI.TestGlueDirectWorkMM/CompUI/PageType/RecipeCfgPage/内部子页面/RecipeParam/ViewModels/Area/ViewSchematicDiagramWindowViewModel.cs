using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 示意图-视图模型
    /// </summary>
    public class ViewSchematicDiagramWindowViewModel : ReactiveObject
    {
        /// <summary>
        /// 点胶指令模板实例列表
        /// </summary>
        public List<DispensingTemplateInstanceExecutionObject> Instances { get; }
        private bool? _dialogResult;
       
        /// <summary>
        /// 对话框结果（true:保存，false:取消，null:未操作）
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }
         
        // 顶部表头数据源（动态生成列1、列2...）
        public List<string> TopHeaders { get; }

        // 左侧表头数据源（字母行：A、B、C）
        public List<string> LeftHeaders { get; }

        public int RowCount { get; set; }   // 3行，可按需修改  

        public int ColumnCount { get; set; }   // 2列，可按需修改
          
        /// <summary>
        /// 构造函数
        /// </summary>
        public ViewSchematicDiagramWindowViewModel(List<DispensingTemplateInstanceExecutionObject> instances)
        {
            Instances= instances;
            // ========== 1. 生成顶部表头（按列数） ==========
            ColumnCount = 2;
            TopHeaders = Enumerable.Range(1, ColumnCount)
                                  .Select(i => $"列{i}")
                                  .ToList();

            // ========== 动态生成左侧表头（字母顺序：A、B、C） ==========  
            // 计算行数（向上取整）（ 无浮点误差，避免除以0）
            RowCount = ColumnCount == 0 ? 0 : (Instances.Count + ColumnCount - 1) / ColumnCount;

            LeftHeaders = Enumerable.Range(0, RowCount)
                                   .Select(i => ((char)('A' + i)).ToString()) // 从A开始生成字母
                                   .ToList(); 
        }
        /// <summary>
        /// /保存逻辑
        /// </summary>
        private void save()
        {
            DialogResult = true;
        }

        /// <summary>
        /// 取消逻辑
        /// </summary>
        private void cancel()
        {
            DialogResult = false;
        }
        
    }
}
