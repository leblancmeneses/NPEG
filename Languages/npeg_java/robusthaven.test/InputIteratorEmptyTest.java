import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class InputIteratorEmptyTest extends TestCase {
    public void testEmpty() {
	InputIterator iter;
	Random rand = new Random();
	byte[] buffer = new byte[101];
	int i;

	try {
	    iter = new StringInputIterator(buffer, 0);
	    for (i = 0; i < 10; i++) {
		int start, end;
	    
		buffer[0] = 1; buffer[1] = 1; buffer[2] = 1; buffer[3] = 1;
		start = rand.nextInt()%100; end = rand.nextInt()%100;
		start = start < 0? -start : start; end = end < 0? -end : end;
		
		junit.framework.Assert.assertTrue(iter.getText(buffer, start, end) == 0);
		System.out.println("\tVerified: iterator with empty string is handled correctly.");
		
		Assert.assertTrue(buffer[0] == 0 && buffer[1] == 1 && buffer[2] == 1);
		System.out.println("\tVerified: no modification of destination buffer apart " + 
				   "from first byte.");
	    }

	    System.out.println("\tReached: get_text works.");
    
	    Assert.assertTrue(iter.getCurrent() == -1);
	    Assert.assertTrue(iter.getNext() == -1);			   
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public InputIteratorEmptyTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(InputIteratorEmptyTest.class);
	System.exit(0);
    }
}