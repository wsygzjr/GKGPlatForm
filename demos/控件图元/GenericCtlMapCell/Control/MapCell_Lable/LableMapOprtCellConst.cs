using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.CtlMapCell.Generic.Lable
{
	internal class LableMapOprtCellConst
	{
		public const string LableValue_MapOprtCellIDStr = "{FAD67070-32C8-47BD-B401-C50DF55A6FCA}";

		public static readonly MapOprtCellID LableValue_MapOprtCellID = MapOprtCellID.Parse(LableValue_MapOprtCellIDStr);

		public const string LableColor_MapOprtCellIDStr = "{FE57DCF4-A44D-4862-8F61-AA31E2AEEFE1}";

		public static readonly MapOprtCellID LableColor_MapOprtCellID = MapOprtCellID.Parse(LableColor_MapOprtCellIDStr);

		public const string BackColor_MapOprtCellIDStr = "{A6626F30-6743-4FEA-BCC1-577B38164784}";

		public static readonly MapOprtCellID BackColor_MapOprtCellID = MapOprtCellID.Parse(BackColor_MapOprtCellIDStr);
	}
}
