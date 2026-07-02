using GKG;
using GKG.ElectronicControl;
using System;
using System.Collections.Generic;

namespace GKG
{
    namespace SubMM
    {
        public class MotionControlCardInformations
        {
            public MotionCardType MotionCardType { get; set; }

            public Guid MotionCardID { get; set; } = Guid.Empty;

            public MotionControlFactoryParameters? MotionControlFactoryParameters { get; set; }

        }

        public class MotionControlCardInformationList : List<MotionControlCardInformations>
        {

        }

        public class EletronicFactoryParameters
        {
            public MotionControlCardInformationList? MotionControlCardInformations { get; set; }

            public IOStateInitParameters? PowerOnIO { get; set; }
     
        }

        public class EletronicAxisCmdParameters
        {
            public AxisBinding AxisBinding { get; set; }
            public double Speed { get; set; }   
            public double Acc { get; set; }
            public double Position { get; set; }
        }

        public class MotionAxisInfo
        {
            public int PhysicalAxis { get; set; }
            public Guid LockAxisGuid { get; set; }
            public bool IsRequestStop { get; set; }
        }

        public class MotionCardInfomation
        {
            public MotionCardType CardType { get; set; }
            public IMotionControlBase? MotionControl { get; set; }
        }
    }
}