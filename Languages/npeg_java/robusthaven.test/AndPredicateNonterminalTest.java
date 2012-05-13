import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class AndPredicateNonterminalTest extends TestCase {
    class AndTest extends Npeg {
	public static final String TESTSTRING1 = "test";
	public static final String TESTSTRING2 = "TEST";
	public static final String OTHERSTRING = "something else";
	
	public static final String emptystring = "";
	public static final String test1string = TESTSTRING1 + "blah";
	public static final String not_test1string = TESTSTRING2 + "blah";
	public static final String test2string = TESTSTRING1 + TESTSTRING1 + TESTSTRING2 + "blah";
	public static final String middle_test2string = OTHERSTRING + TESTSTRING1 + TESTSTRING1 
	    + TESTSTRING2 + "blah";
	public static final String errmsg = "some kind of error";

	protected class sub_expression1 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING1.getBytes(), TESTSTRING1.length(), true);
	    }
	} 
	
	protected class sub_expression2 implements IsMatchPredicate {
	    public boolean evaluate() 
		throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
		return literal(TESTSTRING1.getBytes(), TESTSTRING1.length(), false);
	    }
	} 
	
	protected boolean expression1() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return andPredicate(new sub_expression1()) && new sub_expression2().evaluate();
	}

	protected boolean expression2() 
	    throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return andPredicate(new sub_expression2()) && new sub_expression1().evaluate();
	}

	public boolean isMatch() throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	    return true;
	}

	AndTest(InputIterator iterator) {
	    super(iterator, null);
	}
    }

    StringInputIterator m_iterator;
    AndTest m_context;

    public void testEmpty() {	
	try {
	    m_iterator = new StringInputIterator(AndTest.emptystring.getBytes(), 0);  
	    m_context = new AndTest(m_iterator);
	    Assert.assertTrue(m_context.expression1() == false && m_iterator.getIndex() == 0);
	    Assert.assertTrue(m_context.expression2() == false && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of empty string.\n");
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	    
	}
    }

    public void testSingle() {
	try {
	    m_iterator = new StringInputIterator(AndTest.test1string.getBytes(), 
						 AndTest.test1string.length());
	    m_context = new AndTest(m_iterator);
	    Assert.assertTrue(m_context.expression1() == true 
			      && m_iterator.getIndex() == AndTest.TESTSTRING1.length());
	    m_iterator.setIndex(0);
	    Assert.assertTrue(m_context.expression2() == true 
			      && m_iterator.getIndex() == AndTest.TESTSTRING1.length());
	    System.out.println("\tVerified: Handling of single occurence.");
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }

    public void testNonOccurrence() {
	try {
	    m_iterator = new StringInputIterator(AndTest.not_test1string.getBytes(), 
						 AndTest.not_test1string.length());
	    m_context = new AndTest(m_iterator);
	    assertTrue(m_context.expression1() == false && m_iterator.getIndex() == 0);
	    assertTrue(m_context.expression2() == false && m_iterator.getIndex() == 0);
	    System.out.println("\tVerified: Handling of non-occurence.\n");
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }
	    
    public void testDouble() {
	try {
	    m_iterator = new StringInputIterator(AndTest.test2string.getBytes(), 
						 AndTest.test2string.length());
	    m_context = new AndTest(m_iterator);
	    assertTrue(m_context.expression1() == true 
		       && m_iterator.getIndex() == AndTest.TESTSTRING1.length());
	    m_iterator.setIndex(0);
	    assertTrue(m_context.expression2() == true 
		       && m_iterator.getIndex() == AndTest.TESTSTRING1.length());
	    System.out.println("\tVerified: Handling of double occurence.\n");
	    
	    m_iterator = new StringInputIterator(AndTest.middle_test2string.getBytes(), 
						 AndTest.middle_test2string.length());
	    m_context = new AndTest(m_iterator);
	    m_iterator.setIndex(AndTest.OTHERSTRING.length());
	    assertTrue(m_context.expression1() == true 
		       && m_iterator.getIndex() == AndTest.OTHERSTRING.length() 
		       + AndTest.TESTSTRING1.length());
	    m_iterator.setIndex(AndTest.OTHERSTRING.length());
	    assertTrue(m_context.expression2() == true 
		       && m_iterator.getIndex() == AndTest.OTHERSTRING.length() 
		       + AndTest.TESTSTRING1.length());
	    System.out.println("\tVerified: Handling of double occurence at center of string.\n");
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }

    public AndPredicateNonterminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(AndPredicateNonterminalTest.class);
	System.exit(0);
    }
}
