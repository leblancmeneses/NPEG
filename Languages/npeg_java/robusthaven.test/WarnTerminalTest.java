import robusthaven.text.*;
import java.util.*;
import java.io.*;
import junit.framework.Assert;
import junit.framework.TestCase;

public class WarnTerminalTest extends TestCase {
    public static String g_string = "A user warning message.";
 
    class WarnTest extends Npeg {
	public boolean isMatch() throws ParsingFatalTerminalException {
	    return warn(g_string);
	}
	
	public WarnTest(InputIterator iter) {
	    super(iter, null);
	}
    }

    public void testWarn() {
	StringInputIterator iterator;
	Npeg context;
	
	try {
	    iterator = new StringInputIterator(g_string);
	    context = new WarnTest(iterator);
		
	    Assert.assertTrue( 0 == context.getWarnings().size() );
	    System.out.println("\tVerified: context warnings is zero at start.");
		
	    Assert.assertTrue( true == context.isMatch() );
	    System.out.println("\tVerified: npeg_Warn will always return true.");
		
	    Assert.assertTrue( 1 == context.getWarnings().size());
	    System.out.println("\tVerified: npeg_Warn will add an warning to context warnings collection.");
	
	    Assert.assertTrue(context.getWarnings().lastElement().getMessage() == "A user warning message.");
	    System.out.println("\tVerified: that the original warn message was copied in full to the new allocated npeg managed memory.");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public WarnTerminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(WarnTerminalTest.class);
	System.exit(0);
    }
}

