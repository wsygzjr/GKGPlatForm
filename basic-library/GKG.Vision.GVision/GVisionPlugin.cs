using GF_Gereric;
using ShareMemRPCLite;
using System;

[assembly: Plugin(GKG.Vision.VisionDefAttribute.PLUGINKIND_Str, "{7E632B12-4CF6-4CEB-A4D8-8E8E18D9D4A2}", "GVision Plugin")]

namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// GVision 插件入口。
        /// </summary>
        [VisionDef("{0D89C6E4-90D5-4B49-8D27-3ED9E6655A61}")]
        internal class GVisionPlugin : GriffinsPluginMngClass, IVisionPlugin
        {
            private readonly Lazy<CallGVision> sharedCall = new Lazy<CallGVision>(() => new CallGVision());

            string IVisionPlugin.VisionName => "GVision";

            string IVisionPlugin.DriverName => GVisionDriver.DefaultDriverName;

            void IVisionPlugin.Init(string pluginFileName)
            {
            }

            IVisionDriver IVisionPlugin.CreateVisionDriverObj()
            {
                return new GVisionDriver(sharedCall.Value);
            }

            IVisionDriverParameterEditor IVisionPlugin.CreateVisionDriverParameterEditorObj()
            {
                return new GVisionDriverParameterEditor();
            }

            IFrontendVisionService? IVisionPlugin.CreateFrontendVisionServiceObj()
            {
                return new FrontendVisionService();
            }
        }
    }
}
