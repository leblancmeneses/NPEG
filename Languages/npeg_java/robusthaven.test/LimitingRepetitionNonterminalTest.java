import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class LimitingRepetitionNonterminalTest extends TestCase {
    static public final String TESTSTRING = "test";
    static public final String OTHERSTRING = "somthing else";

    class _RepTest extends Npeg {
	int m_min, m_max;

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
	    return limitingRepetition(m_min, m_max, new _InfiniteLoopExpression(), "");
	}

	public void setBounds(int min, int max) {
	    m_min = min; m_max = max;
	}

	public boolean isMatch() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return limitingRepetition(m_min, m_max, new _expression(), "");
	}

	public _RepTest(InputIterator iterator) {
	    super(iterator, null);
	}
    }

    static public final String emptystring = "";
    static public final String otherstring = "blah";
    static public final String test1string = TESTSTRING + "Tes";
    static public final String test3string = TESTSTRING + TESTSTRING + TESTSTRING + "Tes";
    static public final String middle_test2string = OTHERSTRING + TESTSTRING + TESTSTRING + "blah";
    static public final String errmsg = "some kind of error";

    StringInputIterator m_iterator;
    _RepTest m_context;
    
    public void testEmpty() {
	try {
	    m_iterator = new StringInputIterator(emptystring);
	    m_context = new _RepTest(m_iterator);  
	    m_context.setBounds(0, 10);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == 0);
	    m_context.setBounds(1, 10);
	    Assert.assertTrue(m_context.isMatch() == false && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of empty string.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testNonOccurrence() {
	try {
	    m_iterator = new StringInputIterator(otherstring);
	    m_context = new _RepTest(m_iterator);
	    m_context.setBounds(0, 10);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == 0);
	    m_context.setBounds(1, 10);
	    Assert.assertTrue(m_context.isMatch() == false && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of 0 occurrence in non-empty string.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testTest1() {
	try {
	    m_iterator = new StringInputIterator(test1string);
	    m_context = new _RepTest(m_iterator);
	    m_context.setBounds(0, -1);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == TESTSTRING.length());
	    m_iterator.setIndex(0);
	    m_context.setBounds(1, 10);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == TESTSTRING.length());
	    m_iterator.setIndex(0);
	    m_context.setBounds(2, -1);
	    Assert.assertTrue(m_context.isMatch() == false && m_iterator.getIndex() == 0);	    
	    System.out.println("\tVerified: Handling of single occurrence.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInfiniteLoop() {
	try {
	    m_iterator = new StringInputIterator(otherstring);
	    m_context = new _RepTest(m_iterator);
	    m_context.setBounds(0, -1);
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

    public void testTest3() {
	try {
	    m_iterator = new StringInputIterator(test3string);
	    m_context = new _RepTest(m_iterator);
	    m_context.setBounds(0, 1);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == TESTSTRING.length());
	    m_iterator.setIndex(0);
	    m_context.setBounds(1, -1);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == 3*TESTSTRING.length());
	    m_iterator.setIndex(0);
	    m_context.setBounds(2, 2);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == 2*TESTSTRING.length());
	    m_iterator.setIndex(0);
	    m_context.setBounds(2, 3);
	    Assert.assertTrue(m_context.isMatch() == true && m_iterator.getIndex() == 3*TESTSTRING.length());
	    System.out.println("\tVerified: Handling of tripple occurrence.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testMiddle2() {
	try {
	    m_iterator = new StringInputIterator(middle_test2string);
	    m_context = new _RepTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length());
	    m_context.setBounds(0, 1);
	    Assert.assertTrue(m_context.isMatch() == true 
		   && m_iterator.getIndex() == OTHERSTRING.length() + TESTSTRING.length());
	    m_iterator.setIndex(OTHERSTRING.length());
	    m_context.setBounds(1, -1);
	    Assert.assertTrue(m_context.isMatch() == true 	 
		   && m_iterator.getIndex() == OTHERSTRING.length() + 2*TESTSTRING.length());
	    m_iterator.setIndex(OTHERSTRING.length());
	    m_context.setBounds(2, 2);
	    Assert.assertTrue(m_context.isMatch() == true
		   && m_iterator.getIndex() == OTHERSTRING.length() + 2*TESTSTRING.length());
	    m_iterator.setIndex(OTHERSTRING.length());
	    m_context.setBounds(2, 3);
	    Assert.assertTrue(m_context.isMatch() == true 
		   && m_iterator.getIndex() == OTHERSTRING.length() + 2*TESTSTRING.length());
	    System.out.println("\tVerified: Handling of double occurrence at center of string.");	
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testError() {
	try {
	    m_iterator = new StringInputIterator(middle_test2string);
	    m_context = new _RepTest(m_iterator);
	    m_iterator.setIndex(OTHERSTRING.length());  
	    m_context.setBounds(2, 1);    
	    m_context.isMatch();
	} catch (ParsingFatalTerminalException e) {
	    System.out.println("\tVerified: Handling of non-sensical repetition limits.");	
	    return;
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
	fail("Expected error not raised");
    }

    public LimitingRepetitionNonterminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(LimitingRepetitionNonterminalTest.class);
	System.exit(0);
    }
}