import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class CapturingGroupNonterminalTest extends TestCase {
    public static final String TESTSTRING1 = "test";
    public static final String TESTSTRING2 = "TEST";
    public static final String OTHERSTRING = "something else";

    public static final String _name1 = "1st teststring";
    public static final String _name2 = "2nd teststring";
    public static final String _name3 = "1/2 sequence";
    public static final String _name4 = "2 sequence unreduced";

    class CaptTest extends Npeg {
	protected class match_expression1 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING1.getBytes(), TESTSTRING1.length(), true);
	    } 
	}

	protected class match_expression2 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING1.getBytes(), TESTSTRING1.length(), false);
	    }
	} 

	public boolean capture_expression1() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return capturingGroup(new match_expression1(), _name1, false, false);
	}

	protected class capture_expression1Predicate implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return capture_expression1();
	    }
	}

	public boolean capture_expression2() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return capturingGroup(new match_expression2(), _name2, false, false);
	}

	protected class capture_expression2Predicate implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return capture_expression2();
	    }
	}

	protected class match_expression12 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return sequence(new capture_expression1Predicate(), new capture_expression2Predicate());
	    }
	}

	public boolean capture_expression3() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return capturingGroup(new match_expression12(), _name3, false, false);
	}

	public boolean capture_expression2_red() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return capturingGroup(new capture_expression2Predicate(), _name4, true, false);
	}

	protected class forced_error implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return limitingRepetition(4, 3, new match_expression12(), "");
	    }
	}

	public boolean capture_fatal() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return capturingGroup(new forced_error(), _name4, true, false);
	}
    
	public boolean isMatch() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return true;
	}
    
	public CaptTest(InputIterator iterator) {
	    super(iterator, null);
	}
    }

    private StringInputIterator m_iterator;
    CaptTest m_context;
    AstNode m_ast;

    public void testEmpty() {
	final String emptystring = "";

	try {
	    m_iterator = new StringInputIterator(emptystring);
	    m_context = new CaptTest(m_iterator);
	    Assert.assertTrue(m_context.capture_expression1() == false && m_iterator.getIndex() == 0);
	    Assert.assertTrue(m_context.capture_expression2() == false && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: no change of internal state when operating on empty string");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testMismatch() {
	final String middle_test12string = OTHERSTRING + TESTSTRING1 + TESTSTRING2 + "blah";

	try {
	    m_iterator = new StringInputIterator(middle_test12string);
	    m_context = new CaptTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length()/2);
	    Assert.assertTrue(m_context.capture_expression1() == false 
		   && m_iterator.getIndex() == OTHERSTRING.length()/2);
	    Assert.assertTrue(m_context.capture_expression2() == false 
		   && m_iterator.getIndex() == OTHERSTRING.length()/2);
	    System.out.println("\tVerified: no change of internal state when no occurence of expression");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testTest1() {
	final String test1string = TESTSTRING1 + "blah";

	try {
	    m_iterator = new StringInputIterator(test1string);
	    m_context = new CaptTest(m_iterator);
	    Assert.assertTrue(m_context.capture_expression1() == true
			      && m_iterator.getIndex() == TESTSTRING1.length());
	    m_ast = m_context.getAST();
	    Assert.assertTrue(m_ast.getToken().getName() == _name1);
	    System.out.println("\tVerified: capturing of simple occurence of expression1");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected exception");
	}
    }

    public void testFatal() {
	final String errmsg = "some kind of error";

	try {
	    m_iterator = new StringInputIterator(errmsg);
	    m_context = new CaptTest(m_iterator);
	    m_context.capture_fatal();
	} catch (ParsingFatalTerminalException e) {
	    System.out.println("\tVerified: Abortion on error without modification of state.");  
	    return;
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected exception");
	}
	fail("No exception thrown despite forced error");
    }


    public void testTest2() {
	final String test2string = TESTSTRING2 + "blah";

	try {
	    m_iterator = new StringInputIterator(test2string);
	    m_context = new CaptTest(m_iterator);
	    Assert.assertTrue(m_context.capture_expression2() == true
		   && m_iterator.getIndex() == TESTSTRING2.length());
	    m_ast = m_context.getAST();
	    Assert.assertTrue(m_ast.getToken().getName() == _name2);
	    
	    System.out.println("\tVerified: capturing of simple occurence of expression2");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected exception");
	}
    }

    public void testMiddle12() {
	try {
	    final String middle_test12string = OTHERSTRING + TESTSTRING1 + TESTSTRING2 + "blah";
	    
	    m_iterator = new StringInputIterator(middle_test12string);
	    m_context = new CaptTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length());
	    Assert.assertTrue(m_context.capture_expression3() == true
		   && m_iterator.getIndex() == OTHERSTRING.length() + TESTSTRING1.length() 
		   + TESTSTRING2.length());
	    m_ast = m_context.getAST();  
	    Assert.assertTrue(m_ast.getToken().getName() == _name3 && m_ast.nofChildren() == 2);	    
	    System.out.println("\tVerified: capturing of 1/2 sequence");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected exception");
	}
    }

    public void testReduction() {
	final String test2string = TESTSTRING2 + "blah";

	try {
	    m_iterator = new StringInputIterator(test2string);
	    m_context = new CaptTest(m_iterator);
	    Assert.assertTrue(m_context.capture_expression2_red() == true 
		   && m_iterator.getIndex() == TESTSTRING2.length());
	    m_ast = m_context.getAST();
	    Assert.assertTrue(m_ast.getToken().getName() == _name2 && m_ast.nofChildren() == 0);	    
	    System.out.println("\tVerified: capturing of redundant expression with reduction "
			       + "to single node");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected exception");
	}
    }

    public CapturingGroupNonterminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(CapturingGroupNonterminalTest.class);
	System.exit(0);
    }
}