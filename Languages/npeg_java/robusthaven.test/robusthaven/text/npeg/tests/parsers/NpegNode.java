package robusthaven.text.npeg.tests.parsers;
import java.io.IOException;
import robusthaven.text.*;

public class NpegNode extends Npeg
{
    public NpegNode(InputIterator iterator)
    {
	super(iterator, null);
    }

    public boolean isMatch() throws IOException, ParsingFatalTerminalException
    {
	return new NpegNode_impl_0().evaluate();
    }


    protected class NpegNode_impl_0 implements IsMatchPredicate
    {
	public boolean evaluate() throws ParsingFatalTerminalException, InfiniteLoopException, IOException
	{
	    String _nodeName_0 = "NpegNode";
	    return capturingGroup(new NpegNode_impl_1(), _nodeName_0, false, false);
	}
    }

    protected class NpegNode_impl_1 implements IsMatchPredicate
    {
	public boolean evaluate() throws ParsingFatalTerminalException, InfiniteLoopException, IOException
	{
	    return prioritizedChoice(new NpegNode_impl_2(), new NpegNode_impl_3());
	}
    }

    protected class NpegNode_impl_3 implements IsMatchPredicate
    {
	public boolean evaluate() throws ParsingFatalTerminalException, InfiniteLoopException, IOException
	{
	    String _literal_0 = ".NET Parsing Expression Grammar";
	    return literal(_literal_0.getBytes(), _literal_0.length(), false);
	}
    }

    protected class NpegNode_impl_2 implements IsMatchPredicate
    {
	public boolean evaluate() throws ParsingFatalTerminalException, InfiniteLoopException, IOException
	{
	    String _literal_0 = "NPEG";
	    return literal(_literal_0.getBytes(), _literal_0.length(), false);
	}
    }
}
