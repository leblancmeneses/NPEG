import robusthaven.text.*;
import java.util.*;
import java.io.*;
import junit.framework.Assert;
import junit.framework.TestCase;

public class LiteralTerminalTest extends TestCase {
    public static String g_matchText = ".NET Parsing Expression Grammar";

    class LiteralTest extends Npeg {
	boolean m_isCaseSensitive;

	public void makeCaseSensitive() {
	    m_isCaseSensitive = true;
	}

	public boolean isMatch() throws IOException, ParsingFatalTerminalException {
	    return literal(g_matchText.getBytes(), g_matchText.length(), m_isCaseSensitive);
	}

	public LiteralTest(InputIterator iter) {
	    super(iter, null);
	    m_isCaseSensitive = false;
	}
    }


    public static final String string = ".nEt Parsing expression grammar";
    public static final String string2 = ".NET Parsing Expression Grammar";
    public static final String string3 = "invalid";
	
    StringInputIterator iterator;
    LiteralTest context;

    public void testInput1() {
	try {
	    iterator = new StringInputIterator(string.getBytes(), string.length());
	    context = new LiteralTest(iterator);
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: branch of isCaseSensitive = false; input1 successfully matches.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput2() {
	try {
	    iterator = new StringInputIterator(string2.getBytes(), string2.length());
	    context = new LiteralTest(iterator);
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: branch of isCaseSensitive = false; input2 successfully matches.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput3Fail() {
	try {
	    iterator = new StringInputIterator(string3.getBytes(), string3.length());
	    context = new LiteralTest(iterator);
	    Assert.assertTrue(false == context.isMatch());
	    System.out.println("\tVerified: branch of isCaseSensitive = false; input3 is NOT matched.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }
	
    public void testInput1Fail() {
	try {
	    iterator = new StringInputIterator(string.getBytes(), string.length());
	    context = new LiteralTest(iterator);
	    context.makeCaseSensitive();
	    Assert.assertTrue(false == context.isMatch());
	    System.out.println("\tVerified: branch of isCaseSensitive = true; input1 is NOT matched.");
		} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput2CS() {
	try {
	    iterator = new StringInputIterator(string2.getBytes(), string2.length());
	    context = new LiteralTest(iterator);
	    context.makeCaseSensitive();
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: branch of isCaseSensitive = true; input2 is matched.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

	
    public void testInput3FailCS() {
	try {
	    iterator = new StringInputIterator(string3.getBytes(), string3.length());
	    context = new LiteralTest(iterator);
	    context.makeCaseSensitive();
	    Assert.assertTrue(false == context.isMatch());
	    System.out.println("\tVerified: branch of isCaseSensitive = true; input3 is NOT matched.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public LiteralTerminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(LiteralTerminalTest.class);
	System.exit(0);
    }
}

