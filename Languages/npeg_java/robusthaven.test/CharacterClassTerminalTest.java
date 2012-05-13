import robusthaven.text.*;
import java.util.*;
import java.io.*;
import junit.framework.Assert;
import junit.framework.TestCase;

public class CharacterClassTerminalTest extends TestCase {
    public class ClassTest extends Npeg {
	byte[] m_classStr;
	int m_len;

	public void setClass(String classStr) {
	    try {
		setClass(classStr.getBytes("UTF-8"), classStr.length());
	    } catch (Exception e) {
		e.printStackTrace();
		System.exit(-1);
	    }
	}

	public void setClass(byte[] classStr, int len) {
	    m_classStr = classStr;
	    m_len = len;
	}

	public boolean isMatch() throws ParsingFatalTerminalException, IOException {
	    return characterClass(m_classStr, m_len);
	}

	public ClassTest(InputIterator iter) {
	    super(iter, null);
	}
    }

    byte[] input = new byte[2];
    byte[] cClass = new byte[100];
    int i = 0;			
    StringInputIterator iterator;
    ClassTest context;
    boolean errorSeen;

    public void testEmpty() {
	try {			
	    input[0] = 0; input[1] = 0; 
	    errorSeen = false;
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    try {
		context.setClass(input, 0);
		context.isMatch();    
	    } catch (ParsingFatalTerminalException e) {
		System.out.println(e.getMessage());
		Assert.assertTrue(0 == e.getMessage().compareTo("CharacterClass definition must be a minimum of 3 characters [expression]"));	
		System.out.println("\tVerified: expected exception message. " + e.getMessage());
		errorSeen = true;
	    }
	    Assert.assertTrue(errorSeen);
	    System.out.println("\tVerified: empty character class leads to error.");
	    Assert.assertTrue(0 == context.getWarnings().size());	
	    System.out.println("\tVerified: given InvalidExpressionException context.warnings collection will not be affected and have zero items.");
	    Assert.assertTrue(0 == iterator.getIndex());
	    System.out.println("\tVerified: On no match iterator should not consume character.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testAAA() {
	try {			
	    System.out.println("");		
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    try {
		errorSeen = false;
		context.setClass("aaa");
		context.isMatch();
		System.out.println("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.");
	    } catch (ParsingFatalTerminalException e) {
		Assert.assertTrue(0 == e.getMessage().compareTo("CharacterClass definition must start with ["));	
		System.out.println("\tVerified: expected exception message.  " + e.getMessage());
		errorSeen = true;
	    }
	    Assert.assertTrue(errorSeen);
	    System.out.println("\tVerified: invalid character class leads to error.");  
	    Assert.assertTrue(0 == context.getWarnings().size());	
	    System.out.println("\tVerified: given InvalidExpressionException context.warnings collection will not be affected and have zero items.");
	    Assert.assertTrue(0 == iterator.getIndex());
	    System.out.println("\tVerified: On no match iterator should not consume character.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testHalfAA() {
	try {			
	    System.out.println("");
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    try {
		errorSeen = false;
		context.setClass("[aa");
		context.isMatch();
		System.out.println("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.");
	    } catch (ParsingFatalTerminalException e) {
		Assert.assertTrue(0 == e.getMessage().compareTo("CharacterClass definition must end with ]"));	
		System.out.println("\tVerified: expected exception message." + e.getMessage());
		errorSeen = true;
	    }
	    Assert.assertTrue(errorSeen);
	    System.out.println("\tVerified: invalid character class leads to error.");  
	    Assert.assertTrue(0 == context.getWarnings().size());	
	    System.out.println("\tVerified: given InvalidExpressionException context.warnings collection will not be affected and have zero items.");
	    Assert.assertTrue(0 == iterator.getIndex());
	    System.out.println("\tVerified: On no match iterator should not consume character.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }


    public void testInvalidSlash() {
	try {				
	    System.out.println("");
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    try {
		errorSeen = false;
		context.setClass("[\\]");
		context.isMatch();
		System.out.println("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.");
	    } catch (ParsingFatalTerminalException e) {
		Assert.assertTrue(0 == e.getMessage().compareTo("CharacterClass definition requires user to escape "
				   + "'\\' given location in expression. User must escape by specifying '\\\\'"));	
		System.out.println("\tVerified: expected exception message." + e.getMessage());
		errorSeen = true;
	    }
	    Assert.assertTrue(errorSeen);
	    System.out.println("\tVerified: invalid character class leads to error.");  
	    Assert.assertTrue(0 == context.getWarnings().size());	
	    System.out.println("\tVerified: given InvalidExpressionException context.warnings collection will not be affected and have zero items.");
	    Assert.assertTrue(0 == iterator.getIndex());
	    System.out.println("\tVerified: On no match iterator should not consume character.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInvalidMinus() {
	try {	
	    System.out.println("");
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    try {
		errorSeen = false;
		context.setClass("[-]");
		context.isMatch();
		System.out.println("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.");
	    } catch (ParsingFatalTerminalException e) {
		Assert.assertTrue(0 == e.getMessage().compareTo("CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'"));	
		System.out.println("\tVerified: expected exception message." + e.getMessage());
		errorSeen = true;
	    }
	    Assert.assertTrue(errorSeen);
	    System.out.println("\tVerified: invalid character class leads to error.");  
	    Assert.assertTrue(0 == context.getWarnings().size());	
	    System.out.println("\tVerified: given InvalidExpressionException context.warnings collection will not be affected and have zero items.");
	    Assert.assertTrue(0 == iterator.getIndex());
	    System.out.println("\tVerified: On no match iterator should not consume character.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testAMinus() {
	try {				
	    System.out.println("");
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    try {
		errorSeen = false;
		context.setClass("[a-]");
		context.isMatch();
		System.out.println("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.");
	    } catch (ParsingFatalTerminalException e) {
		Assert.assertTrue(0 == e.getMessage().compareTo("CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'"));	
		System.out.println("\tVerified: expected exception message." + e.getMessage());
		errorSeen = true;
	    }
	    Assert.assertTrue(errorSeen);
	    System.out.println("\tVerified: invalid character class leads to error.");  
	    Assert.assertTrue(0 == context.getWarnings().size());	
	    System.out.println("\tVerified: given InvalidExpressionException context.warnings collection will not be affected and have zero items.");
	    Assert.assertTrue(0 == iterator.getIndex());
	    System.out.println("\tVerified: On no match iterator should not consume character.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

	
	
    public void testSlashL() {
	try {			
	    System.out.println("");
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    try {
		errorSeen = false;
		context.setClass("[\\L]");
		context.isMatch();
		System.out.println("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.");
	    } catch (ParsingFatalTerminalException e) {
		Assert.assertTrue(0 == e.getMessage().compareTo("CharacterClass definition contains an invalid escape sequence. Accepted sequences: \\\\, \\s, \\S, \\d, \\D, \\w, \\W"));	
		System.out.println("\tVerified: expected exception message." + e.getMessage());
		errorSeen = true;
	    }
	    Assert.assertTrue(errorSeen);
	    System.out.println("\tVerified: invalid character class leads to error.");  
	    Assert.assertTrue(0 == context.getWarnings().size());	
	    System.out.println("\tVerified: given InvalidExpressionException context.warnings collection will not be affected and have zero items.");
	    Assert.assertTrue(0 == iterator.getIndex());
	    System.out.println("\tVerified: On no match iterator should not consume character.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }
		       	
    public void testNumeric() {
	try {			
	    System.out.println("\tReached: validate simple character ranges.");
	    for(i = 0; i <= 9; i++)
		{
		    input[0] = (byte)('0' + i);
		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[0-9]");      
		    Assert.assertTrue(true == context.isMatch());
		    System.out.println("\tVerified: [0-9] match with " + input[0] + " as input.");
		    Assert.assertTrue(1 == iterator.getIndex());
		    System.out.println("\tVerified: On match iterator should consume character.");
	
		}
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }
	
    public void testAlphaNum() {
	try {			
	    System.out.println("");
	    System.out.println("\tReached: validate grouped character ranges.");
	    for(i = 0; i < 26; i++)
		{
		    input[0] = (byte)('a' + i);
		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[A-Z0-9a-z]");      
		    Assert.assertTrue(true == context.isMatch());
		    System.out.println("\tVerified: [A-Z0-9a-z] match with " + input[0] + " as input.");
		    Assert.assertTrue(1 == iterator.getIndex());
		    System.out.println("\tVerified: On match iterator should consume character.");
			
		    input[0] = (byte)('A' + i);
		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[A-Z0-9a-z]");      
		    Assert.assertTrue(true == context.isMatch());
		    System.out.println("\tVerified: [A-Z0-9a-z] match with " + (char)input[0] + "as input.");
		    Assert.assertTrue(1 == iterator.getIndex());
		    System.out.println("\tVerified: On match iterator should consume character.");
	
		
		    input[0] = (byte)('0' + i%10);
		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[A-Z0-9a-z]");      
		    Assert.assertTrue(true == context.isMatch());
		    System.out.println("\tVerified: [A-Z0-9a-z] match with " + (char)input[0] + "as input.");
		    Assert.assertTrue(1 == iterator.getIndex());
		    System.out.println("\tVerified: On match iterator should consume character.");
	
		}
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }


	
    public void testSimpleEsc() {
	try {			
	    System.out.println("");
	    System.out.println("\tReached: interpreted simple escape sequence.");
	    for(i = 0; i <= 9; i++)
		{
		    input[0] = (byte)('0' + i);
		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[\\d]");      
		    Assert.assertTrue(true == context.isMatch());
		    System.out.println("\tVerified: \\d match with " + i + " as input.");
		    Assert.assertTrue(1 == iterator.getIndex());
		    System.out.println("\tVerified: On match iterator should consume character.");
	
		}
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }
	
	
    public void testGroupedEsc() {
	try {			
	    System.out.println("");
	    System.out.println("\tReached: interpreted groupped escape sequence.");

	    //"[\\d\\D\\s\\S\\w\\W]"
	    for(i = 0; i <= 9; i++)
		{
		    input[0] = (byte)('0' + i);
		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[\\d\\s]");      
		    Assert.assertTrue(true == context.isMatch());
		    System.out.println("\tVerified: [\\d\\s] match with " + i + " as input.");
		    Assert.assertTrue(1 == iterator.getIndex());
		    System.out.println("\tVerified: On match iterator should consume character.");
	
		
		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[\\s\\w]");      
		    Assert.assertTrue(true == context.isMatch());
		    System.out.println("\tVerified: [\\s\\w] match with " + i + " as input.");
		    Assert.assertTrue(1 == iterator.getIndex());
		    System.out.println("\tVerified: On match iterator should consume character.");
	
		}
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testSpecialEsc() {
	try {			
	    System.out.println("");
	    input[0] = ' ';
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    context.setClass("[\\d\\s]");      
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: [\\d\\s] match with ' ' as input.");
	    Assert.assertTrue(1 == iterator.getIndex());
	    System.out.println("\tVerified: On match iterator should consume character.");

	    input[0] = '\n';
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    context.setClass("[\\d\\s]");      
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: [\\d\\s] match with \\n as input.");
	    Assert.assertTrue(1 == iterator.getIndex());
	    System.out.println("\tVerified: On match iterator should consume character.");


	
	    input[0] = '\f';
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    context.setClass("[\\d\\s]");      
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: [\\d\\s] match with \\f as input.");
	    Assert.assertTrue(1 == iterator.getIndex());
	    System.out.println("\tVerified: On match iterator should consume character.");

	
	    input[0] = '\r';
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    context.setClass("[\\d\\s]");      
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: [\\d\\s] match with \\r as input.");
	    Assert.assertTrue(1 == iterator.getIndex());
	    System.out.println("\tVerified: On match iterator should consume character.");

	
	    input[0] = '\t';
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    context.setClass("[\\d\\s]");      
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: [\\d\\s] match with \\t as input.");
	    Assert.assertTrue(1 == iterator.getIndex());
	    System.out.println("\tVerified: On match iterator should consume character.");       	


	    System.out.println("");
	    System.out.println("\tReached: confirming negative character group.");
	    System.out.println("");
	
	    System.out.println("\tReached: confirming single negative character group.");
	    input[0] = 'b';
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    context.setClass("[^a]");      
	    Assert.assertTrue(true == context.isMatch());
	    System.out.println("\tVerified: [^a] matches input b.");
	    Assert.assertTrue(1 == iterator.getIndex());
	    System.out.println("\tVerified: On match iterator should consume character.");
	
	
	    input[0] = 'a';
	    iterator = new StringInputIterator(input, 2);
	    context = new ClassTest(iterator);
	    context.setClass("[^a]");      
	    Assert.assertTrue(false == context.isMatch());
	    System.out.println("\tVerified: [^a] does matches input a.");
	    Assert.assertTrue(0 == iterator.getIndex());
	    System.out.println("\tVerified: On not match character is not consummed by iterator.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testNegative() {
	try {			
	    System.out.println("\tReached: confirming escaped negative character group.");
	    for(i = 0; i < 26; i++)
		{
		    input[0] = (byte)('a' + i);
		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[^\\W]");      
		    Assert.assertTrue(true == context.isMatch());
		    System.out.println("\tVerified: [^\\W] which means [\\w] match with " + (char)input[0] + "as input.");
		    Assert.assertTrue(1 == iterator.getIndex());
		    System.out.println("\tVerified: On match iterator should consume character.");

		    iterator = new StringInputIterator(input, 2);
		    context = new ClassTest(iterator);
		    context.setClass("[^\\S]");      
		    Assert.assertTrue(false == context.isMatch());
		    System.out.println("\tVerified: [^\\S] which translates to postive character group [\\s] will not match with " + (char)input[0] + "as input.");
		    Assert.assertTrue(0 == iterator.getIndex());
		    System.out.println("\tVerified: On no match character is not consummed by iterator.");
		}
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public CharacterClassTerminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(CharacterClassTerminalTest.class);
	System.exit(0);
    }
}
