import robusthaven.text.*;
import junit.framework.Assert;
import junit.framework.TestCase;
import java.util.*;
import java.io.*;

public class AstTreeBuildingTest extends TestCase {
    private static int[] g_levelcounters = new int[5];

    /*
     * Recursively build a tree (if level < 4):
     * - allocate memory
     * - assign unique integer id to child
     * - assign a number of children that matches the level to the new child.
     */
    private static void _build_tree(AstNode subroot, int level) {
	if (level >= 5) return;
	else {
	    int i;

	    for (i = 0; i < level; i++) {
		AstNode child;
      
		child = new AstNode(new TokenMatch("", 100*level + (g_levelcounters[level] += 1), 0));
		subroot.addChild(child);          
		_build_tree(child, level + 1);
	    }
	}
    } /* _build_tree */

    private static long[] g_last_visited = new long[5];

    private static class _TestVisitor implements IAstNodeVisitor {
	int m_level;

	public void visitEnter(AstNode node) {
	    m_level++;
	}
	
	/*
	 * Visits all nodes in the order they were created.
	 * Checks that there's no excess memory allocated for "children".
	 * Checks that the ID really is unique.
	 * Frees up ID memory.
	 */
	public void visitExecute(AstNode node) {
	    if (m_level >= 5) return;
	    else {
		int i;
		
		if (g_last_visited[m_level] != -1) {
		    Assert.assertTrue(g_last_visited[m_level] + 1 == node.getToken().getStart());
		    System.out.println("\tVerified: node " + node.getToken().getStart() 
				       + " was visited.");
		}
		g_last_visited[m_level] = node.getToken().getStart();
		
		Assert.assertTrue(node.nofChildren() == m_level);
		System.out.println("\tVerified: correct number of children are associated with node.");
	    }
	}

	public void visitLeave(AstNode node) {
	    m_level--;
	}

	public _TestVisitor() {
	    m_level = 0;
	}

	public AstNode createCustomNode(AstNode innode) {
	    return innode;
	}
    }

    /*
     * Tests the insertion of nodes into an AST.
     * Capacity is chosen very low in order to ensure that the children array is expanded as expected.
     */
    public static void testBuilding() {
	AstNode root;
	TokenMatch rootid;
	_TestVisitor visitor;
	
	g_levelcounters[0] = 0; g_levelcounters[1] = 0;
	g_levelcounters[2] = 0; g_levelcounters[3] = 0; 
	g_levelcounters[4] = 0;

	g_last_visited[0] = -1; g_last_visited[1] = -1; 
	g_last_visited[2] = -1; g_last_visited[3] = -1;
	g_last_visited[4] = -1;

	rootid = new TokenMatch("", 0, 0);
	root = new AstNode(rootid);
	visitor = new _TestVisitor();
	
	_build_tree(root, 1);
	System.out.println("\tReached: Insertion complete.");
	
	root.accept(visitor);
    } /* main */

    public AstTreeBuildingTest() {
	super();
    }

    public static void main(String[] argv) {
	junit.textui.TestRunner.run(AstTreeBuildingTest.class);
	System.exit(0);
    }
}