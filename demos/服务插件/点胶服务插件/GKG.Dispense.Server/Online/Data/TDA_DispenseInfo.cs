using Griffins.PF.DB;
using Griffins.PF.Server.DB;
using Griffins;
using GKG.Dispense.Server.Online;

namespace GKG.Dispense.Server.Data
{
    /// <summary>
    /// 点胶信息表数据访问层
    /// </summary>
    class TDA_DispenseInfo : PrjRuntimeTableDataAccessBase
    {
        #region 构造函数

        public TDA_DispenseInfo(string projectID)
            : base(projectID, DispenseDBConst.TableAlias, DispenseOnlineSvrMain.CallForAppSvr.GetCreatePrjRuntimeDbAccessDele())
        {
        }

        public TDA_DispenseInfo(string projectID, GriffinsDbConnection dbConnection)
            : base(projectID, DispenseDBConst.TableAlias, dbConnection, DispenseOnlineSvrMain.CallForAppSvr.GetCreatePrjRuntimeDbAccessDele())
        {
        }

        #endregion

        #region 核心数据操作 (CRUD)

        /// <summary>
        /// 查询指定条件的点胶信息列表
        /// </summary>
        public DispenseInfoList SearchDispenseInfos(GriffinsSQLWhereParameterCollection whereParameters, GriffinsDbTransaction transaction = null!)
        {
            DispenseInfoList infos = new DispenseInfoList();
            UnfixedListObjBindInfo listobjBindInfo = new(infos, typeof(DispenseInfo));

            bind(listobjBindInfo);
            TableObjDataAccess.ReadRows(listobjBindInfo, whereParameters, transaction);

            return infos;
        }

        /// <summary>
        /// 添加或更新点胶信息
        /// </summary>
        public void UpdateDispenseInfo(DispenseInfo info, GriffinsDbTransaction transaction = null!)
        {
            ObjBindInfo objBindInfo = new(info);
            bind(objBindInfo);
            TableObjDataAccess.UpdateTable(objBindInfo, transaction);
        }

        /// <summary>
        /// 删除点胶信息
        /// </summary>
        public void DeleteDispenseInfos(GriffinsSQLWhereParameterCollection whereParameters, GriffinsDbTransaction dbTransaction = null!)
            => TableObjDataAccess.DeleteRows(whereParameters, dbTransaction);

        /// <summary>
        /// 查询最新一条数据
        /// </summary>
        public DispenseInfo GetLatestDispenseInfo(string moduleAlias, GriffinsDbTransaction transaction = null!)
        {
            DispenseInfoList infos = new();
            UnfixedListObjBindInfo listobjBindInfo = new(infos, typeof(DispenseInfo));
            bind(listobjBindInfo);

            GriffinsSQLWhereParameterCollection where = new();
            if (!string.IsNullOrEmpty(moduleAlias))
            {
                where.Add(DispenseDBConst.FIELD_ModuleAlias, new GriffinsSQLWhereParameter(GriffinsCompareMethod.Equal, moduleAlias));
            }

            bool isLastPage;
            TableObjDataAccess.ReadRows(
                listobjBindInfo,        // 绑定对象
                where,                  // 查询条件
                1,                      // pageSize: 页大小设为 1 (相当于 TOP 1)
                1,                      // pageIndex: 取第 1 页
                new string[] { DispenseDBConst.FIELD_DispenseTime }, // 排序字段：按进板时间
                false,                  // aSC: 是否升序？设为 false 表示降序 (DESC)
                out isLastPage,         // 输出参数：是否最后一页
                transaction             // 事务对象
            );

            if (infos.Count > 0)
            {
                return (DispenseInfo)infos[0];
            }

            return null!;
        }

        #endregion

        #region ORM 映射引擎配置

        /// <summary>
        /// 建立数据库字段与 C# 实体属性的映射桥梁
        /// </summary>
        private void bind(ObjBindInfoBase objBindInfo)
        {
            objBindInfo.Bind(DispenseDBConst.FIELD_RecordID, nameof(DispenseInfo.RecordID), Guid.Empty);
            objBindInfo.Bind(DispenseDBConst.FIELD_ModuleAlias, nameof(DispenseInfo.ModuleAlias), string.Empty);
            objBindInfo.Bind(DispenseDBConst.FIELD_ProductBatch, nameof(DispenseInfo.ProductBatch), string.Empty);
            objBindInfo.Bind(DispenseDBConst.FIELD_ProductID, nameof(DispenseInfo.ProductID), string.Empty);
            objBindInfo.Bind(DispenseDBConst.FIELD_MarkTime, nameof(DispenseInfo.MarkTime), DateTime.UtcNow);
            objBindInfo.Bind(DispenseDBConst.FIELD_MarkDuration, nameof(DispenseInfo.MarkDuration), 0.0);
            objBindInfo.Bind(DispenseDBConst.FIELD_DispenseTime, nameof(DispenseInfo.DispenseTime), DateTime.UtcNow);
            objBindInfo.Bind(DispenseDBConst.FIELD_DispenseDuration, nameof(DispenseInfo.DispenseDuration), 0.0);
            objBindInfo.Bind(DispenseDBConst.FIELD_ProcessDuration, nameof(DispenseInfo.ProcessDuration), 0.0);
        }

        #endregion
    }
}