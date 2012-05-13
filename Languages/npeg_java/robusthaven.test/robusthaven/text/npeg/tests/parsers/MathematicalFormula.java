package robusthaven.text.npeg.tests.parsers;
import robusthaven.text.*;
import java.io.IOException;
import robusthaven.text.*;

public class MathematicalFormula extends Npeg
{
    public MathematicalFormula(InputIterator iterator)
    {
	super(iterator, null);
    }
    
    public boolean isMatch() throws IOException, InfiniteLoopException, ParsingFatalTerminalException {
	return new capture_expression().evaluate();
    }

    protected class capture_expression implements IsMatchPredicate {
	public boolean evaluate() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _nodeName_0 = "EXPRESSION";			    

	    return capturingGroup(new MathematicalFormula_impl_2(), _nodeName_0, false, false);	    	    
	}
    }

    protected class MathematicalFormula_impl_2 implements IsMatchPredicate
    {
	public boolean evaluate() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return sequence(new addend(), new sum());
	}
    }

    protected class sum implements IsMatchPredicate
    {
	public boolean evaluate() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return zeroOrMore(new additiveOp(), "");
	}
    }

    protected class additiveOp implements IsMatchPredicate
    {
	public boolean evaluate() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return sequence(new capture_additive(), new addend());
	}
    }

    protected class addend implements IsMatchPredicate
    {
	public boolean evaluate() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return sequence(new capture_operand(), new multiplicativeTermSeq());
	}
    }

    protected class multiplicativeTermSeq implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return zeroOrMore(new multiplicativeTerms(), "");
	}
    }

    protected class left_parenthesis implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _literal_0 = "(";

	    return literal(_literal_0.getBytes(), 1, true);
	}
    }

    protected class capture_number implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _nodeName_0 = "VALUE";

	    return capturingGroup(new number(), _nodeName_0, false, false);
	}
    }

    protected class digit implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _classExpression_0 = "[0-9]";
		
	    return characterClass(_classExpression_0.getBytes(), 5);
	}
    }

    protected class capture_operand implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return prioritizedChoice(new capture_number(), new subexpression());
	}
    }

    protected class right_parenthesis implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _literal_0 = ")";

	    return literal(_literal_0.getBytes(), 1, true);
	}
    }

    protected class capture_additive implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _nodeName_0 = "SYMBOL";
	    
	    return capturingGroup(new additiveOperators(), _nodeName_0, false, false);
	}
    }

    protected class additiveOperators implements IsMatchPredicate {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return prioritizedChoice(new plusOperator(), new minusOperator());
	}
    }

    protected class minusOperator implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _literal_0 = "-";
	    
	    return literal(_literal_0.getBytes(), 1, true);
	}
    }

    protected class plusOperator implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _literal_0 = "+";

	    return literal(_literal_0.getBytes(), 1, true);
	}
    }

    protected class multiplicativeTerms implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return sequence(new capture_multiplicative(), new capture_operand());
	}
    }

    protected class capture_multiplicative implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _nodeName_0 = "SYMBOL";

	    return capturingGroup(new multiplicativeOperators(), _nodeName_0, false, false);
	}
    }

    protected class multiplicativeOperators implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return prioritizedChoice(new multiplicationOperator(), new divisionOperator());
	}
    }

    protected class divisionOperator implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _literal_0 = "/";
	
	    return literal(_literal_0.getBytes(), 1, true);
	}
    }

    protected class multiplicationOperator implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    String _literal_0 = "*";

	    return literal(_literal_0.getBytes(), 1, true);
	}
    }

    protected class subexpression implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return sequence(new subexpression_1(), new right_parenthesis());
	}
    }

    protected class subexpression_1 implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return sequence(new left_parenthesis(), new capture_expression());
	}
    }

    protected class number implements IsMatchPredicate
    {
	public boolean evaluate() 		
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return oneOrMore(new digit(), "");
	}
    }
}
