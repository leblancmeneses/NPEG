using NPEG;

namespace LanguageWorkbench.Algorithms.LanguageWriters
{
	public abstract class WriterBase : IParseTreeVisitor
	{
		public abstract void Write();
	}
}