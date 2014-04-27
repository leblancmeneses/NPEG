using System;
using System.Collections.Generic;
using System.Linq;

namespace NPEG.GrammarInterpreter
{
	public class NpegLog
	{
		public NpegLog()
		{
			Children = new List<NpegLog>();
		}

		public List<NpegLog> Children { get; set; }
		public NpegLog Parent { get; set; }


		public string CommandName {
			get { return Command.GetType().Name; }
		}

		public AExpression Command { get; set; }

		public bool IsMatch { get; set; }

		public Int32 InputStart { get; set; }

		public Int32 InputEnd { set; get; }

		public Int32 RuleStart { get; set; }

		public Int32 RuleEnd { set; get; }

		public bool IsTerminal {
			get
			{
				var terminalsAre = new[] { "Literal", "CodePoint", "AnyCharacter", "CharacterClass", "RecursionCall", "DynamicBackReference" };
				return terminalsAre.Any(x=>x.Equals(CommandName, StringComparison.InvariantCultureIgnoreCase));
			}
		}
	}
}
