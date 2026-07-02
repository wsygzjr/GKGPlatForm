namespace GKG
{
    namespace SubMM
    {
        public class AxisDebugSubMachineModulesConst
        {
            public const string EnableMethodID = "Enable";
            public const string ClearStatusMethodID = "ClearStatus";
            public const string HomeMethodID = "Home";
            public const string StopMethodID = "Stop";
            public const string JogMethodID = "Jog";
            public const string MoveRelativeMethodID = "MoveRelative";

            public const string EnableParamKey = "EnableParam";
            public const string ClearStatusParamKey = "ClearStatusParam";
            public const string HomeParamKey = "HomeParam";
            public const string StopParamKey = "StopParam";
            public const string JogParamKey = "JogParam";
            public const string MoveRelativeParamKey = "MoveRelativeParam";
        }

        public class AxisDebugEnableParameters
        {
            public int LogicalAxis { get; set; }
            public bool Enable { get; set; }
        }

        public class AxisDebugClearStatusParameters
        {
            public int LogicalAxis { get; set; }
        }

        public class AxisDebugHomeParameters
        {
            public int LogicalAxis { get; set; }
        }

        public class AxisDebugStopParameters
        {
            public int LogicalAxis { get; set; }
        }

        public class AxisDebugJogParameters
        {
            public int LogicalAxis { get; set; }
            public double Speed { get; set; }
            public bool Direction { get; set; }
            public bool Start { get; set; }
        }

        public class AxisDebugRelativeMoveParameters
        {
            public int LogicalAxis { get; set; }
            public double RelativeDistance { get; set; }
            public double Speed { get; set; }
            public bool Direction { get; set; }
        }
    }
}
