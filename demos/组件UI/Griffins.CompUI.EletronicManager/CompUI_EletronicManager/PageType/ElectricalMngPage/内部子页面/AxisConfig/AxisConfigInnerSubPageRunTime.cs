using GF_Gereric;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.Views;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ViewModels;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage
{
    public class AxisConfigInnerSubPageRunTime : IInnerSubPageRunTime
    {
        private readonly ElectricalMngPageViewModel _pageViewModel;
        private EventHandler? _afterDataModified;

        public AxisConfigInnerSubPageRunTime()
            : this(new ElectricalMngPageViewModel())
        {
        }

        public AxisConfigInnerSubPageRunTime(ElectricalMngPageViewModel pageViewModel)
        {
            _pageViewModel = pageViewModel;
            _pageViewModel.AfterModified += OnAfterModified;
        }

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            _pageViewModel.SetCallBack(callBack);
        }

        void ISubPageRunTime.Init(byte[] viewCfgInfo)
        {
            SetCfgInfo(viewCfgInfo);
        }

        object ISubPageRunTime.View
        {
            get
            {
                var view = new AxisConfigView
                {
                    DataContext = _pageViewModel.AxisConfigViewModel,
                };
                return view;
            }
        }

        void ISubPageRunTime.SetViewCfgInfo(byte[] viewCfgInfo)
        {
            SetCfgInfo(viewCfgInfo);
        }

        private void SetCfgInfo(byte[] viewCfgInfo)
        {
            if (viewCfgInfo == null)
                return;

            var designCfgInfo = JsonObjConvert.FromJSonBytes<AxisConfigInnerSubPageDesignCfg>(viewCfgInfo);
            if (designCfgInfo == null)
                throw new Exception("Axis config design-time config parse failed.");
        }

        event EventHandler IInnerSubPageRunTime.AfterModified
        {
            add => _afterDataModified += value;
            remove => _afterDataModified -= value;
        }

        void IInnerSubPageRunTime.OnInit()
        {
        }

        void IInnerSubPageRunTime.SetData(byte[] data)
        {
        }

        byte[] IInnerSubPageRunTime.GetData()
        {
            return _pageViewModel.GetData();
        }

        private void OnAfterModified(object? sender, EventArgs e)
        {
            _afterDataModified?.Invoke(sender, e);
        }

        public bool CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }
    }

    public class AxisConfigInnerSubPageDesignCfg
    {
    }
}
