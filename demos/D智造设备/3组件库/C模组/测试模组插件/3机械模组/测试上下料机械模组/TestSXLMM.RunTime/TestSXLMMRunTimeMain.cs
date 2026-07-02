using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;

[assembly: Plugin(MachineModulesMngAttribute.PLUGINKIND_Str, "{CC31B34C-FB3C-433A-BC14-67B1D0CDB389}", "TestSXLMM.RunTime")]

namespace TestSXLMM.RunTime
{
    /// <summary>
    /// 标准上下料机械模组
    /// </summary>
    [MachineModulesMng(GenMMInfoDefine.MMNumberStr, "UpDown")]
    public class TestSXLMMRunTimeMain : GenTestMMRunTimeBase
    {
        private GenTestMMCmdExecutor mMCmdExecutor;
        //容器ID和名称字典
        private Dictionary<string, string> containersNameDic;
        //料盒ID和名称字典
        private Dictionary<string, string> materialBoxNameDic;
        //料槽ID和名称字典
        private Dictionary<string, string> slotObjNameDic;

        public TestSXLMMRunTimeMain() : base(GenMMInfoDefine.GenMMInfo)
        {
            GenMMInfoDefine.GenMMInfo.IUIDataObjPropExChange = new UIDataObjPropExChange(this);
        }
        protected override IMMCmdExecutor _CreateMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
        {
            return Dispatcher.UIThread.Invoke(() =>
            {
                TestMMMain.Init();
                mMCmdExecutor =(GenTestMMCmdExecutor) TestMMMain.CreateMMCmdExecutor(GenMMInfoDefine.GenMMInfo, alias);
                mMCmdExecutor.OnAfterInit += onExecutorAfterInit;
                return mMCmdExecutor;
            });
        }
        /// <summary>
        /// 在MMCmdExecutor执行器初始化后
        /// </summary>
        private void onExecutorAfterInit()
        {
            //测试：
            //创建界面数据对象所需的对象名称信息字典
            containersNameDic = createContainersNameDic();
            materialBoxNameDic = createMaterialBoxNameDic(true,null);
            slotObjNameDic = createSlotObjNameDic();
            //创建界面数据对象
            LoadUnloadUIObj loadUnloadUIObj = createLoadUnloadUIObj();
            TestData.SimpleTest(loadUnloadUIObj);
            TestData.OtherTest(loadUnloadUIObj);
        }


        /// <summary>
        /// 获取子界面数据对象项名称列表
        /// </summary>
        /// <param name="proIDPath">属性路径对象</param>
        /// <returns></returns>
        internal Dictionary<string, string> GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
        {
            return getSubUIProObjItemNames(objInstPropPath, true, null);
        }

        /// <summary>
        /// 在定义时获取子界面数据对象项名称信息字典
        /// </summary>
        /// <param name="objInstPropPath">界面数据对象属性路径</param>
        /// <param name="callBack">定义服务的回调</param>
        /// <returns></returns>
        internal Dictionary<string, string> GetMMSubUIProObjItemNamesOfDefSvr(ObjInstPropPath objInstPropPath, IMachineModulesDefSvrCallBack callBack)
        {
            return getSubUIProObjItemNames(objInstPropPath, false, callBack);
        }
        /// <summary>
        /// 获取子界面数据对象项名称列表
        /// </summary>
        /// <param name="proIDPath">属性路径对象</param>
        /// <param name="isRunTime">是否为运行时调用 true：是 false：为定义时调用</param>
        /// <returns></returns>
        private Dictionary<string, string> getSubUIProObjItemNames(ObjInstPropPath objInstPropPath,bool isRunTime, IMachineModulesDefSvrCallBack callBack)
        {
            var proIDPath = objInstPropPath.PropIDPath;
            if (proIDPath == null || proIDPath.Length == 0)
                return null;
            //是获取料盒容器ID和名称字典
            string objInstPropPathStr = objInstPropPath.ToString();
            //objInstPropPathStr: "MaterialContainers"
            if (proIDPath.Length == 1 && nameof(LoadUnloadUIObj.MaterialContainers) == proIDPath[0])
            {
                if (containersNameDic == null)
                    containersNameDic = createContainersNameDic();
                return containersNameDic;
            }
            //是获取料盒ID和名称字典
            //objInstPropPathStr:MaterialContainers[Containers0].MaterialBoxs
            if (proIDPath.Length == 2 && nameof(MaterialContainer.MaterialBoxs) == proIDPath[1])
            {
                if (materialBoxNameDic == null)
                    materialBoxNameDic = createMaterialBoxNameDic(isRunTime,callBack);
                return materialBoxNameDic;

            }
            //是获取料盒ID和名称字典
            //objInstPropPathStr:MaterialContainers[Containers0].MaterialBoxs[materialBox0].SlotObjs
            if (proIDPath.Length == 3 && nameof(MaterialBox.SlotObjs) == proIDPath[2])
            {
                if (slotObjNameDic == null)
                    slotObjNameDic = createSlotObjNameDic();
                return slotObjNameDic;
            }
            return null;
        }
        /// <summary>
        /// 生成料槽ID和名称字典
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Dictionary<string, string> createSlotObjNameDic()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            string slotObjName = "料槽";
            for (int index = 0; index < 2; index++)
            {
                keyValuePairs.Add($"slotObj{index}", $"{slotObjName}{index}");
            }
            return keyValuePairs;
        }

        /// <summary>
        /// 生成料盒ID和名称字典
        /// </summary>
        /// <returns></returns>
        /// <param name="isRunTime">是否为运行时调用 true：是 false：为定义时调用</param>
        /// <param name="callBack">isRunTime为false有效</param>
        private Dictionary<string, string> createMaterialBoxNameDic(bool isRunTime, IMachineModulesDefSvrCallBack callBack)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            string materialBoxName = getMaterialBoxSubMMName(isRunTime, callBack);
            for (int index = 0; index < 2; index++)
            {
                keyValuePairs.Add($"materialBox{index}", $"{materialBoxName}{index}");
            }
            return keyValuePairs;
        }

        /// <summary>
        /// 生成容器ID和名称字典
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Dictionary<string, string> createContainersNameDic()
        {
            Dictionary<string, string> keyValuePairs=new Dictionary<string, string>();
            string containersName ="容器";
            for (int index = 0; index < 3; index++)
            {
            keyValuePairs.Add($"Containers{index}", $"{containersName}{index}");
            }
            return keyValuePairs;
        }
        /// <summary>
        /// 获取料盒子机械模组实例名称
        /// </summary>
        /// <param name="isRunTime">是否为运行时调用 true：是 false：为定义时调用</param>
        /// <param name="callBack">isRunTime为false有效</param>
        /// <returns>子机械模组实例名称</returns>
        private string getMaterialBoxSubMMName(bool isRunTime, IMachineModulesDefSvrCallBack callBack)
        {
            //return "料盒";
            //料盒是模组下的子机械模组，则根据料盒内部别名获取实例名称
            if (isRunTime)
                //运行时已创建执行器时
                return mMCmdExecutor.IMMCmdExecutorCallBack.GetSubMMInstanceName(GenMMInfoDefine.InnerAliasMaterialBox);
            else
                //不在运行时，在设计时，则通过回调获取
                return callBack.GetSubMMInstanceName(GenMMInfoDefine.InnerAliasMaterialBox);
        }


        /// <summary>
        /// 创建界面数据对象
        /// </summary>
        private LoadUnloadUIObj createLoadUnloadUIObj()
        {
            //获取上下料界面描述对象属性值列表
            LoadUnloadUIObj loadUnloadUIObj = new LoadUnloadUIObj();
            loadUnloadUIObj.MaterialContainers = new Dictionary<string, MaterialContainer>();
            foreach (var containerKv in containersNameDic)
            {
                string containerKey = containerKv.Key;
                string containerName = containerKv.Value;
                var container = new MaterialContainer()
                {
                    ContainerName = containerName,
                    MaterialBoxs = new Dictionary<string, MaterialBox>()
                };

                foreach (var boxKv in materialBoxNameDic)
                {
                    string boxKey = boxKv.Key;
                    string boxName = boxKv.Value;

                    var materialBox = new MaterialBox()
                    {
                        MaterialBoxName = boxName,
                        IsEmpty = true,
                        MaterialBoxCylinderStatus = true,
                        SlotObjs = new Dictionary<string, SlotObj>()
                    };

                    foreach (var slotKv in slotObjNameDic)
                    {
                        string slotKey = slotKv.Key;
                        string slotName = slotKv.Value;

                        var slotObj = new SlotObj()
                        {
                            MaterialStatus = MaterialStatus.Full
                        };
                        materialBox.SlotObjs.Add(slotKey, slotObj);
                    }
                    // 把料盒加入容器
                    container.MaterialBoxs.Add(boxKey, materialBox);
                }
                // 把容器加入根对象
                loadUnloadUIObj.MaterialContainers.Add(containerKey, container);
            }

            loadUnloadUIObj.TestMaterialBox = new  MaterialBox()
            {
                MaterialBoxName = "料盒测试"
            };

            return loadUnloadUIObj;
        }
       
    }
}
