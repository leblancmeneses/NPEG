import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class ZeroOrMoreNonterminalTest extends TestCase {
    static public final String TESTSTRING = "test";
    static public final String OTHERSTRING = "something else";

    class _ZeroTest extends Npeg {
	private class _expression implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING.getBytes("UTF-8"), TESTSTRING.length(), true);
	    } 
	}

	private class _InfiniteLoopExpression implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return notPredicate(new _expression());
	    }		
	}

	public boolean bad() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return zeroOrMore(new _InfiniteLoopExpression(), "");
	}


	public boolean isMatch() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return zeroOrMore(new _expression(), "");
	}

	public _ZeroTest(InputIterator iterator) {
	    super(iterator, null);
	}
    }

    static public final String emptystring = "";
    static public final String otherstring = "blah";
    static public final String test1string = TESTSTRING + "Tes";
    static public final String test2string = TESTSTRING + TESTSTRING + "Tes";
    static public final String middle_test2string = OTHERSTRING + TESTSTRING + TESTSTRING + "blah";
    static public final String errmsg = "some kind of error";

    StringInputIterator m_iterator;
    _ZeroTest m_context;

    public void testEmpty() {
	try {
	    m_iterator = new StringInputIterator(emptystring);
	    m_context = new _ZeroTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of empty string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testNonOcccurrence() {
	try {
	    m_iterator = new StringInputIterator(otherstring);
	    m_context = new _ZeroTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of 0 occurrence in non-empty string.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testSingle() {
	try {
	    m_iterator = new StringInputIterator(test1string);
	    m_context = new _ZeroTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == true 
		   && m_iterator.getIndex() == TESTSTRING.length());
	    System.out.println("\tVerified: Handling of single occurrence.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }
	
    public void testDouble() {
	try {
	    m_iterator = new StringInputIterator(test2string);
	    m_context = new _ZeroTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == true 
		   && m_iterator.getIndex() == 2*TESTSTRING.length());
	    System.out.println("\tVerified: Handling of double occurrence.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testMiddle() {
	try {
	    m_iterator = new StringInputIterator(middle_test2string);
	    m_context = new _ZeroTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length());
	    Assert.assertTrue(m_context.isMatch() == true 
		   && m_iterator.getIndex() == 2*TESTSTRING.length() + OTHERSTRING.length());
	    System.out.println("\tVerified: Handling of double occurrence at string center.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInfiniteLoop() {
	try {
	    m_iterator = new StringInputIterator(otherstring);
	    m_context = new _ZeroTest(m_iterator);
	    Assert.assertTrue(m_context.bad() == true);
	} catch (InfiniteLoopException e) {
	    System.out.println("\tVerified: Detection of infinite loops.");	
	    return;
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
	fail("Inifnite loop detection failed");
    }

    public ZeroOrMoreNonterminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(ZeroOrMoreNonterminalTest.class);
	System.exit(0);
    }
}