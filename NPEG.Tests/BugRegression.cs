using System.Linq;
using System.Text;
using NPEG.Extensions;
using NPEG.GrammarInterpreter;
using NUnit.Framework;

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


		[Test]
		public void Literal_With_Extended_Ascii_Should_Be_Byte_Level_Equivalent_In_GrammerText_And_Input()
		{
			var rules = PEGrammar.Load(@"(?<Degree>): '°';");
			var bytes = Encoding.UTF8.GetBytes(@"°");
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			rules.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			var ast = visitor.AST;
			Assert.IsTrue(ast.Token.ValueAsString(iterator) == @"°");
		}

	}
}
