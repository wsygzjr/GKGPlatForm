using GF_Gereric;
using GKG.ElectronicControl.General;

namespace GKG.ElectronicControl.Dispenser
{
    /// <summary>
    /// GKG供胶装置初始化参数
    /// </summary>
    public class GlueDispensingDeviceGKGInitParams
    {
        /// <summary>
        /// 气压控制初始化参数
        /// </summary>
        public byte[] PressureControlInitParams { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// 胶量感应初始化参数
        /// </summary>
        public string GlueAmountStateInitParams { get; set; } = "";

        public byte[] GlueAmountInitParams { get; set; } = Array.Empty<byte>();
    }

    /// <summary>
    /// 胶量感应参数结构(时间)
    /// </summary>
    public class GlueAmountTime
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = false;

        /// <summary>
        /// 总时间(h)
        /// </summary>
        public double TotalTime { get; set; } = 0;

        /// <summary>
        /// 剩余时间(h)
        /// </summary>
        public double RemainingTime { get; set; } = 0;
    }

    /// <summary>
    /// 胶水余量报警类型枚举
    /// </summary>
    public enum GlueAmountWeightAlarmType
    {
        Percent = 0,
        Weight = 1,
    }

    /// <summary>
    /// 胶水余量报警参数结构
    /// </summary>
    public class GlueAmountWeightAlarm
    {
        /// <summary>
        /// 报警类型
        /// </summary>
        public GlueAmountWeightAlarmType WeightAlarmType { get; set; } = GlueAmountWeightAlarmType.Percent;

        /// <summary>
        /// 报警阈值
        /// </summary>
        public double GlueAmountWeightAlarmValue { get; set; } = 0;
    }

    /// <summary>
    /// 胶量感应参数结构(重量)
    /// </summary>
    public class GlueAmountWeight
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = false;

        /// <summary>
        /// 总重量(mg)
        /// </summary>
        public double TotalWeight { get; set; } = 0;

        /// <summary>
        /// 剩余重量(mg)
        /// </summary>
        public double RemainingWeight { get; set; } = 0;

        public GlueAmountWeightAlarm glueAmountWeightAlarm { get; set; } = new GlueAmountWeightAlarm();
    }

    /// <summary>
    /// 胶量感应参数结构(板数)
    /// </summary>
    public class GlueAmountPcs
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = false;

        /// <summary>
        /// 总板数
        /// </summary>
        public int TotalPcs { get; set; } = 0;

        /// <summary>
        /// 剩余板数
        /// </summary>
        public int RemainingPcs { get; set; } = 0;
    }

    /// <summary>
    /// 胶量检测参数
    /// </summary>
    public class GlueAmountParams
    {
        public GlueAmountTime GlueAmountTimeParams { get; set; } = new GlueAmountTime();
        public GlueAmountWeight GlueAmountWeightParams { get; set; } = new GlueAmountWeight();
        public GlueAmountPcs GlueAmountPcsParams { get; set; } = new GlueAmountPcs();
    }

    /// <summary>
    /// 胶水余量监控对象
    /// </summary>
    internal class GlueAmount
    {
        public GlueAmount()
        {
            glueAmountParams = new GlueAmountParams();
        }

        private GlueAmountParams glueAmountParams;
        private DateTime time;
        private int pcs;
        private double weight;
        public double RemainingGlueTime => glueAmountParams.GlueAmountTimeParams.RemainingTime;

        public double RemainingGlueWeight => glueAmountParams.GlueAmountWeightParams.RemainingWeight;

        public int RemainingGluePcs => glueAmountParams.GlueAmountPcsParams.RemainingPcs;

        public void Init(byte[] initParams)
        {
            // 开线程监测胶水余量
            glueAmountParams = JsonObjConvert.FromJSonBytes<GlueAmountParams>(initParams);
            time = DateTime.Now;
            // 监听事件 PcsChanged;
            //TestCallBack.PcsChangedEvent += PcsChanged;
            // 监听事件 WeightChanged;
            //TestCallBack.WeightChangedEvent += WeightChanged;
            Task.Run(() => { ThreadGlueAmount(); });
        }

        /// <summary>
        /// 时间清零
        /// </summary>
        public void TimeResetToZero()
        {
            time = DateTime.Now;
        }

        /// <summary>
        /// 重量清零
        /// </summary>
        public void WeightResetToZero()
        {
            // 计数清零
            weight = 0;
        }

        /// <summary>
        /// 板数清零
        /// </summary>
        public void PcsResetToZero()
        {
            // 计数清零
            pcs = 0;
        }

        public void SetGlueAmountParams(GlueAmountParams glueAmountParam)
        {
            glueAmountParams = glueAmountParam;
        }

        public void UpdateGluePcs(int pcs)
        {
            glueAmountParams.GlueAmountPcsParams.RemainingPcs = pcs;
        }

        public void UpdateGlueWeight(double weight)
        {
            glueAmountParams.GlueAmountWeightParams.RemainingWeight = weight;
        }

        /// <summary>
        /// 胶水剩余时间不足事件
        /// </summary>
        public event EventHandler? InsufficientRemainingGlueTime;

        /// <summary>
        /// 胶水剩余重量不足事件(事件参数：剩余重量 mg)
        /// </summary>
        public event EventHandler<double>? InsufficientRemainingGlueWeight;

        /// <summary>
        /// 胶水剩余板数不足事件
        /// </summary>
        public event EventHandler? InsufficientRemainingGluePcs;

        /// <summary>
        /// 胶水剩余时间不足事件触发
        /// </summary>
        private void InsufficientRemainingGlueTimeInvoke()
        {
            InsufficientRemainingGlueTime?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 胶水剩余重量不足事件触发
        /// </summary>
        private void InsufficientRemainingGlueWeightInvoke()
        {
            InsufficientRemainingGlueWeight?.Invoke(this, glueAmountParams.GlueAmountWeightParams.RemainingWeight);
        }

        /// <summary>
        /// 胶水剩余板数不足事件触发
        /// </summary>
        private void InsufficientRemainingGluePcsInvoke()
        {
            InsufficientRemainingGluePcs?.Invoke(this, EventArgs.Empty);
        }

        // 胶量检测线程函数
        private void ThreadGlueAmount()
        {
            while (true)
            {
                // 时间
                if (glueAmountParams.GlueAmountTimeParams.Enable)
                {
                    DateTime now = DateTime.Now;
                    glueAmountParams.GlueAmountTimeParams.RemainingTime = glueAmountParams.GlueAmountTimeParams.TotalTime - (double)((now.Hour - time.Hour) * 60 + (now.Minute - time.Minute)) / 60.0;
                    if (glueAmountParams.GlueAmountTimeParams.RemainingTime <= 0)
                        InsufficientRemainingGlueTimeInvoke();
                }

                //重量
                if (glueAmountParams.GlueAmountWeightParams.Enable)
                {
                    if (glueAmountParams.GlueAmountWeightParams.glueAmountWeightAlarm.WeightAlarmType == GlueAmountWeightAlarmType.Weight)
                    {
                        // 剩余重量不足报警
                        if (glueAmountParams.GlueAmountWeightParams.RemainingWeight < glueAmountParams.GlueAmountWeightParams.glueAmountWeightAlarm.GlueAmountWeightAlarmValue)
                        {
                            InsufficientRemainingGlueWeightInvoke();
                        }
                    }
                    else
                    {
                        // 剩余百分比不足报警
                        if (glueAmountParams.GlueAmountWeightParams.RemainingWeight / glueAmountParams.GlueAmountWeightParams.TotalWeight * 100 < glueAmountParams.GlueAmountWeightParams.glueAmountWeightAlarm.GlueAmountWeightAlarmValue)
                        {
                            InsufficientRemainingGlueWeightInvoke();
                        }
                    }
                }

                // 板数
                if (glueAmountParams.GlueAmountPcsParams.Enable)
                {
                    if (glueAmountParams.GlueAmountPcsParams.RemainingPcs == 0)
                        InsufficientRemainingGluePcsInvoke();
                }
                Thread.Sleep(300);
            }
        }

        // 板数变化响应函数
        private void PcsChanged(object? sender, EventArgs e)
        {
            if (glueAmountParams.GlueAmountPcsParams.Enable)
            {
                pcs++;
                glueAmountParams.GlueAmountPcsParams.RemainingPcs = glueAmountParams.GlueAmountPcsParams.TotalPcs - pcs;
                if (glueAmountParams.GlueAmountPcsParams.RemainingPcs < 0)
                    glueAmountParams.GlueAmountPcsParams.RemainingPcs = 0;
            }
        }

        private void WeightChanged(object? sender, double w)
        {
            if (glueAmountParams.GlueAmountWeightParams.Enable)
            {
                weight += w;
                glueAmountParams.GlueAmountWeightParams.RemainingWeight = glueAmountParams.GlueAmountWeightParams.TotalWeight - weight;
                if (glueAmountParams.GlueAmountWeightParams.RemainingWeight < 0)
                    glueAmountParams.GlueAmountWeightParams.RemainingWeight = 0;
            }
        }

        // 重量变化响应函数
        private void WeightChanged(object? sender, EventArgs e)
        {
        }
    }

    /// <summary>
    /// 胶水液位状态枚举
    /// </summary>
    public enum GlueLevelState
    {
        HighLevel,
        LowLevel,
        Empty
    }

    /// <summary>
    /// 胶水液位状态结构
    /// </summary>
    public class GlueLevel
    {
        /// <summary>
        /// 胶水余量状态
        /// </summary>
        public GlueLevelState GlueLevelStatus { get; set; }

        /// <summary>
        /// 胶水余量值
        /// </summary>
        public double GlueRemaining { get; set; } = 0;
    }

    /// <summary>
    /// GKG供胶装置
    /// </summary>
    public class GlueDispensingDeviceGKG : IGlueDispensingDeviceBase
    {
        /// <summary>
        /// 气压控制（胶压）
        /// </summary>
        private IPressureControlBase pressureControl;

        /// <summary>
        /// 胶量感应
        /// </summary>
        private IBaseStateIO glueAmountState;

        /// <summary>
        /// 胶水余量监控对象
        /// </summary>
        private GlueAmount glueAmount = new GlueAmount();

        /// <summary>
        /// 初始化参数
        /// </summary>
        private GlueDispensingDeviceGKGInitParams glueDispensingDeviceGKGInitParam;

        public double RemainingGlueTime => glueAmount.RemainingGlueTime;

        public double RemainingGlueWeight => glueAmount.RemainingGlueWeight;

        public int RemainingGluePcs => glueAmount.RemainingGluePcs;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="InitParams"></param>
        public void Init(byte[] InitParams)
        {
            //glueDispensingDeviceGKGInitParam = JsonObjConvert.FromJSonBytes<GlueDispensingDeviceGKGInitParams>(InitParams);

            //pressureControl = TestCallBack.GetPressureControl(glueDispensingDeviceGKGInitParam.PressureControlInitParams);
            //glueAmountState = IOStateCallBack.GetIOStateControl(glueDispensingDeviceGKGInitParam.GlueAmountStateInitParams, 14);
            //glueAmountState.Init(glueDispensingDeviceGKGInitParam.GlueAmountStateInitParams);

            // 胶量不足响应函数绑定
            glueAmount.InsufficientRemainingGlueTime += OnLowRemainingGlueTimeAlarm;
            glueAmount.InsufficientRemainingGlueWeight += OnLowRemainingGlueWeightAlarm;
            glueAmount.InsufficientRemainingGluePcs += OnLowRemainingGluePcsAlarm;

            //glueAmount.Init(glueDispensingDeviceGKGInitParam.GlueAmountInitParams);
            // 开线程监测胶水余量（感应信号）
            Task.Run(() => { DetectGlueAmount(); });
        }

        /// <summary>
        /// 设置胶量检测参数
        /// </summary>
        /// <param name="glueAmountParam"></param>
        public void SetGlueAmountParams(byte[] glueAmountParam)
        {
            glueAmount.SetGlueAmountParams(JsonObjConvert.FromJSonBytes<GlueAmountParams>(glueAmountParam));
        }

        /// <summary>
        /// 刷新胶量
        /// </summary>
        public void RefreshGlueAmount()
        {
            glueAmount.PcsResetToZero();
            glueAmount.TimeResetToZero();
            glueAmount.WeightResetToZero();
        }

        /// <summary>
        /// 读是否缺胶
        /// </summary>
        /// <returns></returns>
        public bool ReadIsLackOfGlue()
        {
            return !glueAmountState.Read();
        }

        /// <summary>
        /// 获取胶水气压
        /// </summary>
        /// <returns></returns>
        public double GetGlueAirPressure()
        {
            return pressureControl.GetPressure();
        }

        /// <summary>
        /// 设置胶水气压
        /// </summary>
        /// <param name="pressure"></param>
        public void SetGlueAirPressure(double pressure)
        {
            pressureControl.SetPressure(pressure);
        }

        /// <summary>
        /// 设备异常事件
        /// </summary>
        public event EventHandler<EquipmentExceptionEventArgs>? EquipmentException;

        /// <summary>
        /// 胶水液位变化事件
        /// </summary>
        public event EventHandler<string>? GlueLevelChanged;

        /// <summary>
        /// 胶水剩余时间不足
        /// </summary>
        public event EventHandler<EquipmentExceptionEventArgs>? LowRemainingGlueTimeAlarm;

        /// <summary>
        /// 胶水剩余重量不足
        /// </summary>
        public event EventHandler<EquipmentExceptionEventArgs>? LowRemainingGlueWeightAlarm;

        /// <summary>
        /// 胶水剩余板数不足
        /// </summary>
        public event EventHandler<EquipmentExceptionEventArgs>? LowRemainingGluePcsAlarm;

        /// <summary>
        /// 胶压超限报警
        /// </summary>
        public event EventHandler<EquipmentExceptionEventArgs>? GluePressureOutOfRangeAlarm;

        /// <summary>
        /// 空胶报警
        /// </summary>
        public event EventHandler<EquipmentExceptionEventArgs>? GlueEmptyAlarm;

        private void OnEquipmentException(object? sender, EquipmentExceptionEventArgs e)
        {
            EquipmentException?.Invoke(this, e);
        }

        private void OnGlueLevelChanged(object? sender, string e)
        {
            GlueLevelChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 胶量感应检测
        /// </summary>
        private void DetectGlueAmount()
        {
            bool glueAmountStatus = glueAmountState.Read();
            bool glueAmountStatusT = glueAmountStatus;
            while (true)
            {
                // 硬件检测
                glueAmountStatus = glueAmountState.Read();
                if (glueAmountStatus != glueAmountStatusT)
                {
                    if (glueAmountStatus)
                    {
                        OnGlueLevelChanged(this, "High");
                    }
                    else
                    {
                        OnGlueLevelChanged(this, "Low");
                    }
                }
                Thread.Sleep(300);
                // 软件计数
            }
        }

        private void OnLowRemainingGlueTimeAlarm(object? sender, EventArgs e)
        {
            LowRemainingGlueTimeAlarm?.Invoke(this, new EquipmentExceptionEventArgs(10001, "", 2));
        }

        private void OnLowRemainingGlueWeightAlarm(object? sender, double e)
        {
            LowRemainingGlueWeightAlarm?.Invoke(this, new EquipmentExceptionEventArgs(10002, "", 2));
        }

        private void OnLowRemainingGluePcsAlarm(object? sender, EventArgs e)
        {
            LowRemainingGluePcsAlarm?.Invoke(this, new EquipmentExceptionEventArgs(10003, "", 2));
        }

        public void Dispose()
        {
            if (glueAmount != null)
            {
                glueAmount.InsufficientRemainingGlueTime -= OnLowRemainingGlueTimeAlarm;
                glueAmount.InsufficientRemainingGlueWeight -= OnLowRemainingGlueWeightAlarm;
                glueAmount.InsufficientRemainingGluePcs -= OnLowRemainingGluePcsAlarm;
            }
        }
    }
}