import robusthaven.text.*;
import junit.framework.Assert;
import java.util.*;
import java.io.*;

public class InputIteratorGetTextTest extends junit.framework.TestCase {
    public static final int strlen = 10;

    StringInputIterator m_iter;
    byte[] m_string, m_buffer;
 
    public void setUp() {
	Random rand = new Random();
	int i;
	    	    
	m_string = new byte[strlen+1]; m_buffer = new byte[strlen+1];
	    
	for (i = 0; i < strlen; i++) {
	    if (rand.nextInt()%10 < 2) m_string[i] = 0;
	    else m_string[i] = (byte)(rand.nextInt()%('z' - 'a') + 'a');
	}

	try {
	    m_iter = new StringInputIterator(m_string, strlen);
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }

    public void testInit() {
	try {
	    Assert.assertEquals(m_iter.getLength(), strlen);
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }

    public void testCopy() {
	int i;
	    
	try {
	    Assert.assertEquals(m_iter.getText(m_buffer, 0, strlen), strlen);
	    Assert.assertEquals(m_buffer[strlen], 0);
	    for (i = 0; i < strlen; i++) {
		Assert.assertEquals("Extracted text does not match expectation",
						    m_buffer[i], m_string[i]);
	    }
	    System.out.println("\tVerified: copying works.");
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }

    public void testSubstring() {
	try {
	    Assert.assertEquals(m_iter.getText(m_buffer, 2, 5), 3);
	    Assert.assertEquals(m_buffer[3], 0);
	    Assert.assertEquals(m_buffer[0], m_string[2]);
	    Assert.assertEquals(m_buffer[1], m_string[3]);
	    Assert.assertEquals(m_buffer[2], m_string[4]);
	    System.out.println("\tVerified: substring works.");
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }

    public void testNonsenseLimits() {
	try {
	    Assert.assertEquals(m_iter.getText(m_buffer, 2, 1), 0);
	    Assert.assertEquals(m_buffer[strlen], 0);
	    System.out.println("\tVerified: start > end works.");
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }

    public void testOverLong() {
	try {
	    Assert.assertEquals(m_iter.getText(m_buffer, strlen - 1, strlen + 1), 1);
	    Assert.assertEquals(m_buffer[1], 0);
	    Assert.assertEquals(m_buffer[0], m_string[strlen-1]);
	    System.out.println("\tVerified: end > strlen works.");
	} catch (Exception e) {
	    e.printStackTrace();
	    System.exit(-1);
	}
    }

    public InputIteratorGetTextTest(String name) {
	super(name);
    }

    public InputIteratorGetTextTest() {
	super();
    }

    public static junit.framework.Test suite()
    {
	junit.framework.TestSuite suite = new junit.framework.TestSuite();

	return new junit.framework.TestSuite(InputIteratorGetTextTest.class);
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(InputIteratorGetTextTest.class);
	System.exit(0);
    }
}
