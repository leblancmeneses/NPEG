import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;


public class SequenceNonterminalTest extends TestCase {
    static public final String TESTSTRING1 = "test1";
    static public final String TESTSTRING2 = "test__2";
    static public final String OTHERSTRING = "something else";

    class _SeqTest extends Npeg {
	private class _expression1 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING1.getBytes("UTF-8"), TESTSTRING1.length(), true);
	    } 
	}

	private class _expression2 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING2.getBytes("UTF-8"), TESTSTRING2.length(), true);
	    } 
	}

	public boolean isMatch() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return sequence(new _expression1(), new _expression2());
	}

	public boolean isMatch2() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return sequence(new _expression2(), new _expression1());
	}


	public _SeqTest(InputIterator iterator)  {
	    super(iterator, null);
	}
    }

    static public final String emptystring = "";
    static public final String test1string1 = TESTSTRING1 + "blah";
    static public final String test2string2 = TESTSTRING2 + "blah";
    static public final String test1string1_1string2 = TESTSTRING1 + TESTSTRING2 + "blah";
    static public final String middle_test1string2_1string1 = OTHERSTRING + TESTSTRING1 + 
	TESTSTRING2 + "blah";
    static public final String errmsg = "some kind of error";

    StringInputIterator m_iterator;
    _SeqTest m_context;

    public void testEmpty() {
	try {
	    m_iterator = new StringInputIterator(emptystring);
	    m_context = new _SeqTest(m_iterator);
	    assert(m_context.isMatch() == false);
	    assert(m_iterator.getIndex() == 0);
	    assert(m_context.isMatch2() == false);
	    assert(m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of empty string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testPartial() {
	try {
	    m_iterator = new StringInputIterator(test1string1);
	    m_context = new _SeqTest(m_iterator);
	    assert(m_context.isMatch() == false && m_iterator.getIndex() == 0);
	    assert(m_context.isMatch2() == false && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of partial sequence.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testSingle() {
	try {
	    m_iterator = new StringInputIterator(test1string1_1string2);
	    m_context = new _SeqTest(m_iterator);
	    assert(m_context.isMatch() == true);
	    assert(m_iterator.getIndex() == TESTSTRING1.length() + TESTSTRING2.length());
	    m_iterator.setIndex(0);
	    assert(m_context.isMatch2() == false && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of full sequence.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testFull() {
	try {
	    m_iterator = new StringInputIterator(middle_test1string2_1string1);
	    m_context = new _SeqTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length());
	    assert(m_context.isMatch() == true);
	    assert(m_iterator.getIndex() == OTHERSTRING.length() + TESTSTRING2.length() 
		   + TESTSTRING1.length());
	    m_iterator.setIndex(OTHERSTRING.length());
	    assert(m_context.isMatch2() == false);
	    assert(m_iterator.getIndex() == OTHERSTRING.length());
	    System.out.println("\tVerified: Handling of of full sequence at random position.\n");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public SequenceNonterminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(SequenceNonterminalTest.class);
	System.exit(0);
    }
}