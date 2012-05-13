package parser_tests;
import robusthaven.text.npeg.tests.parsers.NpegNode;
import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class NpegNodeTest extends TestCase {
    static final String input1 = "NpEg";
    static final String input2 = ".NET Parsing Expression Grammar";
    static final String input3 = "NET Parsing Expression Grammar";

    static final int BUFFER_SIZE = 100;
    byte[] buffer = new byte[BUFFER_SIZE];
    InputIterator input;
    NpegNode parser;
    AstNode ast;
    
    public void testInput1() {
	try {
	    input = new StringInputIterator(input1);
	    parser = new NpegNode(input);

	    Assert.assertTrue(parser.isMatch());
	    System.out.println("\tVerified: The expected input was matched by parser.");
	
	    ast = parser.getAST();
	    Assert.assertTrue(0 == ast.getToken().getName().compareTo("NpegNode"));
	    System.out.println("\tVerified: The expected token name: " + ast.getToken().getName());
	    /*
	     * Attention: trim is necessary as String interprets all 100 buffer chars as part of the
	     * string, hence there's a huge length mismatch without the application of String.trim to the 
	     * newly created string object.
	     */
	    input.getText(buffer, ast.getToken().getStart(), ast.getToken().getEnd());
	    Assert.assertTrue(0 == new String(buffer).trim().compareTo(input1));
	    System.out.println("\tVerified: The expected matched string: " + new String(buffer).trim());
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("Could not parse input1");
	}
    }
 
    public NpegNodeTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(NpegNodeTest.class);
	System.exit(0);
    }
}