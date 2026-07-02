using GF_Gereric;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.Views;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ViewModels;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage
{
    public class ControlCardInnerSubPageRunTime : IInnerSubPageRunTime
    {
        private readonly ElectricalMngPageViewModel _pageViewModel;
        private EventHandler? _afterDataModified;

        public ControlCardInnerSubPageRunTime()
            : this(new ElectricalMngPageViewModel())
        {
        }

        public ControlCardInnerSubPageRunTime(ElectricalMngPageViewModel pageViewModel)
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
                var view = new ControlCardCfgView();
                view.DataContext = _pageViewModel.ControlCardCfgViewModel;
                _pageViewModel.SetViewReference(view);
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

            var designCfgInfo = JsonObjConvert.FromJSonBytes<ControlCardInnerSubPageDesignCfg>(viewCfgInfo);
            if (designCfgInfo == null)
                throw new Exception("运控卡页面设计时配置数据解析失败");
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

    public class ControlCardInnerSubPageDesignCfg
    {
    }
}
