using GKG.Station.Server.Online;
using Griffins;
using Griffins.PF.DB;
using Griffins.PF.Server.DB;

namespace GKG.Station.Server.Data
{
    /// <summary>
    /// 工位信息表数据访问层
    /// </summary>
    class TDA_StationInfo : PrjRuntimeTableDataAccessBase
    {
        #region 构造函数

        public TDA_StationInfo(string projectID)
            : base(projectID, StationDBConst.TableAlias, StationOnlineSvrMain.CallForAppSvr.GetCreatePrjRuntimeDbAccessDele())
        {
        }

        public TDA_StationInfo(string projectID, GriffinsDbConnection dbConnection)
            : base(projectID, StationDBConst.TableAlias, dbConnection, StationOnlineSvrMain.CallForAppSvr.GetCreatePrjRuntimeDbAccessDele())
        {
        }

        #endregion

        #region 核心数据操作 (CRUD)

        /// <summary>
        /// 查询指定条件的工位信息列表
        /// </summary>
        public StationInfoList SearchStationInfos(GriffinsSQLWhereParameterCollection whereParameters, GriffinsDbTransaction transaction = null!)
        {
            StationInfoList infos = new StationInfoList();
            UnfixedListObjBindInfo listobjBindInfo = new(infos, typeof(StationInfo));

            bind(listobjBindInfo);
            TableObjDataAccess.ReadRows(listobjBindInfo, whereParameters, transaction);

            return infos;
        }

        /// <summary>
        /// 添加或更新工位信息
        /// </summary>
        public void UpdateStationInfo(StationInfo info, GriffinsDbTransaction transaction = null!)
        {
            ObjBindInfo objBindInfo = new(info);
            bind(objBindInfo);
            TableObjDataAccess.UpdateTable(objBindInfo, transaction);
        }

        /// <summary>
        /// 删除工位信息
        /// </summary>
        public void DeleteStationInfos(GriffinsSQLWhereParameterCollection whereParameters, GriffinsDbTransaction dbTransaction = null!)
            => TableObjDataAccess.DeleteRows(whereParameters, dbTransaction);

        /// <summary>
        /// 查询最新一条数据
        /// </summary>
        public StationInfo GetLatestStationInfo(string stationAlias, GriffinsDbTransaction transaction = null!)
        {
            StationInfoList infos = new();
            UnfixedListObjBindInfo listobjBindInfo = new(infos, typeof(StationInfo));
            bind(listobjBindInfo);

            GriffinsSQLWhereParameterCollection where = new();
            if (!string.IsNullOrEmpty(stationAlias))
            {
                where.Add(StationDBConst.FIELD_StationAlias, new GriffinsSQLWhereParameter(GriffinsCompareMethod.Equal, stationAlias));
            }

            bool isLastPage;
            TableObjDataAccess.ReadRows(
                listobjBindInfo,        // 绑定对象
                where,                  // 查询条件
                1,                      // pageSize: 页大小设为 1 (相当于 TOP 1)
                1,                      // pageIndex: 取第 1 页
                new string[] { StationDBConst.FIELD_InTime }, // 排序字段：按进板时间
                false,                  // aSC: 是否升序？设为 false 表示降序 (DESC)
                out isLastPage,         // 输出参数：是否最后一页
                transaction             // 事务对象
            );

            if (infos.Count > 0)
            {
                return (StationInfo)infos[0];
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
            objBindInfo.Bind(StationDBConst.FIELD_RecordID, nameof(StationInfo.RecordID), Guid.Empty);
            objBindInfo.Bind(StationDBConst.FIELD_StationAlias, nameof(StationInfo.StationAlias), string.Empty);
            objBindInfo.Bind(StationDBConst.FIELD_ProductBatch, nameof(StationInfo.ProductBatch), string.Empty);
            objBindInfo.Bind(StationDBConst.FIELD_ProductID, nameof(StationInfo.ProductID), string.Empty);
            objBindInfo.Bind(StationDBConst.FIELD_InDuration, nameof(StationInfo.InDuration), 0.0);
            objBindInfo.Bind(StationDBConst.FIELD_InTime, nameof(StationInfo.InTime), DateTime.UtcNow);
            objBindInfo.Bind(StationDBConst.FIELD_StopDuration, nameof(StationInfo.StopDuration), 0.0);
            objBindInfo.Bind(StationDBConst.FIELD_StopTime, nameof(StationInfo.StopTime), DateTime.UtcNow);
            objBindInfo.Bind(StationDBConst.FIELD_OutDuration, nameof(StationInfo.OutDuration), 0.0);
            objBindInfo.Bind(StationDBConst.FIELD_OutTime, nameof(StationInfo.OutTime), DateTime.UtcNow);
        }

        #endregion
    }
}