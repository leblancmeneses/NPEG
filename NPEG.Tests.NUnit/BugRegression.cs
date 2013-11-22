using System.Linq;
using System.Text;
using NUnit.Framework;
using NPEG.GrammarInterpreter;

namespace NPEG.Tests
{
	[TestFixture]
	public class BugRegression
	{

		[Test]
		public void Recursive_Grammar_Used_In_Predicate_Should_Not_Append_To_Ast()
		{
			var rules = PEGrammar.Load(@"
NewLine: [\r][\n] / [\n][\r] / [\n] / [\r];
Space: ' ';
Tab: [\t];
s: ( Space / Tab  )+;
S: ( NewLine )+;
W: (s / S);
(?<Notes>): 'Notes'\i s* ':' (!Scenario .)+;
(?<Scenario>): 'Scenario'\i s* ':'  (?<Title> (!S .)+ ) W+ Notes?;
(?<Document>): W* (Scenario W*)+ !. ;
			");

			var bytes = Encoding.UTF8.GetBytes(
@"
Scenario:	User creates a new template with a name  
	 
Notes: (markdown)

 
Scenario:	User creates a new template with case information 

Notes: (markdown)
");
			var visitor = new NpegParserVisitor(new ByteInputIterator(bytes));
			rules.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			var ast = visitor.AST;
			var scenarios = ast.Children.Where(x => x.Token.Name.Equals("Scenario")).ToList();
			Assert.IsTrue(scenarios.Count == 2);
			var notes = scenarios[0].Children["Notes"];
			Assert.IsTrue(!notes.Children.Any(), "notes should not have any children");
			// previously Notes would contain nested Scenario due to predicate !Scenario
		}

	}
}
