package robusthaven.text;
import java.io.PrintStream;

public class PrintVisitor implements IAstNodeVisitor {
    PrintStream m_sout;

    public void visitEnter(AstNode node) {
	if (node.nofChildren() > 0) {
	    m_sout.println("VisitEnter: " + node.getToken().getName());
	} else {
	    m_sout.println("VisitEnter: " + node.getToken().getName() 
			   + ": captures " + node.getToken().getStart() + "-" 
			   + node.getToken().getEnd());
	}
    }

    public void visitLeave(AstNode node) {
	m_sout.println("VisitLeave: " + node.getToken().getName());
    }

    public PrintVisitor(PrintStream sout) {
	m_sout = sout;
    }

    /*
     * Prints a linear representation of the AST through sout.
     * Default for sout is std::cout
     */
    public static void printAST(AstNode root, PrintStream sout) {
	PrintVisitor print = new PrintVisitor(sout);

	sout.println("Start - PrintVisitor");
	root.accept(print);
    }

    public static void printAST(AstNode root) {
	printAST(root, System.out);
    }
}
