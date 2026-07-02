using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    namespace MM
    {
      
        public enum EStopPanelMode
        {
            Cylinder,
            Sensor,
        }

        public enum ETransferMode
        {
            Normal,
            TwoSpeed,
        }

        public class InletPanelParameters
        {
            public ETransferMode TransferMode { get; set; }
            public double InletSpeed { get; set; }
            public double SecondSpeed { get; set; }
            public double InletDelay { get; set; }
            public double InletTimeout { get; set; }
        }

        public class OutletPanelParameters
        {
            public ETransferMode TransferMode { get; set; }
            public double OutletSpeed { get; set; } 
            public double SecondOutletSpeed { get; set; }
            public double OutletDelay { get; set; } 
            public double OutletTimeout { get; set; }
        }

        public class StopPanelParameters
        {
            public EStopPanelMode StopPanelMode { get; set; }
            public double BufferingSpeed { get; set; }
            public double BufferingDelay { get; set; }
            public double ReverseSpeed { get; set; }
        }
    }
}
