package robusthaven.text;

public class Warn {
    long m_iteratorIndex;
    String m_message;

    public long getIteratorIndex() {
	return m_iteratorIndex;
    }

    public String getMessage() {
	return m_message;
    }

    public Warn(String msg, long iteratorIndex) {
	m_iteratorIndex = iteratorIndex;
	m_message = msg;
    }
}