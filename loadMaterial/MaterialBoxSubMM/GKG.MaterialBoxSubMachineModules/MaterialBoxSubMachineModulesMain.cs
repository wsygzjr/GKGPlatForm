using GF_Gereric;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{84561883-E202-469A-890F-3E447B3E1B95}", "GKG.MaterialBoxSubMachineModules")]

namespace GKG
{
    namespace SubMM
    {
        /// <summary>料盒子模组插件入口：负责暴露子模组对象。</summary>
        [SubMachineModulesMngAttribute(MaterialBoxSubMachineModulesConst.SubMMModelStr, "LoadUnload")]
        //[PluginSupportCompany("GKG")]
        class MaterialBoxSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
        {
            /// <summary>插件初始化入口；当前无额外启动逻辑。</summary>
            void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
            {

            }

            /// <summary>固定子模组对象 ID；当前插件只暴露一个料盒对象。</summary>
            Guid guid = Guid.Parse("5A02600E-D806-438F-9B56-459E62730461");
            string ISubMachineModulesMng.SubMMName { get => MaterialBoxSubMachineModulesConst.SubMMName; }

            SubMMObjInfoList ISubMachineModulesMng.SubMMObjInfos => new SubMMObjInfoList
            {
                new SubMMObjInfo
                {
                    SubMMObjID = guid,
                    SubMMObjName = "物料箱1",

                }
            };

            public SubMMObjInfoList SubMMObjInfos () { return new SubMMObjInfoList(); }

            /// <summary>
            /// 返回框架要求的能力定义对象。
            /// 普通方法虽已在执行器中实现，但由于宿主接口没有单独的普通方法声明面，这里只返回能力方法定义。
            /// </summary>
            public ISubMachineModulesCabilityDef CretaeCabilityDef()
            {
                return new SubMachineModulesCabilityDef();
            }

            /// <summary>创建料盒执行器。</summary>
            public ISubMMCmdExecutor CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfg)
            {
                return new MaterialBoxSubMMCmdExecutor(alias, subMMObjID, factoryCfg);
            }
        }
    }
}
