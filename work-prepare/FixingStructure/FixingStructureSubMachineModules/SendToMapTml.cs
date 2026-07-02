using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixingStructureSubMachineModules
{

    public class SendToMapTml: InformInfo_StatusChanged
    {
        public SendToMapTml(string messageKind,string data) 
        {
            MessageKind = messageKind;
            Data = data;
        }
        public string MessageKind { get; set; } = "PositionChanged";
        public string Data { get; set; } = "";
    }
}
