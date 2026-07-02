using GKG.ElectronicControl;
using Griffins.PF.Server;
using MaterialBoxSubMachineModules.FeedPort;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GKG.MaterialBoxSubMachineModules.Common
{
    /// <summary>
    /// 送料口/接料口运行时对象。
    /// 负责持有角色、配置以及绑定后的状态 IO，并提供有料状态读取能力。
    /// </summary>
    public class FeedPort
    {
        private FeedPortFactoryCfg factoryCfg;
        private FeedPortInitCfg initCfg;
        private FeedPortPPCfg ppCfg;
        private List<IBaseStateIO> sensorStateIOList = new List<IBaseStateIO>();

        /// <summary>
        /// 创建一个指定角色的送料口/接料口对象。
        /// </summary>
        /// <param name="portRole">料口角色。</param>
        public FeedPort(FeedPortFactoryCfg feedPortFactoryCfg)
        {
            factoryCfg = feedPortFactoryCfg;
            initCfg = new FeedPortInitCfg();
            ppCfg = new FeedPortPPCfg();
        }

        /// <summary>
        /// 当前出厂配置。
        /// </summary>
        public FeedPortFactoryCfg FactoryCfg => factoryCfg;

        /// <summary>
        /// 当前初始化配置。
        /// </summary>
        public FeedPortInitCfg InitCfg => initCfg;

        /// <summary>
        /// 当前配方配置。
        /// </summary>
        public FeedPortPPCfg PPCfg => ppCfg;

        /// <summary>
        /// 设置送料口/接料口出厂配置。
        /// </summary>
        /// <param name="factoryCfg">出厂配置。</param>
        public void SetFactoryCfg(FeedPortFactoryCfg factoryCfg)
        {
            this.factoryCfg = factoryCfg ?? new FeedPortFactoryCfg();
        }

        /// <summary>
        /// 按初始化配置绑定料口检测 IO。
        /// </summary>
        /// <param name="initCfg">初始化配置。</param>
        public void Init(FeedPortInitCfg initCfg, Func<List<Guid>, List<IBaseStateIO>> getIOStateList)
        {
            this.initCfg = initCfg ?? new FeedPortInitCfg();
            sensorStateIOList = new List<IBaseStateIO>();
            sensorStateIOList.AddRange(getIOStateList(this.initCfg.SensorIOGuids));
        }

        /// <summary>
        /// 设置送料口/接料口配方配置。
        /// </summary>
        /// <param name="ppCfg">配方配置。</param>
        public void SetPPCfg(FeedPortPPCfg ppCfg)
        {
            this.ppCfg = ppCfg ?? new FeedPortPPCfg();
        }

        /// <summary>
        /// 获取当前送料口/接料口的有料状态。
        /// 会按配方中的物料到位感应时间连续读取绑定 IO 进行判定。
        /// </summary>
        /// <returns>检测到有料返回 true，否则返回 false。</returns>
        public bool GetMaterialState(int index)
        {
            if (sensorStateIOList == null)
                return false;

            if (sensorStateIOList.Count > index)
            {
                return sensorStateIOList[index].Read();
            }
            else
            {
                return false;
            }
        }
    }
}
