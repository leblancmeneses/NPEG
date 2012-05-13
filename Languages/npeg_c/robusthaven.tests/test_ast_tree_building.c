#include <stdlib.h>
#include <assert.h>
#include <stdio.h>
#include "robusthaven/text/npeg_ast.h"

static int _levelcounters[4] = { 0, 0, 0, 0 };
static const uint _test_capacity = 1; 

/*
 * Recursively build a tree (if level < 4):
 * - allocate memory
 * - assign unique integer id to child
 * - assign a number of children that matches the level to the new child.
 */
static void _build_tree(npeg_astnode *p_subroot, const int level) {
  if (level >= 4) return;
  else {
    int *p_token, i;

    assert(p_subroot->capacity >= p_subroot->nof_children);
    printf("\tVerified: Capacity sufficient for storing all child pointers.\n");
    
    for (i = 0; i < level; i++) {
      npeg_astnode *p_child;

      p_child = (npeg_astnode*)malloc(sizeof(npeg_astnode));
      p_token = (int*)malloc(sizeof(int));
      *p_token = 100*level + (_levelcounters[level] += 1);
      npeg_astnode_add_child(p_subroot, p_child);          
      npeg_astnode_constructor(p_child, (npeg_token*)p_token, _test_capacity);   
      _build_tree(p_child, level + 1);
    }
  }
} /* _build_tree */

static int _last_visited[4] = { -1, -1, -1, -1};

/*
 * Visits all nodes in the order they were created.
 * Checks that there's no excess memory allocated for "children".
 * Checks that the ID really is unique.
 * Frees up ID memory.
 */
void _visit_node(npeg_astnode *p_node, const int level) {
  if (level >= 4) return;
  else {
    int i;

    assert(p_node->nof_children == 0 || p_node->nof_children == p_node->capacity);
    printf("\tVerified: no excess memory in node (trimmed) or no children.\n");

    if (_last_visited[level] != -1) {
      assert(_last_visited[level] + 1 == *((int*)p_node->token));
      printf("\tVerified: node %d was visited.\n", *(int*)p_node->token);
    }
    _last_visited[level] = *((int*)p_node->token);

    assert(p_node->nof_children == level);
    printf("\tVerified: correct number of children are associated with node.\n");
    for (i = 0; i < level; i++) _visit_node(p_node->children[i], level + 1);
  }
} 

static void _ondelete_visitor(npeg_astnode *p_node) {
  free(p_node->token);
} 

/*
 * Tests the insertion of nodes into an AST.
 * Capacity is chosen very low in order to ensure that the children array is expanded as expected.
 */
int main(int argc, char *argv[]) {
  npeg_astnode *p_root;
  int *p_rootid;

  p_root = malloc(sizeof(npeg_astnode));
  p_rootid = malloc(sizeof(int));
  *p_rootid = 0;

  npeg_astnode_constructor(p_root, (npeg_token*)p_rootid, _test_capacity);
  _build_tree(p_root, 1);
  printf("\tReached: Insertion complete.\n");

  npeg_astnode_trim_childrenarrays(p_root);
  printf("\tReached: Memory trimmed.\n");

  _visit_node(p_root, 1);
  npeg_astnode_delete_tree(p_root, _ondelete_visitor);

  return 0;
} /* main */
