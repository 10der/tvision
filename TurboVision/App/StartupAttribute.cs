using System;

namespace TurboVision.App.Runtime
{
	[AttributeUsage( AttributeTargets.All)]
	public class StartupAttribute : Attribute
	{
        private string programName;
		private bool register = true;

		public StartupAttribute()
		{
		}

		public string ProgramName
		{
			get
			{
				return programName;
			}
			set
			{
				programName = value;
			}
		}

		public bool Register
		{
			get
			{
				return register;
			}
			set
			{
				register = value;
			}
		}
	}
}
