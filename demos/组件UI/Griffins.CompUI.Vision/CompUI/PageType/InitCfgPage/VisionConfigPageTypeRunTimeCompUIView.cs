using Avalonia.Controls;
using Avalonia.Layout;
using GF_Gereric;
using GKG.SubMM;
using GKG.Vision;
using Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage.VisionInitCfg.ViewModels;
using Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage.VisionInitCfg.Views;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage
{
    internal class VisionConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly ICompUIRunTimeCallBack _callBack;
        private readonly StackPanel _view;
        IVisionDriverParameterEditor parameterEditor;
        private event EventHandler _afterModified;
        private VisionInitCfgViewModel visionInitCfgViewModel;
        public VisionConfigPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
            visionInitCfgViewModel = new VisionInitCfgViewModel(callBack);
            var rtn = callBack.ExecNormalCtlCmd("GetPluginName", new GFBaseTypeParamValueList());
            string PluginName = rtn[nameof(PluginName)].ToStringVal();
            if (parameterEditor == null)
            {
                parameterEditor = VisionPluginManager.GetVisionDriverParameterEditor(PluginName);
            }
            var view1 = (UserControl)parameterEditor.View;
            var view2 = new VisionInitCfgView();
            view2.DataContext = visionInitCfgViewModel;
            StackPanel view3 = new StackPanel();
            view3.Orientation = Orientation.Vertical;
            //view3.SetCurrentValue(Grid.RowProperty, new RowDefinitions()
            //{
            //    new RowDefinition(){ Height = new GridLength(1, GridUnitType.Star) },
            //    new RowDefinition(){ Height = new GridLength(5, GridUnitType.Star) }
            //});
            //view3.Children.Add(view1);
            view3.Children.Add(view2);
            _view = view3;
            parameterEditor.AfterModify += (s, e) => _afterModified?.Invoke(this, e);
        }

        public object View
        {
            get
            {
                RemoveViewFromParent();
                return _view;
            }
        }

        private void RemoveViewFromParent()
        {
            if (_view == null) return;

            if (_view.Parent is Panel panelParent)
            {
                if (panelParent.Children.Contains(_view))
                {
                    panelParent.Children.Remove(_view);
                }
            }
            else if (_view.Parent is ContentControl contentParent)
            {
                if (contentParent.Content == _view)
                {
                    contentParent.Content = null;
                }
            }
        }

        public OpMngCellID[] EditFuncMngCellIDs => null;

        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        public void SetReadOnly(bool readOnly)
        {
        }

        public void SetData(byte[] data)
        {
            //parameterEditor.SetData(data);
            visionInitCfgViewModel.SetData(JsonObjConvert.FromJSonBytes<VisionSubMachineModulesInitCfg>(data));
        }

        public byte[] GetData()
        {
            //return parameterEditor.GetData();
            return JsonObjConvert.ToJSonBytes(visionInitCfgViewModel.GetData());
        }
    }
}
