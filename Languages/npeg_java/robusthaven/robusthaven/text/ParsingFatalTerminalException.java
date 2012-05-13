package robusthaven.text;

public class ParsingFatalTerminalException extends RuntimeException {
    long m_errorPos;

    public long getErrorPosistion() {
	return m_errorPos;
    }

    public ParsingFatalTerminalException(String msg, long pos) {
	super(msg);
	m_errorPos = pos;
    }
}