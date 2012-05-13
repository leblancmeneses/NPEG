package parser_tests;
import robusthaven.text.npeg.tests.parsers.MathematicalFormula;
import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class MathematicalFormulaTest extends TestCase {
    static final String text1 = "(1*3+4)/5*93";
    static final String text2 = "9+(9-8)*10";  

    InputIterator input;
    MathematicalFormula parser;
    AstNode ast, child;
    
    public void testInput1() {
	String capturedText;

	try {
	    input = new StringInputIterator(text1);
	    parser = new MathematicalFormula(input);

	    Assert.assertTrue(parser.isMatch());
	    System.out.println("\tVerified: Matching of input succeeds\n");
	
	    ast = parser.getAST();
	    PrintVisitor.printAST(ast);
	    Assert.assertTrue(0 == ast.getToken().getName().compareTo("EXPRESSION"));
	    System.out.println("\tVerified: The expected token name: " + ast.getToken().getName());
	    /*
	     * Attention: trim is necessary as String interprets all 100 buffer chars as part of the
	     * string, hence there's a huge length mismatch without the application of String.trim to the 
	     * newly created string object.
	     */
	    capturedText = input.getText(ast.getToken().getStart(), ast.getToken().getEnd());	    
	    Assert.assertTrue(0 == capturedText.compareTo(text1));
	    System.out.println("\tVerified: The expected matched string: " + capturedText.trim());

	    Assert.assertTrue(ast.nofChildren() == 5);
	    System.out.println("\tVerified: Expected number of children.");
	    child = ast.getChildren()[0];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("EXPRESSION"));
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());	    
	    Assert.assertTrue(0 == capturedText.compareTo("1*3+4"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);

	    child = ast.getChildren()[1];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("SYMBOL"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("/"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText.trim());
  
	    child = ast.getChildren()[2];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("5"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText.trim());

	    child = ast.getChildren()[3];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("SYMBOL"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("*"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);

	    child = ast.getChildren()[4];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("93"));
	    System.out.println("\tVerified: The expected matched string: "+ capturedText);

	    Assert.assertTrue(ast.getChildren()[0].nofChildren() == 5);
	    System.out.println("\tVerified: Expected number of children.");

	    child = ast.getChildren()[0].getChildren()[0];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("1"));
	    System.out.println("\tVerified: The expected matched string: "+ capturedText);

	    child = ast.getChildren()[0].getChildren()[1];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("SYMBOL"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("*"));
	    System.out.println("\tVerified: The expected matched string: "+ capturedText);

	    child = ast.getChildren()[0].getChildren()[2];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("3"));
	    System.out.println("\tVerified: The expected matched string: "+ capturedText);

	    child = ast.getChildren()[0].getChildren()[3];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("SYMBOL"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("+"));
	    System.out.println("\tVerified: The expected matched string: "+ capturedText);

	    child = ast.getChildren()[0].getChildren()[4];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo("4"));
	    System.out.println("\tVerified: The expected matched string: "+ capturedText);

	} catch (Exception e) {
	    e.printStackTrace();
	    fail("Could not parse input1");
	}
    }
 
    public void testInput2() {
	String capturedText;

	try {
	    input = new StringInputIterator(text2);
	    parser = new MathematicalFormula(input);
	    Assert.assertTrue(parser.isMatch());
	    System.out.println("\tVerified: The expected input was matched by parser.");

	    ast = parser.getAST();
	    Assert.assertTrue(0 == ast.getToken().getName().compareTo("EXPRESSION"));
	    System.out.println("\tVerified: The expected token name: " + ast.getToken().getName());
	    capturedText = input.getText(ast.getToken().getStart(), ast.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( text2));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);
	    
	    Assert.assertTrue(ast.nofChildren() == 5);
	    System.out.println("\tVerified: Expected number of children.");
	    
	    child = ast.getChildren()[0];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( "9"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);
	    
	    child = ast.getChildren()[1];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("SYMBOL"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( "+"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);
	    
	    child = ast.getChildren()[2];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("EXPRESSION"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( "9-8"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);
	    
	    child = ast.getChildren()[3];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("SYMBOL"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( "*"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);

	    child = ast.getChildren()[4];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( "10"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);
  
	    Assert.assertTrue(ast.getChildren()[2].nofChildren() == 3);
	    System.out.println("\tVerified: Expected number of children.");

	    child = ast.getChildren()[2].getChildren()[0];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( "9"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);

	    child = ast.getChildren()[2].getChildren()[1];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("SYMBOL"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( "-"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);

	    child = ast.getChildren()[2].getChildren()[2];
	    Assert.assertTrue(0 == child.getToken().getName().compareTo("VALUE"));
	    System.out.println("\tVerified: The expected token name: " + child.getToken().getName());
	    capturedText = input.getText(child.getToken().getStart(), child.getToken().getEnd());
	    Assert.assertTrue(0 == capturedText.compareTo( "8"));
	    System.out.println("\tVerified: The expected matched string: " + capturedText);
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("Could not parse input1");
	}
    }

    
    public MathematicalFormulaTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(MathematicalFormulaTest.class);
	System.exit(0);
    }
}