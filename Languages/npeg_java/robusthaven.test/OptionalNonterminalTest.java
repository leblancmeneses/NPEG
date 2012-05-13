import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class OptionalNonterminalTest extends TestCase {
    static public final String TESTSTRING = "test";
    static public final String OTHERSTRING = "something else";

    class _OptTest extends Npeg {
	private class _expression implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING.getBytes("UTF-8"), TESTSTRING.length(), true);
	    } 
	}

	public boolean isMatch() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return optional(new _expression());
	}

	public _OptTest(InputIterator iterator)  {
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
    _OptTest m_context;

    public void testEmpty() {
	try {
	    m_iterator = new StringInputIterator(emptystring);
	    m_context = new _OptTest(m_iterator);
	    assert(m_context.isMatch() == true);
	    assert(m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of empty string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testNonOcccurrence() {
	try {
	    m_iterator = new StringInputIterator(otherstring);
	    m_context = new _OptTest(m_iterator);
	    assert(m_context.isMatch() == true && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of 0 occurrence in non-empty string.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testSingle() {
	try {
	    m_iterator = new StringInputIterator(test1string);
	    m_context = new _OptTest(m_iterator);
	    assert(m_context.isMatch() == true);
	    assert(m_iterator.getIndex() == TESTSTRING.length());
	    System.out.println("\tVerified: Handling of single occurrence.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testDouble() {
	try {
	    m_iterator = new StringInputIterator(test2string);
	    m_context = new _OptTest(m_iterator);
	    assert(m_context.isMatch() == true 
		   && m_iterator.getIndex() == TESTSTRING.length());
	    System.out.println("\tVerified: Single consumption of double occurrence.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testMiddle() {
	try {
	    m_iterator = new StringInputIterator(middle_test2string);
	    m_context = new _OptTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length());
	    assert(m_context.isMatch() == true 
		   && m_iterator.getIndex() == TESTSTRING.length() + OTHERSTRING.length());
	    System.out.println("\tVerified: Single consumption of double occurrence at string center.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public OptionalNonterminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(OptionalNonterminalTest.class);
	System.exit(0);
    }
}