package robusthaven.text;

public class TokenMatch {
    String m_name;
    long m_start, m_end;
    
    public final String getName() {
	return m_name;
    }

    public long getStart() {
	return m_start;
    }

    public long getEnd() {
	return m_end;
    }

    public TokenMatch(String name, long start, long end) {
	m_name = name;
	m_start = start;
	m_end = end;
    }
}