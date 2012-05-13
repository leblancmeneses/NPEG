import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class CodePointTerminalTest extends TestCase {
    static final String OTHER_STRING = "blah this is something else";
    static final String input1 = ((((((OTHER_STRING + (char)0x78) + (char)0x9a) + (char)0xbc)
				    + (char)0xde) + (char)0xf0) + (char)0x01) + (char)0x02;
    static final String match_h1 = "#x789abcdef00102";
    static final String match_h1w = "#x789Xbcxef00X02";
    static final String match_b1 = "#b"+ "111" + "1000" + "1001" + "1010";
    static final String match_b1w = "#b" + "11X" + "x000" + "100X" + "1x10";
    static final String input2 = OTHER_STRING + (char)0x08 + (char)0x9a + (char)0xbc + (char)0xde
	+ (char)0xf0 + (char)0x01 + (char)0x02;
    static final String match_h2 = "#x89abcdef00102";
    static final String match_h2w = "#x8XabxdeX00x02";
    static final String input3 = OTHER_STRING + (char)0xf0 + (char)0x9a + (char)0xbc;
    static final String match_h3 = "#xf09abc";
    static final String match_b3 = "#b" + "1111" + "0000" + "1001" + "1010" + "1011" + "1100";
    static final String input4 = new Character((char)0x08).toString() + new Character((char)0x9a).toString();
    static final String input5 = OTHER_STRING + (char)0;
    static final String match_d5 = "#0";

    class CPTest extends Npeg {
	byte[] m_codeStr;
	int m_len;

	public void setCode(byte[] codeStr, int len) {
	    m_codeStr = codeStr;
	    m_len = len;
	}

	public void setCode(String code) {
	    try {
		setCode(StringInputIterator.stringToByteArray(code), code.length());
	    } catch (Exception e) {
		e.printStackTrace();
		System.exit(-1);
	    }
	}

	public boolean isMatch() throws ParsingFatalTerminalException, IOException {
	    return codePoint(m_codeStr, m_len);
	}

	public CPTest(InputIterator iter) {
	    super(iter, null);
	}
    }

    int match_d3_val, match_d4_val, i;
    String match_d3, match_d4;
    StringInputIterator iterator;
    CPTest context;

    public void setUp() {
	match_d3_val = 0;
	for (i = 0; i < 3; i++) {
	    match_d3_val = (match_d3_val << 8) + (int)input3.charAt(OTHER_STRING.length()+i);
	}
	match_d3 = "#" + match_d3_val; 
	
	match_d4_val = 0;
	for (i = 0; i < 2; i++) {
	    match_d4_val = (match_d4_val << 8) + (int)input4.charAt(i);
	}
	match_d4 = "#" + match_d4_val;  	    
    }

    public void testMismatch() {
	try {	    
	    iterator = new StringInputIterator(input1);
	    context = new CPTest(iterator);
	    context.setCode(match_h1.getBytes(), match_h1.length());
	    Assert.assertTrue(context.isMatch() == false);
	    context.setCode(match_b1.getBytes(), match_b1.length());
	    Assert.assertTrue(context.isMatch() == false);
	    context.setCode(match_d4);
	    Assert.assertTrue(context.isMatch() == false);
	    Assert.assertTrue(iterator.getIndex() == 0);
	    System.out.println("\tVerfied: Handling of string mismatch");	  
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testMismatchWC() {
	try {	    
	    iterator = new StringInputIterator(input1.getBytes(), input1.length());
	    context = new CPTest(iterator);
	    context.setCode(match_h1w.getBytes(), match_h1.length());
	    Assert.assertTrue(context.isMatch() == false);
	    context.setCode(match_b1w.getBytes(), match_b1.length());
	    Assert.assertTrue(context.isMatch() == false);
	    Assert.assertTrue(iterator.getIndex() == 0);
	    System.out.println("\tVerfied: Handling of string mismatch with wildcards");	  
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput1() {
	try {	    
	    iterator = new StringInputIterator(input1);
	    context = new CPTest(iterator);
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_h1);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == input1.length());
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_b1);
	    Assert.assertTrue(context.isMatch() == true && iterator.getIndex() == OTHER_STRING.length() + 2);
	    System.out.println("\tVerfied: Parsing of input1: no padding w/ hex, 1bit padding w/ binary");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput1WC() {
	try {	    
	    iterator = new StringInputIterator(input1);
	    context = new CPTest(iterator);
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_h1w);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == input1.length());
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_b1w);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == OTHER_STRING.length() + 2);
	    System.out.println("\tVerfied: Parsing of input1 /w wildcards: no padding w/ hex, "
			       + "1bit padding w/ binary");	  
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput2() {
	try {	    
	    iterator = new StringInputIterator(input2);
	    context = new CPTest(iterator);
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_h2);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == input2.length());
	    System.out.println("\tVerfied: Parsing of input2: padding of 4bits w/ hex");	  
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput2WC() {
	try {	    
	    iterator = new StringInputIterator(input2);
	    context = new CPTest(iterator);
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_h2w);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == input2.length());
	    System.out.println("\tVerfied: Parsing of input2 with wildcards: padding of 4bits w/ hex");	  
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput3() {
	try {	    
	    iterator = new StringInputIterator(input3);
	    context = new CPTest(iterator);
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_h3);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == input3.length());
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_b3);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == OTHER_STRING.length() + 3);
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_d3);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == input3.length());
	    System.out.println("\tVerfied: Parsing of input3: no padding");	  
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput4() {
	try {	    
	    iterator = new StringInputIterator(input4);
	    context = new CPTest(iterator);
	    context.setCode(match_d4);
	    Assert.assertTrue(context.isMatch() == true);
	    System.out.println("\tVerfied: Parsing of input4: short decimal test");	  
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testInput5() {
	try {	    
	    iterator = new StringInputIterator(input5.getBytes(), input5.length() + 1);
	    context = new CPTest(iterator);
	    iterator.setIndex(OTHER_STRING.length());
	    context.setCode(match_d5);
	    Assert.assertTrue(context.isMatch() == true);
	    Assert.assertTrue(iterator.getIndex() == OTHER_STRING.length() + 1);
	    System.out.println("\tVerfied: Parsing of input5: matching 0 with decimals");	  
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public CodePointTerminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(CodePointTerminalTest.class);
	System.exit(0);
    }
}
