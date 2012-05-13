import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class NotPredicateNonterminalTest extends TestCase {
    static public final String TESTSTRING = "test";
    static public final String OTHERSTRING = "something else";

    class _NotTest extends Npeg {
	private class _expression implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING.getBytes(), TESTSTRING.length(), true);
	    } 
	}

	public boolean isMatch() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return notPredicate(new _expression());
	}

	public _NotTest(InputIterator iterator)  {
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
    _NotTest m_context;

    public void testEmpty() {
	try {
	    m_iterator = new StringInputIterator(emptystring);
	    m_context = new _NotTest(m_iterator);
	    assert(m_context.isMatch() == true);
	    assert(m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of empty string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testSingle() {
	try {
	    m_iterator = new StringInputIterator(test1string);
	    m_context = new _NotTest(m_iterator);
	    assert(m_context.isMatch() == false);
	    assert(m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of single occurrence.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testNonOcccurrence() {
	try {
	    m_iterator = new StringInputIterator(otherstring);
	    m_context = new _NotTest(m_iterator);
	    assert(m_context.isMatch() == true && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of 0 occurrence in non-empty string.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testDouble() {
	try {
	    m_iterator = new StringInputIterator(test2string);
	    m_context = new _NotTest(m_iterator);
	    m_iterator.setIndex(TESTSTRING.length()/2);
	    assert(m_context.isMatch() == true && m_iterator.getIndex() == TESTSTRING.length()/2);
	    System.out.println("\tVerified: Handling of double occurrence.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testMiddle() {
	try {
	    m_iterator = new StringInputIterator(middle_test2string);
	    m_context = new _NotTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length());
	    assert(m_context.isMatch() == false
		   && m_iterator.getIndex() == OTHERSTRING.length());
	    System.out.println("\tVerified: operation with single occurrence of string "
			       + "w/ iterator in middle of string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public NotPredicateNonterminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(NotPredicateNonterminalTest.class);
	System.exit(0);
    }
}