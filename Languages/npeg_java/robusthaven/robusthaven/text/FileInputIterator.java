package robusthaven.text;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.File;

public class FileInputIterator extends InputIterator {
    public FileInputIterator(String path) throws IOException {
	super(new FileInputStream(new File(path)), new File(path).length());
    }
}