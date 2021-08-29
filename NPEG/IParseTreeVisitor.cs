using System;
using System.Reflection;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG
{
  public abstract class IParseTreeVisitor
  {
    public void VisitEnter(AExpression expression)
    {
      // Use reflection to find and invoke the correct Visit method
      var types = new[] { expression.GetType() };
      MethodInfo methodInfo = GetType().GetMethod("VisitEnter", types);
      if (methodInfo != null)
      {
        methodInfo.Invoke(this, new object[] { expression });
      }
      else
      {
        throw new Exception("Visitor does not implement the Visit method for the type: " + expression.GetType());
      }
    }

    public void VisitExecute(AExpression expression)
    {
      // Use reflection to find and invoke the correct Visit method
      var types = new[] { expression.GetType() };
      MethodInfo methodInfo = GetType().GetMethod("VisitExecute", types);
      if (methodInfo != null)
      {
        methodInfo.Invoke(this, new object[] { expression });
      }
      else
      {
        throw new Exception("Visitor does not implement the Visit method for the type: " + expression.GetType());
      }
    }

    public void VisitLeave(AExpression expression)
    {
      // Use reflection to find and invoke the correct Visit method
      var types = new[] { expression.GetType() };
      MethodInfo methodInfo = GetType().GetMethod("VisitLeave", types);
      if (methodInfo != null)
      {
        methodInfo.Invoke(this, new object[] { expression });
      }
      else
      {
        throw new Exception("Visitor does not implement the Visit method for the type: " + expression.GetType());
      }
    }


    public void Visit(AExpression expression)
    {
      // Use reflection to find and invoke the correct Visit method
      var types = new[] { expression.GetType() };
      MethodInfo methodInfo = GetType().GetMethod("Visit", types);
      if (methodInfo != null)
      {
        methodInfo.Invoke(this, new object[] { expression });
      }
      else
      {
        throw new Exception("Visitor does not implement the Visit method for the type: " + expression.GetType());
      }
    }

    #region nonterminals

    public abstract void VisitEnter(AndPredicate expression);
    public abstract void VisitExecute(AndPredicate expression);
    public abstract void VisitLeave(AndPredicate expression);

    public abstract void VisitEnter(PrioritizedChoice expression);
    public abstract void VisitExecute(PrioritizedChoice expression);
    public abstract void VisitLeave(PrioritizedChoice expression);

    public abstract void VisitEnter(NotPredicate expression);
    public abstract void VisitExecute(NotPredicate expression);
    public abstract void VisitLeave(NotPredicate expression);

    public abstract void VisitEnter(ZeroOrMore expression);
    public abstract void VisitExecute(ZeroOrMore expression);
    public abstract void VisitLeave(ZeroOrMore expression);

    public abstract void VisitEnter(OneOrMore expression);
    public abstract void VisitExecute(OneOrMore expression);
    public abstract void VisitLeave(OneOrMore expression);

    public abstract void VisitEnter(Optional expression);
    public abstract void VisitExecute(Optional expression);
    public abstract void VisitLeave(Optional expression);

    public abstract void VisitEnter(Sequence expression);
    public abstract void VisitExecute(Sequence expression);
    public abstract void VisitLeave(Sequence expression);

    public abstract void VisitEnter(CapturingGroup expression);
    public abstract void VisitExecute(CapturingGroup expression);
    public abstract void VisitLeave(CapturingGroup expression);

    public abstract void VisitEnter(RecursionCreate expression);
    public abstract void VisitExecute(RecursionCreate expression);
    public abstract void VisitLeave(RecursionCreate expression);

    public abstract void VisitEnter(LimitingRepetition expression);
    public abstract void VisitExecute(LimitingRepetition expression);
    public abstract void VisitLeave(LimitingRepetition expression);

    #endregion

    #region terminals

    public abstract void Visit(Literal expression);
    public abstract void Visit(CharacterClass expression);
    public abstract void Visit(AnyCharacter expression);
    public abstract void Visit(RecursionCall expression);
    public abstract void Visit(DynamicBackReference expression);
    public abstract void Visit(CodePoint expression);
    public abstract void Visit(Warn expression);
    public abstract void Visit(Fatal expression);

    #endregion
  }
}
