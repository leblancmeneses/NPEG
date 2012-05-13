package robusthaven.text;
import java.io.InputStream;
import java.io.IOException;

public class InputIterator {
    InputStream m_stream;
    long m_index, m_length;
    int m_current;

    public synchronized long getIndex() {
	return m_index;
    }

    public synchronized void setIndex(long index) throws IOException {
	m_index = index;
	m_stream.reset();
	m_stream.mark((int)m_length);
	m_stream.skip(index);
	m_current = m_stream.read();
    }

    public synchronized long getLength() throws IOException {
	return m_length;
    }

    public synchronized int getNext() throws IOException {
	m_index += 1;

	return (m_current = m_stream.read());
    }

    public synchronized int getCurrent() throws IOException {
	return m_current;
    }

    public synchronized int getText(byte[] buffer, long start, long end) throws IOException {
	long pos;
	int nofRead;

	if (getLength() == 0 || start > end) nofRead = 0;
	else {
	    pos = m_index;
	    setIndex(start);
	    if (m_current == -1) {
		buffer[0] = 0;
		nofRead = 0;
	    } else {
		buffer[0] = (byte)m_current;
		nofRead = m_stream.read(buffer, 1, (int)(end - start - 1));
		if (nofRead < 0) nofRead = 0;
		nofRead += 1;
	    }
	    setIndex(pos);		    
	    assert(nofRead <= end - start);
	}
	buffer[nofRead] = 0;

	return nofRead;
    }

    public String getText(long start, long end) throws IOException {
	byte[] buffer = new byte[(int)(end - start + 1)];
	int nofRead;
	
	nofRead = getText(buffer, start, end);

	return new String(buffer, 0, nofRead);
    }

    public InputIterator(InputStream stream, long length) throws IOException {
	if (!stream.markSupported()) throw new IOException("Unsuitable InputStream object");
	
	m_stream = stream;
	m_index = 0;
	m_length = length;
	m_stream.mark((int)m_length);
	m_current = m_stream.read();	
    }
}