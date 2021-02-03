using System;
using TurboVision.Objects;

namespace TurboVision.Dialogs
{
	/// <summary>
	/// Summary description for ParamText.
	/// </summary>
	public class ParamText : StaticText
	{

		public int ParamCount;

		public ParamText( Rect Bounds, string AText, int AParamCount):base( Bounds, AText)
		{
			ParamCount = AParamCount;
		}
	}
}
