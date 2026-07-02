using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Services;
using System;

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Factories
{
    public static class DeviceStatusEditorFactoryRegistration
    {
        private static bool _isRegistered = false;
        private static bool _isGlobalRegistered = false;
        private static readonly object _lock = new object();

        public static void Register(PropertyGrid propertyGrid)
        {
            if (propertyGrid == null) return;
            lock (_lock)
            {
                RegisterGlobal();
                if (!_isRegistered)
                {
                    propertyGrid.Factories.AddFactory(new DeviceStatusImageSourcesEditorFactory());
                    _isRegistered = true;
                }
            }
        }

        public static void RegisterGlobal()
        {
            lock (_lock)
            {
                if (_isGlobalRegistered) return;
                try
                {
                    CellEditFactoryService.Default.AddFactory(new DeviceStatusImageSourcesEditorFactory());
                }
                catch (Exception) { }
                _isGlobalRegistered = true;
            }
        }

        public static void MarkAsRegistered() => _isRegistered = true;
        public static bool IsRegistered => _isRegistered;
    }
}
