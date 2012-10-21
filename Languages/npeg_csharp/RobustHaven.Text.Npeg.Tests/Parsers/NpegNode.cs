using System;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class NpegNode : NpegParser
	{
		public NpegNode(InputIterator iterator) : base(iterator)
		{
		}

		public override Boolean IsMatch()
		{
			return NpegNode_impl_0();
		}


		protected Boolean NpegNode_impl_0()
		{
			String _nodeName_0 = "NpegNode";
			return CapturingGroup(NpegNode_impl_1, _nodeName_0, false, false);
		}

		protected Boolean NpegNode_impl_1()
		{
			return PrioritizedChoice(NpegNode_impl_2, NpegNode_impl_3);
		}

		protected Boolean NpegNode_impl_3()
		{
			String _literal_0 = ".NET Parsing Expression Grammar";
			return Literal(_literal_0, false);
		}

		protected Boolean NpegNode_impl_2()
		{
			String _literal_0 = "NPEG";
			return Literal(_literal_0, false);
		}
	}
}