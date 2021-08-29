using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NPEG.GrammarInterpreter;
using NPEG.Tests.Extensions;
using NUnit.Framework;

namespace NPEG.Tests
{
  [TestFixture]
  public class NpegOptionTests
  {
    [Test]
    public void Compiled_Parse_Tree_Is_Faster()
    {
      string grammar = @"
NewLine: [\r][\n] / [\n][\r] / [\n] / [\r];
Space: ' ';
Tab: [\t];
(?<EndOfLineComment>): (('#' / '//') (?<msg>(!NewLine .)*) NewLine) ;
(?<MultilineComment>): ('/*' (?<msg>(!'*/' .)*) '*/');
s: ( Space / Tab / MultilineComment )+;
S: ( NewLine / EndOfLineComment )+;
S1: ( NewLine / EndOfLineComment );
W: (s / S);
(?<Table>):(?<Row>
s* ('|' (?<DataColumn> (! ('|' / S) .)+ ))+ '|' S1
)+;
(?<Gherkin>): (((?<Line> s* (?<Key> 'Given'\i / 'When'\i / 'Then'\i / 'And'\i / 'But'\i ) (?<Statement> (!S .)+ ) ) W*) / Table)+;
(?<TagLine>): (?<Tag>'@' ((?<Name> (!(s* '@' / s* S) .)+ )) s*)+ W;
(?<FeatureLine>): 'Feature'\i s* ':' s* (?<Title> (!S .)+ ) W+
(?<InOrder> s* &'In order to'\i (?<Text> (!S .)+ ) S1)?
(?<AsAn> s* &'As an'\i (?<Text> (!S .)+ ) S1)?
(?<IWantTo> s* &'I want to'\i (?<Text> (!S .)+ ) S1)?;
(?<Background>): 'Background'\i s* ':' W* Gherkin ;
(?<Scenario>): TagLine* 'Scenario'\i s* ':' (?<Title> (!S .)+ ) W* Gherkin?;
(?<ScenarioOutline>): TagLine* 'Scenario'\i s 'Outline'\i s* ':' (?<Title> (!S .)+ ) W* Gherkin? (?<Example> s* 'Examples:'\i S1 Table);
(?<Document>): W* TagLine* FeatureLine Background? W* ((Scenario/ScenarioOutline) W*)+ ;
".Trim();

      var withNone = Timing.TimedFor(() =>
      {
        PEGrammar.Load(grammar, NpegOptions.Optimize);
      }, 10);

      var withCompiled = Timing.TimedFor(() =>
      {
        PEGrammar.Load(grammar, NpegOptions.Cached | NpegOptions.Optimize);
      }, 10);

      var withNoneAfter = Timing.TimedFor(() =>
      {
        PEGrammar.Load(grammar, NpegOptions.Optimize);
      }, 10);

      Console.WriteLine("withNone: {0}", withNone.ElapsedMilliseconds);
      Console.WriteLine("withCompiled: {0}", withCompiled.ElapsedMilliseconds);
      Console.WriteLine("withNoneAfter: {0}", withNoneAfter.ElapsedMilliseconds);
      Console.WriteLine("total time: {0}", withNone.ElapsedMilliseconds + withCompiled.ElapsedMilliseconds + withNoneAfter.ElapsedMilliseconds);

      Assert.Greater(withNone.Elapsed, withCompiled.Elapsed);
      Assert.Greater(withNoneAfter.Elapsed, withCompiled.Elapsed);
    }
  }
}
