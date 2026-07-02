using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins;
using System;

namespace GriffinsGeneralTestMM
{
    public partial class UctlTestMMCfgView : UctlParamListView
    { 
        public UctlTestMMCfgView()
        {
            InitializeComponent(); 
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // 配置信息（字节数组） 
        public byte[] CfgInfo
        {
            get
            {
                return getCfgInfo();
            }
            set
            {
                setCfgInfo(value);

            }
        }

        private byte[] getCfgInfo()
        {
            var paramValues = base.ParamValues;
            return paramValues.ToBytes();
        }

        private void setCfgInfo(byte[] value)
        {
            if ((value == null) || (value.Length == 0))
                return;
            var paramValues = new GFBaseTypeParamValueList();
            paramValues.FromBytes(value);
            base.ParamValues = paramValues;
        }
    }
}