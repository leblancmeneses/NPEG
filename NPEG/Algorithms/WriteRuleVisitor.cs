using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.Algorithms
{
  // write rules for existing composite
  public class WriteRuleVisitor : IParseTreeVisitor
  {
    private readonly List<String> statements = new List<string>();
    private readonly Stack<StringBuilder> terminal = new Stack<StringBuilder>();

    public WriteRuleVisitor()
    {
      terminal.Push(new StringBuilder());
    }

    public override void Visit(AnyCharacter expression)
    {
      terminal.Peek().Append('.');
    }

    public override void Visit(CharacterClass expression)
    {
      terminal.Peek().Append(expression.ClassExpression);
    }

    public override void Visit(Literal expression)
    {
      terminal.Peek().Append("'");
      terminal.Peek().Append(Regex.Replace(expression.MatchText, @"'", @"\'"));
      terminal.Peek().Append("'");
      if (!expression.IsCaseSensitive)
      {
        terminal.Peek().Append(@"\i");
      }
    }

    public override void Visit(CodePoint expression)
    {
      terminal.Peek().Append(expression.Match);
    }

    public override void Visit(DynamicBackReference expression)
    {
      terminal.Peek().Append(@"\k<");
      terminal.Peek().Append(expression.BackReferenceName);
      terminal.Peek().Append(">");
      if (!expression.IsCaseSensitive)
      {
        terminal.Peek().Append("[");
        terminal.Peek().Append(@"\i");
        terminal.Peek().Append("]");
      }
    }

    public override void Visit(RecursionCall expression)
    {
      // Terminal Reference Name
      terminal.Peek().Append(expression.FunctionName);
    }


    private readonly Dictionary<String, String> uniqueCapturedGroup = new Dictionary<string, string>();
    //  name, rule text
    public override void VisitEnter(CapturingGroup expression)
    {
      terminal.Push(new StringBuilder());
    }

    public override void VisitExecute(CapturingGroup expression)
    {
    }

    public override void VisitLeave(CapturingGroup expression)
    {
      String rule = terminal.Pop().ToString();

      var nodeText = new StringBuilder();
      nodeText.Append("(?<");
      nodeText.Append(expression.Name);
      if (expression.DoCreateCustomAstNode || expression.DoReplaceBySingleChildNode)
      {
        nodeText.Append("[");

        if (expression.DoReplaceBySingleChildNode)
          nodeText.Append("\rsc");

        if (expression.DoCreateCustomAstNode)
        {
          nodeText.Append("\rn");
        }

        nodeText.Append("]");
      }
      nodeText.Append(">");


      if (uniqueCapturedGroup.ContainsKey(expression.Name))
      {
        if (uniqueCapturedGroup[expression.Name] == rule)
        {
          terminal.Peek().Append(expression.Name);
        }
        else
        {
          //same name but different rule so write inline
          nodeText.Append(rule);
          nodeText.Append(")");

          terminal.Peek().Append(nodeText.ToString());
        }
      }
      else
      {
        if (uniqueCapturedGroup.ContainsValue(rule))
        {
          // different name same rule
          String name = uniqueCapturedGroup.Where(kvp => kvp.Value == rule).Select(kvp => kvp.Key).First();
          terminal.Peek().Append(name);
        }
        else
        {
          nodeText.Append("): ");
          nodeText.Append(rule);
          nodeText.Append(";");
          statements.Add(nodeText.ToString());
          uniqueCapturedGroup.Add(expression.Name, rule);

          if (terminal.Count > 0)
            terminal.Peek().Append(expression.Name);
        }
      }
    }


    private readonly Dictionary<String, String> uniqueRecursionBlocks = new Dictionary<string, string>();
    //rule, name
    public override void VisitEnter(RecursionCreate expression)
    {
      terminal.Push(new StringBuilder());
    }

    public override void VisitExecute(RecursionCreate expression)
    {
    }

    public override void VisitLeave(RecursionCreate expression)
    {
      String rule = terminal.Pop().ToString();
      if (!uniqueRecursionBlocks.ContainsKey(rule))
      {
        uniqueRecursionBlocks.Add(rule, expression.FunctionName);

        var nodeText = new StringBuilder();
        nodeText.AppendFormat("{0}: {1};", expression.FunctionName, rule);
        statements.Add(nodeText.ToString());
      }

      if (terminal.Count > 0)
        terminal.Peek().Append(uniqueRecursionBlocks[rule]);
    }


    public override void VisitEnter(AndPredicate expression)
    {
      terminal.Peek().Append("&(");
    }

    public override void VisitExecute(AndPredicate expression)
    {
    }

    public override void VisitLeave(AndPredicate expression)
    {
      terminal.Peek().Append(") ");
    }


    public override void VisitEnter(NotPredicate expression)
    {
      terminal.Peek().Append("!(");
    }

    public override void VisitExecute(NotPredicate expression)
    {
    }

    public override void VisitLeave(NotPredicate expression)
    {
      terminal.Peek().Append(")");
    }


    public override void VisitEnter(OneOrMore expression)
    {
      terminal.Peek().Append("(");
    }

    public override void VisitExecute(OneOrMore expression)
    {
    }

    public override void VisitLeave(OneOrMore expression)
    {
      terminal.Peek().Append(")+");
    }


    public override void VisitEnter(Optional expression)
    {
      terminal.Peek().Append("(");
    }

    public override void VisitExecute(Optional expression)
    {
    }

    public override void VisitLeave(Optional expression)
    {
      terminal.Peek().Append(")?");
    }


    public override void VisitEnter(ZeroOrMore expression)
    {
      terminal.Peek().Append("(");
    }

    public override void VisitExecute(ZeroOrMore expression)
    {
    }

    public override void VisitLeave(ZeroOrMore expression)
    {
      terminal.Peek().Append(")*");
    }


    public override void VisitEnter(LimitingRepetition expression)
    {
      terminal.Peek().Append("(");
    }

    public override void VisitExecute(LimitingRepetition expression)
    {
    }

    public override void VisitLeave(LimitingRepetition expression)
    {
      terminal.Peek().Append("){");

      if (expression.Max == expression.Min)
      {
        if (expression.Min == null)
          throw new ArgumentException("Min and Max should not be null.");

        // exact count
        terminal.Peek().Append(expression.Max.ToString());
      }
      else if (expression.Max == null)
      {
        // only min limit
        terminal.Peek().Append(expression.Min.ToString());
        terminal.Peek().Append(",");
      }
      else if (expression.Min == null)
      {
        // only max limit
        terminal.Peek().Append(",");
        terminal.Peek().Append(expression.Max.ToString());
      }
      else
      {
        // both min and max limit set
        terminal.Peek().Append(expression.Min.ToString());
        terminal.Peek().Append(",");
        terminal.Peek().Append(expression.Max.ToString());
      }

      terminal.Peek().Append(@"}");
    }

    public override void Visit(Warn expression)
    {
      terminal.Peek().AppendFormat("/Warn<'{0}'>", Regex.Replace(expression.Message, "'", @"\'"));
    }

    public override void Visit(Fatal expression)
    {
      terminal.Peek().AppendFormat("/Fatal<'{0}'>", Regex.Replace(expression.Message, "'", @"\'"));
    }


#warning create nodes of repeated rules.. place this in prioritychoice / sequence
    private Dictionary<String, String> uniqueBranches = new Dictionary<string, string>();
    // value of node, name
    //Int32 branchcount = 0;

    public override void VisitEnter(PrioritizedChoice expression)
    {
      terminal.Peek().Append("(");
    }

    public override void VisitExecute(PrioritizedChoice expression)
    {
      terminal.Peek().Append(" / ");
    }

    public override void VisitLeave(PrioritizedChoice expression)
    {
      terminal.Peek().Append(")");

      //String input = terminal.Pop().ToString();
      //if (!this.uniqueBranches.ContainsKey(input))
      //{
      //    String nodename = "node" + branchcount++;
      //    this.uniqueBranches.Add(input, nodename);
      //    this.statements.Add( String.Format("{0}: {1};", nodename, input) );
      //}

      //// remember last node is always a captured group so peek should not throw exceptions
      //// insert terminal name
      //terminal.Peek().Append(this.uniqueBranches[input]);
    }


    public override void VisitEnter(Sequence expression)
    {
      //this.terminal.Push(new StringBuilder());
    }

    public override void VisitExecute(Sequence expression)
    {
      terminal.Peek().Append(" ");
    }

    public override void VisitLeave(Sequence expression)
    {
      //String input = terminal.Pop().ToString();
      //if (!this.uniqueBranches.ContainsKey(input))
      //{
      //    String nodename = "node" + branchcount++;
      //    this.uniqueBranches.Add(input, nodename);
      //    this.statements.Add(String.Format("{0}: {1};", nodename, input));
      //}

      //// remember last node is always a captured group so peek should not throw exceptions
      //// insert terminal name
      //terminal.Peek().Append(this.uniqueBranches[input]);
    }


    public String GrammarOutput
    {
      get
      {
        var grammar = new StringBuilder();
        string[] q = statements.Distinct().ToArray();
        foreach (String s in q)
        {
          grammar.Append(s + Environment.NewLine);
        }

        return grammar.ToString();
      }
    }
  }
}
