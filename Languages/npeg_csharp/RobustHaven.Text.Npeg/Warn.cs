using System;

namespace RobustHaven.Text.Npeg
{
	public class Warn
	{
		public Warn()
		{
			Message = String.Empty;
		}

		public Warn(String message, Int32 position)
		{
			Message = message;
			Position = position;
		}

		public String Message { get; set; }

		public Int32 Position { get; set; }
	}
}