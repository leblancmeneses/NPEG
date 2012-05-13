package robusthaven.text;

public class InfiniteLoopException extends ParsingFatalTerminalException {
    InfiniteLoopException(String msg, long pos) {
	super(msg, pos);
    }
}