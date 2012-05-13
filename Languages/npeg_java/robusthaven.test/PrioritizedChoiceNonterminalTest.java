import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;


public class PrioritizedChoiceNonterminalTest extends TestCase {
    static public final String TESTSTRING1 = "test";
    static public final String TESTSTRING2 = "test with extension";
    static public final String OTHERSTRING = "something else";

    class _ChoiceTest extends Npeg {
	private class _expression1 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING1.getBytes(), TESTSTRING1.length(), true);
	    } 
	}

	private class _expression2 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING2.getBytes(), TESTSTRING2.length(), true);
	    } 
	}

	public boolean isMatch() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return prioritizedChoice(new _expression1(), new _expression2());
	}

	public boolean isMatch2() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return prioritizedChoice(new _expression2(), new _expression1());
	}

	public _ChoiceTest(InputIterator iterator)  {
	    super(iterator, null);
	}
    }

    static public final String emptystring = "";
    static public final String otherstring = "blah";
    static public final String test1string1 = TESTSTRING1 + "blah";
    static public final String test2string2 = TESTSTRING2 + "blah";
    static public final String test1string1_1string2 = TESTSTRING1 + TESTSTRING2 + "blah";
    static public final String middle_test1string2 = OTHERSTRING + TESTSTRING2 + "blah";
    static public final String errmsg = "some kind of error";

    StringInputIterator m_iterator;
    _ChoiceTest m_context;

    public void testEmpty() {
	try {
	    m_iterator = new StringInputIterator(emptystring);
	    m_context = new _ChoiceTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == false);
	    Assert.assertTrue(m_iterator.getIndex() == 0);
	    Assert.assertTrue(m_context.isMatch2() == false);
	    Assert.assertTrue(m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of empty string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testNonOcccurrence() {
	try {
	    m_iterator = new StringInputIterator(otherstring);
	    m_context = new _ChoiceTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == false && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of 0 occurrence in non-empty string.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testSingle() {
	try {
	    m_iterator = new StringInputIterator(test1string1);
	    m_context = new _ChoiceTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == true);
	    Assert.assertTrue(m_iterator.getIndex() == TESTSTRING1.length());
	    System.out.println("\tVerified: Handling of single occurrence.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testSingleLong() {
	try {
	    m_iterator = new StringInputIterator(test2string2);
	    m_context = new _ChoiceTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == true);
	    Assert.assertTrue(m_iterator.getIndex() == TESTSTRING1.length());
	    m_iterator.setIndex(0);
	    Assert.assertTrue(m_context.isMatch2() == true);
	    Assert.assertTrue(m_iterator.getIndex() == TESTSTRING2.length());
	    System.out.println("\tVerified: Handling of single occurence of long string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testShortLong() {
	try {
	    m_iterator = new StringInputIterator(test1string1_1string2);
	    m_context = new _ChoiceTest(m_iterator);
	    Assert.assertTrue(m_context.isMatch() == true);
	    Assert.assertTrue(m_iterator.getIndex() == TESTSTRING1.length());
	    System.out.println("\tVerified: Handling of single occurence of short string "
			       + "before long string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testMiddle() {
	try {
	    m_iterator = new StringInputIterator(middle_test1string2);
	    m_context = new _ChoiceTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length());
	    Assert.assertTrue(m_context.isMatch() == true);
	    Assert.assertTrue(m_iterator.getIndex() == OTHERSTRING.length() + TESTSTRING1.length());
	    System.out.println("\tVerified: Handling of single occurence of short string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public PrioritizedChoiceNonterminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(PrioritizedChoiceNonterminalTest.class);
	System.exit(0);
    }
}