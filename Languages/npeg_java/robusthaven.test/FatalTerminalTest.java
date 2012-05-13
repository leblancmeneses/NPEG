import robusthaven.text.*;
import java.util.*;
import java.io.*;
import junit.framework.Assert;
import junit.framework.TestCase;

public class FatalTerminalTest extends TestCase {
    public static final String g_errorStr = "some error";

    class FatalTest extends Npeg {
	public boolean isMatch() throws ParsingFatalTerminalException, IOException {
	    return fatal(g_errorStr);
	}
	
	public FatalTest(InputIterator iter) {
	    super(iter, null);
	}
    }

    public void testFatal() {
	byte[] str = new byte[1];

	StringInputIterator iterator;
	Npeg context;
	boolean errorSeen;
		
	errorSeen = false;
	try {
	    iterator = new StringInputIterator(str, 0);
	    context = new FatalTest(iterator);

	    context.isMatch();
	} catch (ParsingFatalTerminalException e) {
	    errorSeen = true;
	    assert(e.getMessage().compareTo(g_errorStr) == 0);
	    System.out.println("\tVerified: error string matches input");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}

	assert(errorSeen);
	System.out.println("\tVerified: fatal works.");
    }

    public FatalTerminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(FatalTerminalTest.class);
	System.exit(0);
    }
}
