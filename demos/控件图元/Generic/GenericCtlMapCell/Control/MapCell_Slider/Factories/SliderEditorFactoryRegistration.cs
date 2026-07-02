using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Services;
using System;
using System.Runtime.CompilerServices;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.Factories
{
    public static class SliderEditorFactoryRegistration
    {
        private static bool _isRegistered = false;
        private static bool _isGlobalRegistered = false;
        private static readonly object _lock = new object();

        [ModuleInitializer]
        internal static void InitModule()
        {
            RegisterGlobal();
        }

        public static void Register(PropertyGrid propertyGrid)
        {
            if (propertyGrid == null) return;

            lock (_lock)
            {
                RegisterGlobal();
                if (!_isRegistered)
                {
                    propertyGrid.Factories.AddFactory(new SliderValueStepEditorFactory());
                    _isRegistered = true;
                }
            }
        }

        public static void RegisterGlobal()
        {
            lock (_lock)
            {
                if (_isGlobalRegistered)
                {
                    return;
                }

                try
                {
                    CellEditFactoryService.Default.AddFactory(new SliderValueStepEditorFactory());
                }
                catch (Exception)
                {
                }

                _isGlobalRegistered = true;
            }
        }

        public static void MarkAsRegistered()
        {
            _isRegistered = true;
        }

        public static bool IsRegistered => _isRegistered;
    }
}
