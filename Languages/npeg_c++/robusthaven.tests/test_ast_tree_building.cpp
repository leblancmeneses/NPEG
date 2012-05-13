#include <cassert>
#include <iostream>
#include "robusthaven/text/AstNode.h"
#include "robusthaven/text/IAstNodeReplacement.h"


using namespace RobustHaven::Text;
using namespace std;

static int g_levelcounters[4] = { 0, 0, 0, 0 };

/*
 * Recursively build a tree (if level < 4):
 * - allocate memory
 * - assign unique integer id to child
 * - assign a number of children that matches the level to the new child.
 */
static void _build_tree(AstNode *p_subroot, const int level) {
  if (level >= 5) return;
  else {
    int i;

    for (i = 0; i < level; i++) {
      AstNode *p_child;
      
      p_child = new AstNode(new TokenMatch("", 100*level + (g_levelcounters[level] += 1), 0), true);
      p_subroot->addChild(p_child);          
      _build_tree(p_child, level + 1);
    }
  }
} /* _build_tree */

static int g_last_visited[5] = { -1, -1, -1, -1, -1};

class _TestVisitor : public IAstNodeVisitor {
  int m_level;

public:
  virtual void visitEnter(AstNode &node) {
    m_level++;
  }

  /*
   * Visits all nodes in the order they were created.
   * Checks that there's no excess memory allocated for "children".
   * Checks that the ID really is unique.
   * Frees up ID memory.
   */
  virtual void visitExecute(AstNode &node) {
    if (m_level >= 5) return;
    else {
      int i;
      
      if (g_last_visited[m_level] != -1) {
	assert(g_last_visited[m_level] + 1 == node.getToken()->getStart());
	cout << "\tVerified: node " << node.getToken()->getStart() << " was visited.\n";
      }
      g_last_visited[m_level] = node.getToken()->getStart();

      assert(node.getChildren().size() == m_level);
      cout << "\tVerified: correct number of children are associated with node.\n";
    }
  }

  virtual void visitLeave(AstNode &node) {
    m_level--;
  }

public:
  _TestVisitor(void) {
    m_level = 0;
  }
};

/*
 * Tests the insertion of nodes into an AST.
 * Capacity is chosen very low in order to ensure that the children array is expanded as expected.
 */
int main(int argc, char *argv[]) {
  AstNode *p_root;
  TokenMatch *p_rootid;
  _TestVisitor visitor;

  p_rootid = new TokenMatch("", 0, 0);
  p_root = new AstNode(p_rootid, true);

  _build_tree(p_root, 1);
  cout << "\tReached: Insertion complete.\n";

  p_root->accept(visitor);
  AstNode::deleteAST(p_root);

  return 0;
} /* main */
