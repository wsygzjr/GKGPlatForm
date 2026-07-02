using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKG.UI.General;

namespace Griffins.CompUI.SL.InitCfgPage.Models
{
    public class MotorMechanismCompUIModel
    {
        public int PositionNumber { get; set; } = 1;

        public decimal CoordinateValue { get; set; } = 0m;

        public MotorMoveType MoveType { get; set; } = MotorMoveType.MoveToSpecifiedPosition;
    }
}
