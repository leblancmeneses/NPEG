import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class AnyCharacterTerminalTest extends TestCase {
    public class AnyTest extends Npeg {
	private class AnyCharMatch implements IsMatchPredicate {
	    public boolean evaluate() throws ParsingFatalTerminalException, IOException {
		return anyCharacter();
	    }
	}

	public boolean isMatch() throws ParsingFatalTerminalException, IOException {
	    return new AnyCharMatch().evaluate();
	}
    
	public AnyTest(InputIterator iter) {
	    super(iter, null);
	}
    }

    public static final int randstringlen = 10;
    byte[] randomstring = new byte[randstringlen];
    StringInputIterator iterator;
    Random rand = new Random();
    Npeg context;

    /*
     * - Test that consumption of an empty string results in failure.
     * - Test that consumption of a random string works until the end of the string is reached.
     */
    public void testEmpty() {
	try {
	    iterator = new StringInputIterator(randomstring, 0);
	    context = new AnyTest(iterator);
	    Assert.assertTrue(context.isMatch() == false);
	    System.out.println("\tVerified: handling of empty string");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public void testConsumption() {
	int i;

	try {
	    for (i = 0; i < randstringlen; i++) {
		int r;
		
		r = rand.nextInt(); r = r < 0? -r : r;
		randomstring[i] = (byte)(r%('z' - 'a') + 'a');
	    }
  
	    iterator = new StringInputIterator(randomstring, randstringlen);
	    context = new AnyTest(iterator);
	    for (i = 0; i < randstringlen; i++) {
		Assert.assertTrue(iterator.getCurrent() == randomstring[i]);
		Assert.assertTrue(context.isMatch() == true);
	    }
	    System.out.println("\tVerified: character consumption");
	    
	    Assert.assertTrue(context.isMatch() == false);
	    System.out.println("\tVerified: handling of end of string");
	} catch (Exception e) {
	    e.printStackTrace();
	    fail("unexpected error");
	}
    }

    public AnyCharacterTerminalTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(AnyCharacterTerminalTest.class);
	System.exit(0);
    }
}