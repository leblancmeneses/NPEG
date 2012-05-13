import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class InputIteratorStdLookupsTest extends TestCase {
    public static final int strlen = 10;
    public static Random rand = new Random();
    
    byte[] string = new byte[strlen+1]; 
    byte[] buffer = new byte[strlen+1];
    StringInputIterator iter;
    
    public void setUp() {
	int i;

	for (i = 0; i < strlen; i++) {
	    if (rand.nextInt()%10 < 2) string[i] = 0;
	    else string[i] = (byte)(rand.nextInt()%('z' - 'a') + 'a');
	}
    }

    public void testStart() {
	StringInputIterator iter;

	try {
	    iter = new StringInputIterator(string, strlen);	
	    Assert.assertTrue(iter.getLength() == strlen);
	    Assert.assertTrue(iter.getIndex() == 0);
	    Assert.assertTrue(iter.getCurrent() == string[0] && iter.getIndex() == 0);
	    Assert.assertTrue(iter.getNext() == string[1] && iter.getIndex() == 1);
	    System.out.println("\tVerified: start of string OK.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testNext() {
	StringInputIterator iter;
	int i;

	try {
	    iter = new StringInputIterator(string, strlen);	
	    for (i = 0; i < strlen - 1; i++) {
		Assert.assertTrue(iter.getNext() == string[i+1]);
	    }
	    Assert.assertTrue(iter.getNext() == -1 && iter.getIndex() == strlen);
	    System.out.println("\tVerified: next works.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public InputIteratorStdLookupsTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(InputIteratorStdLookupsTest.class);
	System.exit(0);
    }
}