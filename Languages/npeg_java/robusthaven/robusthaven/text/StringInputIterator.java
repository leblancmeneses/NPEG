package robusthaven.text;
import java.io.ByteArrayInputStream;
import java.io.IOException;

public class StringInputIterator extends InputIterator {
    public StringInputIterator(byte[] input, int length) throws IOException {       	
	super(new ByteArrayInputStream(input, 0, length), length);
    }

    public static byte[] stringToByteArray(String instring) {
	byte[] outArray;
	int i, len;

	len = instring.length();
	outArray = new byte[len];
	for (i = 0; i < len; i++) {
	    outArray[i] = (byte)instring.charAt(i);
	}
	
	return outArray;
    }

    public StringInputIterator(String instring) throws IOException {       		       
	super(new ByteArrayInputStream(stringToByteArray(instring), 0, instring.length()), 
	      instring.length());
    }
}