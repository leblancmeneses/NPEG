using System;
using System.Text;
using RobustHaven.Text.Npeg;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class NpegNode : NpegParser
	{
		public NpegNode(InputIterator iterator): base(iterator){}

		public override Boolean IsMatch()
		{
			return NpegNode_impl_0();
		}


		protected Boolean NpegNode_impl_0()
		{
			String _nodeName_0 = "NpegNode";
			return this.CapturingGroup(new IsMatchPredicate(this.NpegNode_impl_1), _nodeName_0, false, false);
		}

		protected Boolean NpegNode_impl_1()
		{
			return this.PrioritizedChoice(new IsMatchPredicate(this.NpegNode_impl_2), new IsMatchPredicate(this.NpegNode_impl_3));
		}

		protected Boolean NpegNode_impl_3()
		{
			String _literal_0 = ".NET Parsing Expression Grammar";
			return this.Literal(_literal_0, false);
		}

		protected Boolean NpegNode_impl_2()
		{
			String _literal_0 = "NPEG";
			return this.Literal(_literal_0, false);
		}
	}
}
